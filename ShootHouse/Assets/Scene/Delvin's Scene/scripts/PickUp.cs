using UnityEngine;

public class PickUp : MonoBehaviour
{
    [SerializeField] AudioSource pickup;
    [SerializeField] AudioClip pickupAud;
    [SerializeField] GunStats gun;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gun.ammoCurrent = gun.ammoMax;
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {

        IPickUp pick = other.GetComponent<IPickUp>();

        if (pick != null)
        {  
            pickup.PlayOneShot(pickupAud,.4f);
            pick.GetGunStats(gun);
            Destroy(gameObject);
          
        }
    }
}
