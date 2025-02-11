using UnityEngine;

public class damage : MonoBehaviour
{
    enum damagetype { moving, stationary}
    [SerializeField] Rigidbody rb;
    [SerializeField] damagetype type;
    [SerializeField] int damageamount;
    [SerializeField] int speed;
    [SerializeField] int destroytime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(type == damagetype.moving)
        {
            rb.linearVelocity = transform.forward * speed;
            Destroy(gameObject, destroytime);
        }
    }
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        IDamage dmg = other.GetComponent<IDamage>();
        if (dmg != null)
        {
            dmg.takeDamage(damageamount);
        }
        if(type == damagetype.moving)
        {
            Destroy(gameObject);
        }

    }
}
