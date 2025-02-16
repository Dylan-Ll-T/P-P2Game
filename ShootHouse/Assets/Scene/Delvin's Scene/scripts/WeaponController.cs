using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Settings")]
    public GameObject pistol;
    public GameObject rifle;
    public GameObject shotgun;

    public GameObject currentWeapon;
    public int currentAmmo;
    public int CurrentMaxAmmo;
    public float fireRate;
    public int weaponDamage;
    public ParticleSystem muzzleFlash;

    [Header("Ammo Settings")]
    public int pistolAmmo;
    public int rifleAmmo;
    public int shotgunAmmo;
    public int pistolMaxAmmo;
    public int rifleMaxAmmo;
    public int shotgunMaxAmmo;

    [Header("Damage Settings")]
    public int pistolDamage;
    public int rifleDamage;
    public int shotgunDamage;

    [Header("Fire Rate Settings")]
    public float pistolFireRate;
    public float rifleFireRate;
    public float shotgunFireRate;

    [Header("Positions")]
    public Transform pistolHipPos;
    public Transform pistolAimPos;
    public Transform rifleHipPos;
    public Transform rifleAimPos;
    public Transform shotgunHipPos;
    public Transform shotgunAimPos;
    public Transform currentHipPos;
    public Transform currentAimPos;

    [Header("Effects")]
    public ParticleSystem pistolMuzzleFlash;
    public ParticleSystem rifleMuzzleFlash;
    public ParticleSystem shotgunMuzzleFlash;

    [Header("Sound Effects")]
    public AudioSource audioSource; 
    public AudioClip pistolShot;
    public AudioClip rifleShot;
    public AudioClip shotgunShot;
    public AudioSource reloadSound;
    public AudioClip reload;

    public float shootTimer;
    public bool isReloading = false;
    public Vector3 originalRotation;

    void Start()
    {
        EquipWeapon(pistol);
        gamemanager.instance.updateAmmo(currentAmmo, CurrentMaxAmmo);
    }

    void Update()
    {
        HandleWeaponSwitching();
        HandleAiming();
        HandleShooting();
        HandleReloading();
    }

    // -------- WEAPON SWITCHING --------
    public void HandleWeaponSwitching()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipWeapon(pistol);
        if (Input.GetKeyDown(KeyCode.Alpha2)) EquipWeapon(rifle);
        if (Input.GetKeyDown(KeyCode.Alpha3)) EquipWeapon(shotgun);
    }

    public void EquipWeapon(GameObject weapon)
    {
        // Store the ammo of the current weapon before switching
        if (currentWeapon == pistol) pistolAmmo = currentAmmo;
        else if (currentWeapon == rifle) rifleAmmo = currentAmmo;
        else if (currentWeapon == shotgun) shotgunAmmo = currentAmmo;

        // Deactivate all weapons
        pistol.SetActive(false);
        rifle.SetActive(false);
        shotgun.SetActive(false);

        // Activate the selected weapon
        currentWeapon = weapon;
        currentWeapon.SetActive(true);

        // Restore the correct ammo count when switching
        if (weapon == pistol)
        {
            currentAmmo = pistolAmmo;
            CurrentMaxAmmo = pistolMaxAmmo;
            fireRate = pistolFireRate;
            weaponDamage = pistolDamage;
            muzzleFlash = pistolMuzzleFlash;
        }
        else if (weapon == rifle)
        {
            currentAmmo = rifleAmmo;
            CurrentMaxAmmo = rifleMaxAmmo;
            fireRate = rifleFireRate;
            weaponDamage = rifleDamage;
            muzzleFlash = rifleMuzzleFlash;
        }
        else if (weapon == shotgun)
        {
            currentAmmo = shotgunAmmo;
            CurrentMaxAmmo = shotgunMaxAmmo;
            fireRate = shotgunFireRate;
            weaponDamage = shotgunDamage;
            muzzleFlash = shotgunMuzzleFlash;
        }

        gamemanager.instance.updateAmmo(currentAmmo, CurrentMaxAmmo); // Update UI
        originalRotation = currentWeapon.transform.localEulerAngles;
    }


    // -------- AIMING --------
    public void HandleAiming()
    {
        if (isReloading) return;

        float aimSpeed = 20f; 

        if (currentWeapon == pistol)
        {
            currentHipPos = pistolHipPos.transform;
            currentAimPos = pistolAimPos.transform;
        }
        else if (currentWeapon == rifle)
        {
            currentHipPos = rifleHipPos.transform;
            currentAimPos = rifleAimPos.transform;
        }
        else if (currentWeapon == shotgun)
        {
            currentHipPos = shotgunHipPos.transform;
            currentAimPos = shotgunAimPos.transform;
        }

        if (Input.GetKey(KeyCode.Mouse1)) 
        {
            currentWeapon.transform.position = Vector3.MoveTowards(
                currentWeapon.transform.position,
                currentAimPos.position,
                Time.deltaTime * aimSpeed
            );

            currentWeapon.transform.rotation = Quaternion.Slerp(
                currentWeapon.transform.rotation,
                currentAimPos.rotation,
                Time.deltaTime * aimSpeed
            );
        }
        else
        {
            currentWeapon.transform.position = Vector3.MoveTowards(
                currentWeapon.transform.position,
                currentHipPos.position,
                Time.deltaTime * aimSpeed
            );

            currentWeapon.transform.rotation = Quaternion.Slerp(
                currentWeapon.transform.rotation,
                currentHipPos.rotation,
                Time.deltaTime * aimSpeed
            );
        }
    }


    // -------- SHOOTING --------
    public void HandleShooting()
    {
        shootTimer += Time.deltaTime;
        if (Input.GetButton("Fire1") && shootTimer >= fireRate && currentAmmo > 0 && !isReloading)
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        if (currentAmmo > 0)
        {
            shootTimer = 0;
            currentAmmo--;
            muzzleFlash.Play();
            StartCoroutine(ShootEffect());

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 100))
            {
                if (hit.collider.CompareTag("Player")) return;

                IDamage dmg = hit.collider.GetComponent<IDamage>();
                if (dmg != null)
                {
                    dmg.takeDamage(weaponDamage);
                }
            }

            gamemanager.instance.updateAmmo(currentAmmo, CurrentMaxAmmo); // Update UI
        }
    }

    public IEnumerator ShootEffect()
    {
        if (currentWeapon == pistol && pistolShot != null)
        {
            audioSource.PlayOneShot(pistolShot);
        }
        else if (currentWeapon == rifle && rifleShot != null)
        {
            audioSource.PlayOneShot(rifleShot);
        }
        else if (currentWeapon == shotgun && shotgunShot != null)
        {
            audioSource.PlayOneShot(shotgunShot);
        }

        if (currentWeapon == pistol)
        {
            pistolMuzzleFlash.Play();
        }
        else if (currentWeapon == rifle)
        {
            rifleMuzzleFlash.Play();
        }
        else if (currentWeapon == shotgun)
        {
            shotgunMuzzleFlash.Play();
        }

        yield return null; // Wait for one frame to continue
    }

    // -------- RELOADING --------
    public void HandleReloading()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
          
            StartCoroutine(Reload());
        }
    }

    public IEnumerator Reload()
    {
        isReloading = true;
        float reloadTime = 1f;
       
        reloadSound.Play();

        // Store original rotation properly
        Quaternion startRotation = currentWeapon.transform.localRotation;
        Quaternion reloadRotation = startRotation * Quaternion.Euler(-30, 0, 0); // Tilt weapon down

        float elapsedTime = 0;

        // Rotate down
        while (elapsedTime < reloadTime / 2)
        {
            elapsedTime += Time.deltaTime;
            currentWeapon.transform.localRotation = Quaternion.Slerp(
                startRotation, reloadRotation, elapsedTime / (reloadTime / 2)
            );
            yield return null;
        }

        yield return new WaitForSeconds(reloadTime / 2); // Pause at bottom position

        elapsedTime = 0;

        // Rotate back up
        while (elapsedTime < reloadTime / 2)
        {
            elapsedTime += Time.deltaTime;
            currentWeapon.transform.localRotation = Quaternion.Slerp(
                reloadRotation, startRotation, elapsedTime / (reloadTime / 2)
            );
            yield return null;
        }

        // Fix: Ensure exact reset
        currentWeapon.transform.localRotation = startRotation;

        if (currentWeapon == pistol) currentAmmo = pistolMaxAmmo;
        else if (currentWeapon == rifle) currentAmmo = rifleMaxAmmo;
        else if (currentWeapon == shotgun) currentAmmo = shotgunMaxAmmo;

        CurrentMaxAmmo = currentAmmo; // Ensure max ammo is updated
        gamemanager.instance.updateAmmo(currentAmmo, CurrentMaxAmmo); // Update UI

        isReloading = false;
    }
}


