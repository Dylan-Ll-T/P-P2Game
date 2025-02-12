using UnityEngine;

public class CanvasMark : MonoBehaviour
{
    public GameObject Player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Player.transform.position.x, -30f, Player.transform.position.z);
        transform.rotation = Quaternion.Euler(90f, -19.305f,0f);
    }
}
