using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Damageindicator : MonoBehaviour
{
    
    

    GameObject Enemy;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Enemy = gamemanager.instance.FindClosestEnemy();
        if (Enemy != null)
        {
            Vector3 Direction = (Enemy.transform.position - gamemanager.instance.player.transform.position);
            float angle = (Vector3.SignedAngle(Direction, gamemanager.instance.player.transform.forward, Vector3.up));
           transform.localEulerAngles = new Vector3(0, 0, angle);
        }
        { }
    }

    
}
