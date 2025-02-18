using UnityEngine;

public class DashRefill : MonoBehaviour, IPickUp
{
    public void OnPickup(GameObject player)
    {
        playerController pc = player.GetComponent<playerController>();
        if (pc != null)
        {
            pc.RefillDash();
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