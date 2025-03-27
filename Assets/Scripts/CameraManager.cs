using System.Collections;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    Camera cam;
    Vector2 startMousePos;
    Vector2 startCamPos;
    public float cameraSpeed = 2;
    float cameraZoomSpeed = 0;
    public float cameraZoomMouseForce;
    public static CameraManager instance;
    public bool lerpingPos = false;
    Vector2 mouseDeltta = Vector2.zero;
    private void Start()
    {
        cam = Camera.main;
        instance = this;
    }
    private void Update()
    {
        cameraZoomSpeed += Input.mouseScrollDelta.y*Time.deltaTime*cameraZoomMouseForce;
        cameraZoomSpeed = Mathf.Lerp(cameraZoomSpeed, 0, Time.deltaTime * 6);
        cam.orthographicSize += cameraZoomSpeed;
        if (cam.orthographicSize < 1)
        {
            cam.orthographicSize = 1;
            cameraZoomSpeed *= -1;
        }


        if (lerpingPos) return;
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            startMousePos = Input.mousePosition;
            startCamPos = cam.transform.position;
           // StartCoroutine(lerpToPoint(cam.ScreenToWorldPoint(startMousePos)));
        }
        if (Input.GetKey(KeyCode.Mouse1))
        {
            mouseDeltta = startMousePos - (Vector2)Input.mousePosition;
        }
        cam.transform.position = Vector3.Lerp(cam.transform.position, startCamPos + mouseDeltta * cameraSpeed, Time.deltaTime * 15);
        cam.transform.position = cam.transform.position + new Vector3(0, 0, -10);
    }

    public void startLerpToPlant(GameObject plant)
    {
        if (lerpingPos) return;
        StartCoroutine(lerpToPoint(plant.transform.position));
        StartCoroutine(lerpToZoom(8));
    }

    IEnumerator lerpToPoint(Vector3 pos)
    {
        lerpingPos = true;
        while (MathHelpers.distance(pos, gameObject.transform.position) > 0.05f)
        {
            transform.position = Vector3.Lerp(gameObject.transform.position, pos, Time.deltaTime * 5);
            transform.position = new Vector3(transform.position.x, transform.position.y, -10);
            yield return new WaitForEndOfFrame();
        }
        lerpingPos = false;
        mouseDeltta = Vector2.zero;
        startCamPos = cam.transform.position;
        yield return null;
    }
    IEnumerator lerpToZoom(float zoom)
    {
        while (Mathf.Pow(cam.orthographicSize - zoom, 2) > 0.1f)
        {
            cam.orthographicSize=Mathf.Lerp(cam.orthographicSize, zoom, Time.deltaTime*3);
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
}

public static class MathHelpers
{
    public static float distance(Vector2 a, Vector2 b)
    {
        float x = b.x - a.x;
        float y = b.y - a.y;
        float h = Mathf.Sqrt(x*x + y*y);
        return h;
    }
}



