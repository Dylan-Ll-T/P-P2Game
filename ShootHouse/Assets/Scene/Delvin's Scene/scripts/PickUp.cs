using UnityEngine;

public class PickUp : MonoBehaviour
{

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
            pick.GetGunStats(gun);
            Destroy(gameObject);
        }
    }
}
