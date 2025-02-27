using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour, IDamage
{
    [Header("Health Settings")]
    public float health;

    [Header("Spawn Settings")]
    public GameObject[] spawnMaterials;
    public int spawnAmount;
    public float spawnHeightOffset = 1.5f;
    public float spawnRadius = 2f;

    [Header("Destruction Settings")]
    public GameObject destroyedEffect;


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
        if (destroyedEffect)
        {
            Instantiate(destroyedEffect, transform.position, Quaternion.identity);
        }

        SpawnItemsOnGround(); // Ensure items are spawned before destroying the object

        Destroy(gameObject);
    }

    private void SpawnItemsOnGround()
    {
        if (spawnMaterials == null || spawnMaterials.Length == 0)
        {
            Debug.LogWarning("No spawn materials assigned.");
            return;
        }

        for (int i = 0; i < spawnAmount; i++)
        {
            Vector3 randomPosition = transform.position + new Vector3(
                Random.Range(-spawnRadius, spawnRadius),
                spawnHeightOffset,
                Random.Range(-spawnRadius, spawnRadius)
            );

            // Ensure valid object selection
            GameObject prefab = spawnMaterials[Random.Range(0, spawnMaterials.Length)];
            if (prefab == null) continue; // Skip null objects

            GameObject spawnedObject = Instantiate(prefab, randomPosition, Quaternion.identity);

            Collider col = spawnedObject.GetComponent<Collider>();
            if (col == null)
            {
                spawnedObject.AddComponent<BoxCollider>(); // Add a collider if it's missing
            }
        }
    }
}