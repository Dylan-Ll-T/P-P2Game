using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyArrow : MonoBehaviour
{
    public Transform Player;
    public Transform Enemy;
    public CanvasGroup IsVisable;
    public float distance;
    public Image AllertArrow;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(Player.transform.position, Enemy.transform.position);
        if (distance <= 59)
        {
            IsVisable.alpha = 1;
            if (distance > 15)
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
