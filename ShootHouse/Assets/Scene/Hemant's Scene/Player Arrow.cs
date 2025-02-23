using UnityEngine;
using UnityEngine.UI;

public class PlayerArrow : MonoBehaviour
{
    public Image playerArrow;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        playerArrow.transform.rotation = gamemanager.instance.player.transform.rotation; 
    }
}
