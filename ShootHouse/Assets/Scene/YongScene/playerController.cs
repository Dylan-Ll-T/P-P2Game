using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerController : MonoBehaviour, IDamage
{
    [Header("Movement Settings")]
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] int HP;
    [SerializeField] int speed;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpSpeed;
    [SerializeField] int jumpMax;
    [SerializeField] int gravity;

    [Header("Aiming Settings")]
    [SerializeField] float aimSpeed;

    [Header("Shooting Settings")]
    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;

    [Header("Weapon Settings")]
    [SerializeField] GameObject gun;
    [SerializeField] GameObject hipPos;
    [SerializeField] GameObject aimPos;

    [Header("Stamina Settings")]
    [SerializeField] float maxStamina;
    [SerializeField] float staminaDepleteRate;
    [SerializeField] float staminaRegenRate;
    [SerializeField] float staminaRegenDelay;
    [SerializeField] Image staminaBar;

    // Private variables
    int jumpCount;
    float shootTimer;
    Vector3 moveDir;
    Vector3 playerVel;
    bool isSprinting;
    float currentStamina;
    float timeSinceLastSprint;
    int baseSpeed;
    int HPOrig;

    private Vector3 crouchScale = new Vector3(0.4166666f, 0.2083333f, 0.4166666f);
    private Vector3 playerScale = new Vector3(0.4166666f, 0.4166666f, 0.4166666f);
    private bool isCrouching = false;

    void Start()
    {
        HPOrig = HP;
        updatePlayerUI();

        baseSpeed = speed;
        currentStamina = maxStamina;
        if (staminaBar) staminaBar.fillAmount = 1f;
    }

    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.yellow);

        movement();
        HandleStamina();
        UpdateStaminaUI();

        if (Input.GetButtonDown("Crouch"))
        {
            ToggleCrouch();
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            gun.transform.position = Vector3.Lerp(gun.transform.position, aimPos.transform.position, aimSpeed);
        }
        else
        {
            gun.transform.position = Vector3.Lerp(gun.transform.position, hipPos.transform.position, aimSpeed);
        }
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

        if (Input.GetButton("Fire1") && shootTimer >= shootRate)
        {
            shoot();
        }
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
                transform.localScale = playerScale;
                transform.position = new Vector3(transform.position.x, transform.position.y + 0.2083333f, transform.position.z);
            }
            isCrouching = !isCrouching;
        }
    }

    void shoot()
    {
        shootTimer = 0;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
        {
            Debug.Log(hit.collider.name);

            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.takeDamage(shootDamage);
            }
        }
    }

    void ToggleCrouch()
    {
        if (isCrouching)
        {
            transform.localScale = playerScale;
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.2083333f, transform.position.z);
        }
        else
        {
            transform.localScale = crouchScale;
            transform.position = new Vector3(transform.position.x, transform.position.y - 0.2083333f, transform.position.z);
        }

        isCrouching = !isCrouching;
    }

    void HandleStamina()
    {
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

    public void takeDamage(int amount)
    {
        HP -= amount;
        updatePlayerUI();

        if (HP <= 0)
        {
            gamemanager.instance.youLose();
        }
    }

    void updatePlayerUI()
    {
        gamemanager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
    }
}