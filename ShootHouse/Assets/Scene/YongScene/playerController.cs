using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerController : MonoBehaviour, IDamage, IPickUp
{
    [Header("Movement Settings")]
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] float HP;
    [SerializeField] int speed;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpSpeed;
    [SerializeField] int jumpMax;
    [SerializeField] int gravity;
    private Transform hipMuzzleFlashPos;
    private Transform aimMuzzleFlashPos;
    private ParticleSystem hipMuzzleFlashEffect;
    private ParticleSystem aimMuzzleFlashEffect;

    
    private int currentAmmo;
    private int maxAmmo;
   
    private bool isAiming = false;
    public bool isReloading = false;
    public Vector3 originalRotation;

    [Header("Shooting Settings")]
    [SerializeField] float shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;
    [SerializeField] GameObject gunModel;
    [SerializeField] List<GunStats> gunList = new List<GunStats>();
    [SerializeField] private Transform muzzleFlashPos;
    private ParticleSystem muzzleFlashEffect;

    [Header("Stamina Settings")]
    [SerializeField] float maxStamina;
    [SerializeField] float staminaDepleteRate;
    [SerializeField] float staminaRegenRate;
    [SerializeField] float staminaRegenDelay;
    [SerializeField] Image staminaBar;

    [Header("Dash Settings")]
    [SerializeField] int maxDashCount;
    [SerializeField] float dashSpeed;
    [SerializeField] float dashDuration;

    public AudioSource audioSource;


    // Private variables
    int jumpCount;
    float shootTimer;
    Vector3 moveDir;
    Vector3 playerVel;
    bool isSprinting;
    float currentStamina;
    float timeSinceLastSprint;
    int baseSpeed;
    float HPOrig;

    int gunListPos;

    private bool isInfiniteStamina = false;

    private bool isCrouching = false;

    private int currentDashCount;
    private bool isDashing = false;

    void Start()
    {
        HPOrig = HP;
        updatePlayerUI();
      
        baseSpeed = speed;
        currentStamina = maxStamina;
        if (staminaBar) staminaBar.fillAmount = 1f;

        currentDashCount = maxDashCount;
        gamemanager.instance.UpdateDashUI(currentDashCount, maxDashCount);
    }

    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.yellow);

        gamemanager.instance.updateAmmo(currentAmmo, maxAmmo);

        HandleStamina();
        UpdateStaminaUI();

        if (Input.GetButtonDown("Crouch"))
        {
            ToggleCrouch();
        }
        if (!gamemanager.instance.isPause)
        {
            movement();
            sprint();
        }


        if (Input.GetButtonDown("Dash") && !isDashing && currentDashCount > 0)
        {
            StartCoroutine(Dash());
        }

        isAiming = Input.GetButton("Fire2");

        HandleAiming();
    }



    void movement()
    {
        

        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVel = Vector3.zero;
        }

        moveDir = (Input.GetAxis("Horizontal") * transform.right) +
                  (Input.GetAxis("Vertical") * transform.forward);
        controller.Move(moveDir * speed * Time.deltaTime);

        jump();

        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

        shootTimer += Time.deltaTime;

        if (Input.GetButton("Fire1") && gunList.Count > 0 && gunList[gunListPos].ammoCurrent > 0 && shootTimer >= shootRate)
        {
            shoot();
        }
        SelectGun();
        GunReload();
    }

    void HandleAiming()
    {
        if (gunList.Count <= 0 || isReloading) return;

        GunStats currentGun = gunList[gunListPos];

        // Get the correct transform based on aiming state
        Transform targetTransform = isAiming ? currentGun.aimPos : currentGun.hipPos;

        // Smoothly move the gun to the target position
        gunModel.transform.localPosition = Vector3.Lerp(
            gunModel.transform.localPosition,
            targetTransform.localPosition,
            Time.deltaTime * currentGun.aimSpeed
        );

        // Smoothly rotate the gun to the target rotation
        gunModel.transform.localRotation = Quaternion.Slerp(
            gunModel.transform.localRotation,
            targetTransform.localRotation,
            Time.deltaTime * currentGun.aimSpeed
        );
    }


    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
        }
    }

    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVel.y = jumpSpeed;

            if (isCrouching)
            {
                controller.height = 2f;
                controller.center = Vector3.zero;
                isCrouching = false;
            }
        }
    }

    void shoot()
    {
        if (isReloading) return;

        shootTimer = 0;
        gunList[gunListPos].ammoCurrent--;
        StartCoroutine(ShootEffect());

        currentAmmo = gunList[gunListPos].ammoCurrent;
        gamemanager.instance.updateAmmo(currentAmmo, maxAmmo);

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
        {
            // Instantiate bullet impact effect
            ParticleSystem effect = Instantiate(gunList[gunListPos].hitEffect, hit.point, Quaternion.identity);
            Destroy(effect.gameObject, 1f);

            // Apply damage if enemy is hit
            IDamage dmg = hit.collider.GetComponent<IDamage>();
            if (dmg != null)
            {
                dmg.takeDamage(shootDamage);
            }
        }

        
        PlayMuzzleFlash();
        
    }
    public IEnumerator ShootEffect()
    {
        if (gunList[gunListPos] && gunList[gunListPos].shootSound != null)
        {
            audioSource.PlayOneShot(gunList[gunListPos].shootSound, gunList[gunListPos].shootVol);
        }
        yield return null; // Wait for one frame to continue
    }

    void PlayMuzzleFlash()
    {
        GunStats currentGun = gunList[gunListPos];

        // Choose the correct muzzle flash effect and position based on aiming state
        ParticleSystem muzzleFlashEffect = isAiming ? currentGun.AimMuzzleFlash : currentGun.HipMuzzleFlash;
        Transform muzzleFlashPos = isAiming ? currentGun.AimMuzzleFlashPos : currentGun.HipMuzzleFlashPos;

        if (muzzleFlashEffect != null && muzzleFlashPos != null)
        {
            // Instantiate muzzle flash at the correct position and rotation
            ParticleSystem muzzleFlashInstance = Instantiate(
                muzzleFlashEffect,
                muzzleFlashPos.position,
                muzzleFlashPos.rotation
            );

            // Play the muzzle flash effect
            muzzleFlashInstance.Play();

            // Destroy the muzzle flash instance after it finishes
            Destroy(muzzleFlashInstance.gameObject, 1f); // Adjust duration as needed
        }
    }



    void ToggleCrouch()
    {
        if (isCrouching)
        {
            // Stand up
            controller.height = 2f;
            controller.center = Vector3.zero;
        }
        else
        {
            // Crouch
            controller.height = 1f;
            controller.center = new Vector3(0, 0.5f, 0);
        }

        isCrouching = !isCrouching;
    }

    void HandleStamina()
    {

        if (isInfiniteStamina)
        {
            currentStamina = maxStamina;
            UpdateStaminaUI();
            return;
        }

        bool isMoving = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;
        bool trySprint = Input.GetButton("Sprint");

        if (trySprint && isMoving && currentStamina > 0 && !isCrouching)
        {
            if (!isSprinting)
            {
                isSprinting = true;
                speed = baseSpeed * sprintMod;
            }

            currentStamina = Mathf.Max(currentStamina - staminaDepleteRate * Time.deltaTime, 0);
            timeSinceLastSprint = 0;

            if (currentStamina <= 0)
            {
                EndSprint();
            }
        }
        else
        {
            if (isSprinting) EndSprint();

            timeSinceLastSprint += Time.deltaTime;
            if (timeSinceLastSprint >= staminaRegenDelay)
            {
                currentStamina = Mathf.Min(currentStamina + staminaRegenRate * Time.deltaTime, maxStamina);
            }
        }
    }

    void EndSprint()
    {
        isSprinting = false;
        speed = baseSpeed;
    }

    void UpdateStaminaUI()
    {
        if (staminaBar)
            staminaBar.fillAmount = currentStamina / maxStamina;
    }

    public void takeDamage(float amount)
    {
        HP -= amount;

        updatePlayerUI();
        StartCoroutine(flashDamageScreen());

        if (HP <= 0)
        {
            gamemanager.instance.youLose();
        }
    }

    IEnumerator flashDamageScreen()
    {
        gamemanager.instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gamemanager.instance.playerDamageScreen.SetActive(false);
    }

    void updatePlayerUI()
    {
        gamemanager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
    }

    public void ActivateInfiniteStamina(float duration)
    {
        StartCoroutine(InfiniteStaminaRoutine(duration));
    }

    private IEnumerator InfiniteStaminaRoutine(float duration)
    {
        isInfiniteStamina = true;
        currentStamina = maxStamina;
        UpdateStaminaUI();
        yield return new WaitForSeconds(duration);
        isInfiniteStamina = false;
    }

    IEnumerator Dash()
    {
        isDashing = true;
        currentDashCount--;
        gamemanager.instance.UpdateDashUI(currentDashCount, maxDashCount);

        float startTime = Time.time;
        Vector3 dashDirection = moveDir.normalized;

        while (Time.time < startTime + dashDuration)
        {
            controller.Move(dashDirection * dashSpeed * Time.deltaTime);
            yield return null;
        }

        isDashing = false;
    }

    public void RefillDash()
    {
        currentDashCount = maxDashCount;
        gamemanager.instance.UpdateDashUI(currentDashCount, maxDashCount);
    }

    void IPickUp.OnPickup(GameObject player)
    {
        playerController pc = player.GetComponent<playerController>();
        if (pc != null)
        {
            pc.RefillDash();
            Destroy(gameObject);
        }
    }



    public void GetGunStats(GunStats gun)
    {
        gunList.Add(gun);
        gunListPos = gunList.Count - 1;
        ChangeGun();

    }

    void SelectGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && gunListPos < gunList.Count - 1)
        {
            gunListPos++;
            ChangeGun();

        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && gunListPos > 0)
        {
            gunListPos--;
            ChangeGun();

        }

    }

    void ChangeGun()
    {
        if (gunList.Count == 0) return; // Prevent errors if there are no guns

        GunStats currentGun = gunList[gunListPos]; // Get the selected gun stats

        // Activate the gun model
        gunModel.SetActive(true);

        // Set gun stats
        shootDamage = currentGun.shootDamage;
        shootRate = currentGun.shootRate;
        shootDist = currentGun.shootDistance;

        // Update the mesh and material of the gun model
        gunModel.GetComponent<MeshFilter>().sharedMesh = currentGun.model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = currentGun.model.GetComponent<MeshRenderer>().sharedMaterial;

        // Set gun position and rotation based on the hip position
        gunModel.transform.localPosition = currentGun.hipPos.localPosition;
        gunModel.transform.localRotation = currentGun.hipPos.localRotation;

        // Set muzzle flash positions and effects for both hip-fire and aim
        hipMuzzleFlashPos = currentGun.HipMuzzleFlashPos;
        aimMuzzleFlashPos = currentGun.AimMuzzleFlashPos;
        hipMuzzleFlashEffect = currentGun.HipMuzzleFlash;
        aimMuzzleFlashEffect = currentGun.AimMuzzleFlash;

        // Update ammo count
        currentAmmo = currentGun.ammoCurrent;
        maxAmmo = currentGun.ammoMax;

        gamemanager.instance.updateAmmo(currentAmmo, maxAmmo);
    }

    void GunReload()
    {
        if (Input.GetButtonDown("Reload"))
        {
            StartCoroutine(ReloadGun());
        }
    }

    IEnumerator ReloadGun()
    {
        if (isReloading) yield break; // Prevents multiple reloads at once

        isReloading = true;

        // Play Reload Sound
        if (gunList[gunListPos].reloadSound != null)
        {
            audioSource.PlayOneShot(gunList[gunListPos].reloadSound, gunList[gunListPos].reloadVol);
        }

        // Store the original rotation
        Quaternion originalRotation = gunModel.transform.localRotation;
        Quaternion reloadRotation = Quaternion.Euler(originalRotation.eulerAngles.x - 30, originalRotation.eulerAngles.y, originalRotation.eulerAngles.z);

        float duration = 0.5f;
        float elapsed = 0;

        // Rotate backward
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            gunModel.transform.localRotation = Quaternion.Slerp(originalRotation, reloadRotation, elapsed / duration);
            yield return null;
        }

        // Optional wait at peak
        yield return new WaitForSeconds(0.1f);

        elapsed = 0;
        // Rotate forward to original position
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            gunModel.transform.localRotation = Quaternion.Slerp(reloadRotation, originalRotation, elapsed / duration);
            yield return null;
        }

        // Reset ammo
        gunList[gunListPos].ammoCurrent = gunList[gunListPos].ammoMax;

        gunList[gunListPos].ammoCurrent = gunList[gunListPos].ammoMax;
        currentAmmo = gunList[gunListPos].ammoCurrent;  // Update the `currentAmmo`
        gamemanager.instance.updateAmmo(currentAmmo, maxAmmo);

        // Finish reloading
        isReloading = false;
    }
}