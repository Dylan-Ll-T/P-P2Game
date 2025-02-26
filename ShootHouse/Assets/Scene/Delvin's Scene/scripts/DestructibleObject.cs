using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour, IDamage
{
    [Header("Health Settings")]
    public float health;

    [Header("Spawn Settings")]
    public GameObject[] spawnMaterials;
    public int spawnAmount;
    public float spawnHeightOffset = 1.5f; // Height offset for spawning above the ground

    [Header("Destruction Settings")]
    public GameObject destroyedEffect; // Particle effect on destruction

    public void takeDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            DestroyObject();
        }
    }

    private void DestroyObject()
    {
        // Spawn a destruction effect
        if (destroyedEffect)
        {
            Instantiate(destroyedEffect, transform.position, Quaternion.identity);
        }

        // Spawn materials above the object's position
        if (spawnMaterials.Length > 0)
        {
            for (int i = 0; i < spawnAmount; i++)
            {
                Vector3 spawnPosition = transform.position + new Vector3(0, spawnHeightOffset, 0);
                Instantiate(spawnMaterials[Random.Range(0, spawnMaterials.Length)], spawnPosition, Quaternion.identity);
            }
        }

        Destroy(gameObject);
    }
}