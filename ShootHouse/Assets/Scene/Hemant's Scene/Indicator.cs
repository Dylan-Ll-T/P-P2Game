using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Damageindicator : MonoBehaviour
{
    public Transform PlaneSize;
    public Transform Player;
    public Transform DamageImagePivot;

    GameObject Enemy;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Enemy = FindClosestEnemy();
        if (Enemy != null)
        {
            Vector3 Direction = (Enemy.transform.position - Player.position);
            float angle = (Vector3.SignedAngle(Direction, Player.forward, Vector3.up));
            DamageImagePivot.transform.localEulerAngles = new Vector3(0, 0, angle);
        }
        { }
    }

    public GameObject FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest = null;
        float minDistance = (PlaneSize.localScale.x * PlaneSize.localScale.y);
        float dist;

        for (int i = 0; i < enemies.Length; i++)
        {
            dist = Vector3.Distance(Player.position, enemies[i].transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = enemies[i];
            }
        }

        return closest;
    }
}
