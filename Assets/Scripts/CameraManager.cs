using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    Camera cam;
    public Vector3 focusedCenter = Vector3.zero;
    Vector2 lastMousePos;
    public float reclineSpeed = 30, rotateSpeed = 1, zoomSpeed = 10;
    float reclineAngle = 45, rot = 0, dist = 2.2f;
    bool mouseDown;

    public static CameraManager Instance { get; private set; }
    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        if (mouseDown)
        {
            rot += rotateSpeed * (Input.mousePosition.x - lastMousePos.x);
            reclineAngle += reclineSpeed * (Input.mousePosition.y - lastMousePos.y);
            reclineAngle = Mathf.Clamp(reclineAngle, .01f, 179f);
            lastMousePos = Input.mousePosition;
        }
        if (Input.GetMouseButtonDown(1))
        {
            mouseDown = true;
            lastMousePos = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(1))
            mouseDown = false;
        
        transform.position = focusedCenter + Quaternion.Euler(reclineAngle, rot, 0) * new Vector3(0, 10, 0);
        transform.LookAt(focusedCenter);
        //dist += Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * -1;
        dist = Mathf.Clamp(dist, 1, 8);
        cam.orthographicSize = dist;
    }
}
