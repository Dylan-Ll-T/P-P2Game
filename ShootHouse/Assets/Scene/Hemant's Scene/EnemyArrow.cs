using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyArrow : MonoBehaviour
{
    public Transform PlaneSize;
    public CanvasGroup IsVisable;
    public float distance;
    public Image AllertArrow;

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
            distance = Vector3.Distance(gamemanager.instance.player.transform.position, Enemy.transform.position);
            if (distance <= 5)
            {
                IsVisable.alpha = 1;
                if (distance > 3)
                {
                    AllertArrow.color = Color.cyan;
                }
                else
                {
                    AllertArrow.color = Color.yellow;
                }
            }
            else
            {
                IsVisable.alpha = 0;

            }
        }
    }
    public GameObject FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest = null;
        float minDistance = (PlaneSize.localScale.x * PlaneSize.localScale.y);
        float dist;

        for (int i = 0; i < enemies.Length; i++)
        {
            dist = Vector3.Distance(gamemanager.instance.player.transform.position, enemies[i].transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = enemies[i];
            }
        }

        return closest;
    }
}
