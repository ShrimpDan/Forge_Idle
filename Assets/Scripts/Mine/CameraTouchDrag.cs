using UnityEngine;

[System.Serializable]
public struct CameraLimit
{
    public float MinX, MaxX, MinY, MaxY;
}

public class CameraTouchDrag : MonoBehaviour
{
    public float dragSpeed = 0.6f;
    public float smoothTime = 0.14f;

    [Header("카메라 이동 리밋")]
    public float minX, maxX, minY, maxY;

    private Vector3 lastWorldPos;
    private Vector3 targetPos;
    private Vector3 velocity;
    private bool isDragging;

    private void Awake()
    {
        targetPos = Camera.main.transform.position;
    }

    public void SetCameraLimit(CameraLimit limit)
    {
        minX = limit.MinX;
        maxX = limit.MaxX;
        minY = limit.MinY;
        maxY = limit.MaxY;

        Vector3 now = Camera.main.transform.position;
        now.x = Mathf.Clamp(now.x, minX, maxX);
        now.y = Mathf.Clamp(now.y, minY, maxY);
        Camera.main.transform.position = now;
        targetPos = now;
    }

    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouseDrag();
#elif UNITY_ANDROID || UNITY_IOS
        HandleTouchDrag();
#else
        HandleMouseDrag();
#endif
        Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position, targetPos, ref velocity, smoothTime);
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

    Vector3 GetWorld(Vector3 screenPos)
    {
        Vector3 camPos = Camera.main.transform.position;
        screenPos.z = -camPos.z;
        return Camera.main.ScreenToWorldPoint(screenPos);
    }

    void MoveCamera(Vector3 delta)
    {
        Vector3 next = targetPos + delta * dragSpeed;
        next.x = Mathf.Clamp(next.x, minX, maxX);
        next.y = Mathf.Clamp(next.y, minY, maxY);
        next.z = Camera.main.transform.position.z;
        targetPos = next;
    }
}
