using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyArrow : MonoBehaviour
{
    public Image Pivot;
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
        //transform.position = new Vector3(0, (Pivot.transform.position.y + 60),0);
        //transform.rotation = Quaternion.Euler(Pivot.transform.rotation.x, Pivot.transform.rotation.y, Pivot.transform.rotation.z);
        Enemy = gamemanager.instance.FindClosestEnemy();
        if (Enemy != null)
        {
            distance = Vector3.Distance(gamemanager.instance.player.transform.position, Enemy.transform.position);
            if (distance <= 8 && gamemanager.instance.isPause == false)
            {
                IsVisable.alpha = 1;
                if (distance > 4)
                {
                    AllertArrow.color = Color.yellow;
                }
                else
                {
                    AllertArrow.color = Color.red;
                }
            }
            else
            {
                IsVisable.alpha = 0;

            }
        }
    }
   
}
