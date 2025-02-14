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
    public int magazineSize;
    public float fireRate;
    public int weaponDamage;
    public float recoilAmount;
    public ParticleSystem muzzleFlash;

    [Header("Ammo Settings")]
    public int pistolAmmo;
    public int rifleAmmo;
    public int shotgunAmmo;

    [Header("Damage Settings")]
    public int pistolDamage;
    public int rifleDamage;
    public int shotgunDamage;

    [Header("Fire Rate Settings")]
    public float pistolFireRate;
    public float rifleFireRate;
    public float shotgunFireRate;

    [Header("Positions")]
    public Transform hipPos;
    public Transform aimPos;

    [Header("Effects")]
    public GameObject shootSound;
    public ParticleSystem pistolMuzzleFlash;
    public ParticleSystem rifleMuzzleFlash;
    public ParticleSystem shotgunMuzzleFlash;

    public float shootTimer;
    public bool isReloading = false;
    public Vector3 originalRotation;

    void Start()
    {
        EquipWeapon(pistol);
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
        pistol.SetActive(false);
        rifle.SetActive(false);
        shotgun.SetActive(false);

        currentWeapon = weapon;
        currentWeapon.SetActive(true);

        if (weapon == pistol)
        {
            currentAmmo = pistolAmmo;
            magazineSize = pistolAmmo;
            fireRate = pistolFireRate;
            weaponDamage = pistolDamage;
            recoilAmount = 0.05f;
            muzzleFlash = pistolMuzzleFlash;
        }
        else if (weapon == rifle)
        {
            currentAmmo = rifleAmmo;
            magazineSize = rifleAmmo;
            fireRate = rifleFireRate;
            weaponDamage = rifleDamage;
            recoilAmount = 0.1f;
            muzzleFlash = rifleMuzzleFlash;
        }
        else if (weapon == shotgun)
        {
            currentAmmo = shotgunAmmo;
            magazineSize = shotgunAmmo;
            fireRate = shotgunFireRate;
            weaponDamage = shotgunDamage;
            recoilAmount = 0.2f;
            muzzleFlash = shotgunMuzzleFlash;
        }

        originalRotation = currentWeapon.transform.localEulerAngles;
    }

    // -------- AIMING --------
    public void HandleAiming()
    {
        float aimSpeed = 10f;

        if (Input.GetKey(KeyCode.Mouse1))
        {
            currentWeapon.transform.position = Vector3.Lerp(
                currentWeapon.transform.position,
                aimPos.position,
                aimSpeed * Time.deltaTime
            );
        }
        else
        {
            currentWeapon.transform.position = Vector3.Lerp(
                currentWeapon.transform.position,
                hipPos.position,
                aimSpeed * Time.deltaTime
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
        shootTimer = 0;
        currentAmmo--;
        muzzleFlash.Play();
        StartCoroutine(ShootEffect());

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 100))
        {
            IDamage dmg = hit.collider.GetComponent<IDamage>();
            if (dmg != null)
            {
                dmg.takeDamage(weaponDamage);
            }
        }

        StartCoroutine(ApplyRecoil());
    }

    public IEnumerator ApplyRecoil()
    {
        Vector3 recoilRotation = originalRotation + new Vector3(-recoilAmount, 0, 0);
        float recoilTime = 0.1f;

        float elapsedTime = 0;
        while (elapsedTime < recoilTime)
        {
            currentWeapon.transform.localEulerAngles = Vector3.Lerp(
                originalRotation, recoilRotation, elapsedTime / recoilTime
            );
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0;
        while (elapsedTime < recoilTime)
        {
            currentWeapon.transform.localEulerAngles = Vector3.Lerp(
                recoilRotation, originalRotation, elapsedTime / recoilTime
            );
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator ShootEffect()
    {
        shootSound.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        shootSound.SetActive(false);
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
        float reloadTime = 2.0f;

        Vector3 reloadRotation = originalRotation + new Vector3(-30, 0, 0);
        float elapsedTime = 0;

        while (elapsedTime < reloadTime / 2)
        {
            currentWeapon.transform.localEulerAngles = Vector3.Lerp(
                originalRotation, reloadRotation, elapsedTime / (reloadTime / 2)
            );
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(reloadTime / 2);

        elapsedTime = 0;
        while (elapsedTime < reloadTime / 2)
        {
            currentWeapon.transform.localEulerAngles = Vector3.Lerp(
                reloadRotation, originalRotation, elapsedTime / (reloadTime / 2)
            );
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        currentAmmo = magazineSize;
        isReloading = false;
    }
}

