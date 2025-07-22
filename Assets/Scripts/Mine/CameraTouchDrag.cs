using UnityEngine;

[System.Serializable]
public struct CameraLimit
{
    public float MinX, MaxX, MinY, MaxY;
}

public class CameraTouchDrag : MonoBehaviour
{
    [Header("카메라 줌(확대/축소)")]
    public float minCameraSize = 5f;
    public float maxCameraSize = 15f;

    [HideInInspector]
    public bool enableZoom = true; // 씬 관리자가 on/off

    private float zoomSpeedTouch = 0.04f;
    private float zoomSpeedMouse = 2f;

    public float dragSpeed = 0.6f;
    public float smoothTime = 0.14f;

    [Header("카메라 이동 리밋")]
    public float minX, maxX, minY, maxY;

    private Vector3 lastWorldPos;
    private Vector3 targetPos;
    private Vector3 velocity;
    private bool isDragging;

    [Header("직접 연결")]
    public Camera targetCamera; // Inspector에서 MineCamera 직접 연결

    private void Awake()
    {
        if (targetCamera == null)
            Debug.LogError("CameraTouchDrag: targetCamera가 연결되어 있지 않습니다!");
        else
            targetPos = targetCamera.transform.position;
    }

    public void SetCameraLimit(CameraLimit limit)
    {
        minX = limit.MinX;
        maxX = limit.MaxX;
        minY = limit.MinY;
        maxY = limit.MaxY;

        if (targetCamera == null)
        {
            Debug.LogError("CameraTouchDrag: targetCamera가 연결되어 있지 않습니다!");
            return;
        }
        Vector3 now = targetCamera.transform.position;
        now.x = Mathf.Clamp(now.x, minX, maxX);
        now.y = Mathf.Clamp(now.y, minY, maxY);
        targetCamera.transform.position = now;
        targetPos = now;
    }

    // ★ MineDetailCanvas로 돌아올 때 호출해주면 됩니다!
    public void SetCameraSize(float size)
    {
        if (targetCamera == null) return;
        targetCamera.orthographicSize = Mathf.Clamp(size, minCameraSize, maxCameraSize);
    }

    void Update()
    {
        if (targetCamera == null) return;
        HandleZoom();
#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouseDrag();
#elif UNITY_ANDROID || UNITY_IOS
        HandleTouchDrag();
#else
        HandleMouseDrag();
#endif
        targetCamera.transform.position = Vector3.SmoothDamp(targetCamera.transform.position, targetPos, ref velocity, smoothTime);
    }

    Vector3 GetWorld(Vector3 screenPos)
    {
        Vector3 camPos = targetCamera.transform.position;
        screenPos.z = -camPos.z;
        return targetCamera.ScreenToWorldPoint(screenPos);
    }

    void HandleMouseDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastWorldPos = GetWorld(Input.mousePosition);
            isDragging = true;
        }
        if (Input.GetMouseButtonUp(0)) isDragging = false;

        if (isDragging && Input.GetMouseButton(0))
        {
            Vector3 curWorldPos = GetWorld(Input.mousePosition);
            Vector3 delta = lastWorldPos - curWorldPos;
            delta.z = 0;
            MoveCamera(delta);
            lastWorldPos = curWorldPos;
        }
    }

    void HandleTouchDrag()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                lastWorldPos = GetWorld(touch.position);
                isDragging = true;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
            }
            else if (isDragging && touch.phase == TouchPhase.Moved)
            {
                Vector3 curWorldPos = GetWorld(touch.position);
                Vector3 delta = lastWorldPos - curWorldPos;
                delta.z = 0;
                MoveCamera(delta);
                lastWorldPos = curWorldPos;
            }
        }
    }

    void MoveCamera(Vector3 delta)
    {
        Vector3 next = targetPos + delta * dragSpeed;
        next.x = Mathf.Clamp(next.x, minX, maxX);
        next.y = Mathf.Clamp(next.y, minY, maxY);
        next.z = targetCamera.transform.position.z;
        targetPos = next;
    }

    void HandleZoom()
    {
        if (!enableZoom) return;

#if UNITY_EDITOR || UNITY_STANDALONE
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            float newSize = targetCamera.orthographicSize - scroll * zoomSpeedMouse;
            targetCamera.orthographicSize = Mathf.Clamp(newSize, minCameraSize, maxCameraSize);
        }
#endif
        // 모바일 핀치 줌
        if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);
            Vector2 prevT0 = t0.position - t0.deltaPosition;
            Vector2 prevT1 = t1.position - t1.deltaPosition;

            float prevMag = (prevT0 - prevT1).magnitude;
            float curMag = (t0.position - t1.position).magnitude;
            float diff = prevMag - curMag;
            float newSize = targetCamera.orthographicSize + diff * zoomSpeedTouch;
            targetCamera.orthographicSize = Mathf.Clamp(newSize, minCameraSize, maxCameraSize);
        }
    }
}
