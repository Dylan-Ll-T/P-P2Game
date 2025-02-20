using Unity.Hierarchy;
using UnityEngine;

[CreateAssetMenu]

public class GunStats : ScriptableObject
{
    public GameObject model;
    public int shootDamage;
    public int shootDistance;
    public float shootRate;
    public int ammoCurrent;
    public int ammoMax;

    public ParticleSystem hitEffect;
    public ParticleSystem AimMuzzleFlash;
    public ParticleSystem HipMuzzleFlash;
    
    public AudioClip shootSound;
    public float shootVol;
    public Transform AimMuzzleFlashPos;
    public Transform HipMuzzleFlashPos;
    public AudioClip reloadSound;
    public float reloadVol;

    public Transform hipPos;
    public Transform aimPos;
    public float aimSpeed = 10f;


}
