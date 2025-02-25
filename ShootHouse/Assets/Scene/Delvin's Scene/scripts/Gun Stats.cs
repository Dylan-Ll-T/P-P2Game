using Unity.Hierarchy;
using UnityEngine;

[CreateAssetMenu]

public class GunStats : ScriptableObject
{
    public string gunName;
    public GameObject model;
    public int shootDamage;
    public int shootDistance;
    public float shootRate;
    public int ammoCurrent;
    public int ammoMax;

    public ParticleSystem hitEffect;
    public AudioClip shootSound;
    public float shootVol;
    public AudioClip reloadSound;
    public float reloadVol;

    public Transform hipPos;
    public Transform aimPos;

 
    public float aimSpeed = 10f;
}