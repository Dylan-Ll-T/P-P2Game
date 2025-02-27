using UnityEngine;

public class HealthPack : MonoBehaviour, IPickUp
{
    [SerializeField] int healAmount;

    public void OnPickup(GameObject player)
    {
        playerController pc = player.GetComponent<playerController>();
        if (pc != null)
        {
            pc.Heal(healAmount); 
            Destroy(gameObject); 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnPickup(other.gameObject);
        }
    }
    public void GetGunStats(GunStats gun)
    {
        throw new System.NotImplementedException();
    }
}