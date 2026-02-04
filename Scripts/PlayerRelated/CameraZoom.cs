using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraZoom : MonoBehaviour
{
    public float zoomSpeed = 5f;
    public float minZoom = 2f;
    public float maxZoom = 10f;
    public float defaultZoom = 5f;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();

        if (!cam.orthographic)
        {
            Debug.LogWarning("This script works best with an Orthographic camera.");
        } else {
            cam.orthographicSize = Mathf.Clamp(defaultZoom, minZoom, maxZoom);
        }
    }

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0f)
        {
            cam.orthographicSize -= scroll * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        }
    }
}
