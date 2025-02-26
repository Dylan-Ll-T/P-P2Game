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

    //Delvin's Addition//
    // Muzzle Flash Quad (GameObjects)
    [Header("Muzzle Flash")]
    private GameObject hipMuzzleFlashQuad;
    private GameObject aimMuzzleFlashQuad;

    public GameObject pistolHipMuzzleFlashQuad;
    public GameObject pistolAimMuzzleFlashQuad;
    public GameObject rifleHipMuzzleFlashQuad;
    public GameObject rifleAimMuzzleFlashQuad;
    public GameObject shotgunHipMuzzleFlashQuad;
    public GameObject shotgunAimMuzzleFlashQuad;
    private GameObject muzzleFlashQuad;

    // Muzzle Flash Positions (Transforms)
    private Transform hipMuzzleFlashPos;
    private Transform aimMuzzleFlashPos;


    private int currentAmmo;
    private int maxAmmo;
   
    private bool isAiming = false;
    public bool isReloading = false;
    public Vector3 originalRotation;

    [Header("Shooting Settings")]
    [SerializeField] float shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;
    [SerializeField] int headshotMult; // Dylan's Addition
    [SerializeField] GameObject gunModel;
    [SerializeField] List<GunStats> gunList = new List<GunStats>();
    [SerializeField] private Transform muzzleFlashPos;
    private ParticleSystem muzzleFlashEffect;
    //End of Delvin's Addition//

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

    [Header("Audio Settings")]
    [SerializeField] AudioSource aud;
    [Range(0, 1)][SerializeField] AudioClip[] audSteps;
    [Range(0, 1)][SerializeField] float audStepsVol;
    [Range(0, 1)][SerializeField] AudioClip[] audHurt;
    [Range(0, 1)][SerializeField] float audHurtVol;
    [Range(0, 1)][SerializeField] AudioClip[] audJump;
    [Range(0, 1)][SerializeField] float audJumpVol;
    [SerializeField] AudioClip headShot;


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
    private float originalHeight;
    private Vector3 originalCenter;

    private int currentDashCount;
    private bool isDashing = false;
    bool isPlayerSteps;

    void Start()
    {
        HPOrig = HP;
        updatePlayerUI();
      
        baseSpeed = speed;
        currentStamina = maxStamina;
        if (staminaBar) staminaBar.fillAmount = 1f;

        currentDashCount = maxDashCount;
        gamemanager.instance.UpdateDashUI(currentDashCount, maxDashCount);


        originalHeight = controller.height;
        originalCenter = controller.center;
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
            if (moveDir.magnitude > .3f && !isPlayerSteps)
            {
                StartCoroutine(playSteps());
            }
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

        //Delvin's Addition//
        if (Input.GetButton("Fire1") && gunList.Count > 0 && gunList[gunListPos].ammoCurrent > 0 && shootTimer >= shootRate)
        {
            shoot();
        }
        SelectGun();
        GunReload();
    }

    void HandleAiming()
    {
        if (gunList.Count <= 0 || isReloading)
            return;

        GunStats currentGun = gunList[gunListPos];

        Transform targetTransform;
        if (isAiming)
        {
            targetTransform = currentGun.aimPos;
        }
        else
        {
            targetTransform = currentGun.hipPos;
        }

        gunModel.transform.localPosition = Vector3.Lerp(
            gunModel.transform.localPosition,
            targetTransform.localPosition,
            Time.deltaTime * currentGun.aimSpeed
        );

        gunModel.transform.localRotation = Quaternion.Slerp(
            gunModel.transform.localRotation,
            targetTransform.localRotation,
            Time.deltaTime * currentGun.aimSpeed
        );
    }
    //End of Delvin's Addition//

    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
            isPlayerSteps = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
            isPlayerSteps = false;
        }
    }
    IEnumerator playSteps()
    {
        isPlayerSteps = true;
        aud.PlayOneShot(audSteps[Random.Range(0, audSteps.Length)], audStepsVol);

        if (!isSprinting)
        {
            yield return new WaitForSeconds(.5f);

        }
        else
            yield return new WaitForSeconds(0.3f);
        isPlayerSteps = false;
    }
    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVel.y = jumpSpeed;
            aud.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVol);

            if (isCrouching)
            {
                controller.height = originalHeight;
                controller.center = originalCenter;
                isCrouching = false;
            }
        }
    }

    //Delvin's Addition//
    void shoot()
    {
        if (isReloading) return;

        shootTimer = 0;
        gunList[gunListPos].ammoCurrent--;
        currentAmmo = gunList[gunListPos].ammoCurrent;
        gamemanager.instance.updateAmmo(currentAmmo, maxAmmo);


        StartCoroutine(ShootEffect());

        currentAmmo = gunList[gunListPos].ammoCurrent;
        gamemanager.instance.updateAmmo(currentAmmo, maxAmmo);

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
        {
            //Dylan's Additions
            float endDamage = shootDamage;
            bool headshot = false;
            if (hit.collider.CompareTag("EnemyHead"))
            {
                endDamage *= headshotMult;
                headshot = true;
                aud.PlayOneShot(headShot, .2f);
            }
            //End of Dylan's Additions

            ParticleSystem effect = Instantiate(gunList[gunListPos].hitEffect, hit.point, Quaternion.identity);
            Destroy(effect.gameObject, 1f);

            //Edited by Dylan
            if (headshot)
            {
                IDamage dmg = hit.collider.GetComponentInParent<IDamage>();

                if (dmg != null)
                {
                    dmg.takeDamage(endDamage);
                }
            }
            else
            {
                IDamage dmg = hit.collider.GetComponent<IDamage>();

                if (dmg != null)
                {
                    dmg.takeDamage(endDamage);
                }
            }
            //End of Dylan's Edit

        }

        PlayMuzzleFlash(); 
    }
    public IEnumerator ShootEffect()
    {
        if (gunList[gunListPos] && gunList[gunListPos].shootSound != null)
        {
            aud.PlayOneShot(gunList[gunListPos].shootSound, gunList[gunListPos].shootVol);
        }
        yield return null;
    }

    void PlayMuzzleFlash()
    {
        if (gunList.Count == 0) return;

        GunStats currentGun = gunList[gunListPos];

        GameObject muzzleFlashQuad = null;

        if (currentGun.gunName == "Pistol")
        {
            if (isAiming)
            {
                muzzleFlashQuad = pistolAimMuzzleFlashQuad;
                muzzleFlashQuad.transform.localPosition = pistolAimMuzzleFlashQuad.transform.localPosition;
            }
            else
            {
                muzzleFlashQuad = pistolHipMuzzleFlashQuad;
                muzzleFlashQuad.transform.localPosition = pistolHipMuzzleFlashQuad.transform.localPosition;
            }
        }
        else if (currentGun.gunName == "Rifle")
        {
            if (isAiming)
            {
                muzzleFlashQuad = rifleAimMuzzleFlashQuad;
                muzzleFlashQuad.transform.localPosition = rifleAimMuzzleFlashQuad.transform.localPosition;
            }
            else
            {
                muzzleFlashQuad = rifleHipMuzzleFlashQuad;
                muzzleFlashQuad.transform.localPosition = rifleHipMuzzleFlashQuad.transform.localPosition;
            }
        }
        else if (currentGun.gunName == "Shotgun")
        {
            if (isAiming)
            {
                muzzleFlashQuad = shotgunAimMuzzleFlashQuad;              
                muzzleFlashQuad.transform.localPosition = shotgunAimMuzzleFlashQuad.transform.localPosition;
            }
            else
            {
                muzzleFlashQuad = shotgunHipMuzzleFlashQuad;           
                muzzleFlashQuad.transform.localPosition = shotgunHipMuzzleFlashQuad.transform.localPosition;
            }
        }

        
        if (muzzleFlashQuad != null)
        {
            
            StartCoroutine(ShowMuzzleFlash(muzzleFlashQuad));

            //Debug.Log("Muzzle Flash Quad: " + muzzleFlashQuad);
        }
    }

    // Coroutine to enable and disable muzzle flash
    IEnumerator ShowMuzzleFlash(GameObject muzzleFlashQuad)
    {
        muzzleFlashQuad.SetActive(true);  
        yield return new WaitForSeconds(0.02f);  
        muzzleFlashQuad.SetActive(false);  
    }

    //End of Delvin's Addition//
    void ToggleCrouch()
    {
        if (isCrouching)
        {
            // Stand up
            controller.height = originalHeight;
            controller.center = originalCenter;
        }
        else
        {
            // Crouch
            controller.height = originalHeight / 2f;
            controller.center = new Vector3(originalCenter.x, originalCenter.y * 0.5f, originalCenter.z);
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
        aud.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);

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

    //Delvin's Addition//
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
        if (gunList.Count == 0) return; 

        GunStats currentGun = gunList[gunListPos]; 

        // Activate the gun model
        if (gunModel != null)
        {
            gunModel.SetActive(true);

            MeshFilter meshFilter = gunModel.GetComponent<MeshFilter>();
            MeshRenderer meshRenderer = gunModel.GetComponent<MeshRenderer>();

            if (meshFilter != null && currentGun.model != null)
                meshFilter.sharedMesh = currentGun.model.GetComponent<MeshFilter>().sharedMesh;

            if (meshRenderer != null && currentGun.model != null)
                meshRenderer.sharedMaterial = currentGun.model.GetComponent<MeshRenderer>().sharedMaterial;

        
            gunModel.transform.localPosition = currentGun.hipPos.localPosition;
            gunModel.transform.localRotation = currentGun.hipPos.localRotation;
        }

      
        shootDamage = currentGun.shootDamage;
        shootRate = currentGun.shootRate;
        shootDist = currentGun.shootDistance;

      
        if (currentGun.gunName == "Pistol")
        {
            hipMuzzleFlashQuad = pistolHipMuzzleFlashQuad;
            aimMuzzleFlashQuad = pistolAimMuzzleFlashQuad;
        }
        else if (currentGun.gunName == "Rifle")
        {
            hipMuzzleFlashQuad = rifleHipMuzzleFlashQuad;
            aimMuzzleFlashQuad = rifleAimMuzzleFlashQuad;
        }
        else if (currentGun.gunName == "Shotgun")
        {
            hipMuzzleFlashQuad = shotgunHipMuzzleFlashQuad;
            aimMuzzleFlashQuad = shotgunAimMuzzleFlashQuad;
        }

     
        if (hipMuzzleFlashQuad) hipMuzzleFlashQuad.SetActive(false);
        if (aimMuzzleFlashQuad) aimMuzzleFlashQuad.SetActive(false);

        if (currentGun.ammoCurrent >= 0)
        {
            currentAmmo = currentGun.ammoCurrent;
        }
      

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
        if (isReloading) yield break;

        // Ensure gunListPos is valid at the beginning of the method
        if (gunListPos < 0 || gunListPos >= gunList.Count)
        {
            Debug.LogError("Invalid gunListPos index: " + gunListPos);
            yield break; // Exit the method if the index is invalid
        }

        isReloading = true;

        if (gunList[gunListPos].reloadSound != null)
        {
            aud.PlayOneShot(gunList[gunListPos].reloadSound, gunList[gunListPos].reloadVol);
        }

        Quaternion originalRotation = gunModel.transform.localRotation;
        Quaternion reloadRotation = Quaternion.Euler(originalRotation.eulerAngles.x - 30, originalRotation.eulerAngles.y, originalRotation.eulerAngles.z);

        float duration = 0.5f;
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            gunModel.transform.localRotation = Quaternion.Slerp(originalRotation, reloadRotation, elapsed / duration);
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            gunModel.transform.localRotation = Quaternion.Slerp(reloadRotation, originalRotation, elapsed / duration);
            yield return null;
        }

        gunList[gunListPos].ammoCurrent = gunList[gunListPos].ammoMax;
        currentAmmo = gunList[gunListPos].ammoCurrent;
        gamemanager.instance.updateAmmo(currentAmmo, maxAmmo);

        isReloading = false;
    }
}