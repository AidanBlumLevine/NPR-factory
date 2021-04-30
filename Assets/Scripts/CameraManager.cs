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

    void Start()
    {
        dist = (Grid.data.gridSize.z + 1) / 2;
        focusedCenter = new Vector3(0, Grid.data.gridSize.y / 2f - 1f, 0);
        cam.orthographicSize = dist;
    }

    void Update()
    {
        if (Input.GetMouseButton(2) || Input.GetKey(KeyCode.Space) && (Input.GetMouseButton(1) || Input.GetMouseButton(0)))
        {
            rot += rotateSpeed * (Input.mousePosition.x - lastMousePos.x);
            reclineAngle += reclineSpeed * (Input.mousePosition.y - lastMousePos.y);
            reclineAngle = Mathf.Clamp(reclineAngle, .01f, 179f);
        }

        transform.position = focusedCenter + Quaternion.Euler(reclineAngle, rot, 0) * new Vector3(0, 20, 0);
        transform.LookAt(focusedCenter);

        lastMousePos = Input.mousePosition;
    }
}
