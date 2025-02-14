using UnityEngine;

public class StaminaBoost : MonoBehaviour, IPickUp
{
    [SerializeField] float duration;

    public void OnPickup(GameObject player)
    {
        playerController pc = player.GetComponent<playerController>();
        if (pc != null)
        {
            pc.ActivateInfiniteStamina(duration);
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