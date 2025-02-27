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
   
        SpawnItemsOnGround();
        Destroy(gameObject);
    }

    private void SpawnItemsOnGround()
    {
        for (int i = 0; i < spawnAmount; i++)
        {
           
            Vector3 randomPosition = transform.position + new Vector3(
                Random.Range(-spawnRadius, spawnRadius), 
                spawnHeightOffset,
                Random.Range(-spawnRadius, spawnRadius) 
            );

           
            GameObject spawnedObject = Instantiate(
                spawnMaterials[Random.Range(0, spawnMaterials.Length)],
                randomPosition,
                Quaternion.identity);

            // Ensure the spawned object has a collider
            Collider col = spawnedObject.GetComponent<Collider>();
            if (col == null)
            {
                spawnedObject.AddComponent<BoxCollider>(); // Add a collider if it's missing
            }
        }
    }
}