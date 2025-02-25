using UnityEngine;

public class MiniMapCam : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = new Vector3(gamemanager.instance.player.transform.position.x, +60, gamemanager.instance.player.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        CamNoRotate();
    }

    private void CamNoRotate()
    {
        transform.position = new Vector3(gamemanager.instance.player.transform.position.x, +60, gamemanager.instance.player.transform.position.z);
        transform.rotation = Quaternion.Euler(90f, 180f, 0f);
    }
}
