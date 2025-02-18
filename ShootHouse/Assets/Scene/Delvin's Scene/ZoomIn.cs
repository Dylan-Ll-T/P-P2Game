using UnityEngine;

public class ZoomIn : MonoBehaviour
{
    [SerializeField] private int zoom;
    [SerializeField] private int normal;
    [SerializeField] private float transition;

    private Camera cam;
    private bool isZoomed = false;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isZoomed = !isZoomed;
        }

        if (isZoomed)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, zoom, Time.deltaTime * transition);
        }

        if (Input.GetMouseButtonUp(1))
        {
            isZoomed = false;
        }

        if (!isZoomed)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, normal, Time.deltaTime * transition);
        }
    }
}
