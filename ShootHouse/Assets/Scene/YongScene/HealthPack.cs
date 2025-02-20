using UnityEngine;

public class HealthPack : MonoBehaviour, IPickUp
{
    [SerializeField] int healAmount;

    public void OnPickup(GameObject player)
    {
        IDamage damageable = player.GetComponent<IDamage>();
        if (damageable != null)
        {
            damageable.takeDamage(-healAmount);
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
}