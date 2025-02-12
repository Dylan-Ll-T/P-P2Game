using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Damageindicator : MonoBehaviour
{
    public Transform Player;
    public Transform Enemy;
    public Transform DamageImagePivot;
    public Vector3 DamageLocation;
    public CanvasGroup IsVisable;
    public float distance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 Direction = (Enemy.position - Player.position).normalized;
        float angle = (Vector3.SignedAngle(Direction, Player.forward, Vector3.up));
        DamageImagePivot.transform.localEulerAngles = new Vector3(0, 0, angle);
    }
}
