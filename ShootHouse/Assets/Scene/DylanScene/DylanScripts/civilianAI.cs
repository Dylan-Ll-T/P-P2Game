using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class civilianAI : MonoBehaviour
{
    [Header("--Basics--")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;

    [SerializeField] int enemyHealth;

    [Header("--Roaming--")]
    [SerializeField] int roamPauseTime;
    [SerializeField] int roamDist;

    Color colorOrig;

    float roamTimer;
    float stoppingDistOrig;

    Vector3 startingPos;

    // Putting this here just in case we want the AI to roam when the player enters the room.
    bool playerInRange;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = model.material.color;
        //This would need to be changed.
        //gamemanager.instance.updateGameGoal(1);
        startingPos = transform.position;
        stoppingDistOrig = agent.stoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        roam();
    }
    void roam()
    {
        roamTimer = 0;
        agent.stoppingDistance = 0;

        Vector3 ranPos = Random.insideUnitSphere * roamDist;
        ranPos += startingPos;

        NavMeshHit hit;
        NavMesh.SamplePosition(ranPos, out hit, roamDist, 1);
        agent.SetDestination(hit.position);
    }

    // These will only be used if trying to have the AI only work when player is nearby.
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        playerInRange = true;
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        playerInRange = false;
    //        agent.stoppingDistance = 0;
    //    }
    //}

    public void takeDamage(int amount)
    {
        enemyHealth -= amount;
        StartCoroutine(flashRed());
        agent.SetDestination(gamemanager.instance.player.transform.position);
        if (enemyHealth <= 0)
        {
            gamemanager.instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }
}
