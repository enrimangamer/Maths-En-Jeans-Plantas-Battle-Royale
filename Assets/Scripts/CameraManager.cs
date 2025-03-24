using UnityEngine;

public class CameraManager : MonoBehaviour
{
    Camera cam;
    Vector2 startMousePos;
    Vector2 startCamPos;
    public float cameraSpeed = 2;
    private void Start()
    {
        cam = Camera.main;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            startMousePos = Input.mousePosition;
            startCamPos = cam.transform.position;
        }
        if (Input.GetKey(KeyCode.Mouse1))
        {
            Vector2 mouseDeltta = startMousePos - (Vector2)Input.mousePosition;
            cam.transform.position = startCamPos + mouseDeltta * cameraSpeed;
            cam.transform.position = cam.transform.position + new Vector3(0, 0, -10);
        }
    }
}
