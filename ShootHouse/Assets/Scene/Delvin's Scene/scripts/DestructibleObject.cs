using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour, IDamage
{
    [Header("Health Settings")]
    public float health; 
    [Header("Spawn Settings")]
    public GameObject[] spawnMaterials;
    public int spawnAmount ;
    public Vector2 spawnForceRange = new Vector2(2f, 5f); // Random force range for spawning

    [Header("Destruction Settings")]
    public GameObject destroyedEffect; //particle effect on destruction
  

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

        // Spawn materials
        if (spawnMaterials.Length > 0)
        {
            for (int i = 0; i < spawnAmount; i++)
            {
                GameObject material = Instantiate(spawnMaterials[Random.Range(0, spawnMaterials.Length)],
                                                 transform.position + Random.insideUnitSphere * 0.5f,
                                                 Quaternion.identity);

                Rigidbody rb = material.GetComponent<Rigidbody>();
                if (rb)
                {
                    Vector3 randomForce = Random.insideUnitSphere * Random.Range(spawnForceRange.x, spawnForceRange.y);
                    rb.AddForce(randomForce, ForceMode.Impulse);
                }
            }
        }


        Destroy(gameObject);
    }
}
