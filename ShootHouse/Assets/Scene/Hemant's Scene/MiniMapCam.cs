using UnityEngine;

public class MiniMapCam : MonoBehaviour
{

    public GameObject Player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = new Vector3(Player.transform.position.x, -60, Player.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        CamNoRotate();
    }

    private void CamNoRotate()
    {
        transform.position = new Vector3(Player.transform.position.x, -60, Player.transform.position.z);
        transform.rotation = Quaternion.Euler(90f, 180f, 0f);
    }
}
