using UnityEngine;

public interface IPickUp
{
    void OnPickup(GameObject player);

    public void GetGunStats(GunStats gun);

}