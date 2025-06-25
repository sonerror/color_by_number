using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class Player : Singleton<Player>
{
    [SerializeField] private Camera mainCamera;

    [Header("Camera Zoom")]
    [SerializeField] private float zoomSpeed = 0.1f;
    [SerializeField] private float minZoom = 2f;
    [SerializeField] private float maxZoom = 10f;
    [SerializeField] private float zoomTweenDuration = 0.2f;

    [Header("Camera Pan")]
    [SerializeField] private float panSensitivity = 1f;
    [SerializeField] private Vector2 panLimitMin;
    [SerializeField] private Vector2 panLimitMax;

    [Header("Painting Input")]
    [SerializeField] private float dragToPaintThreshold = 5f;

    private Pixel currentPixelBeingPainted;
    private Vector2 lastInputScreenPosition;
    private Vector3 lastCameraWorldPosition;

    private bool isDraggingForPaint = false;
    private bool isCameraPanning = false;

    private const int MAX_OVERLAP_HITS = 8;
    private readonly Collider2D[] overlapResultsBuffer = new Collider2D[MAX_OVERLAP_HITS];

    private void Awake()
    {
      
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize, minZoom, maxZoom);
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        int pointerId = Input.touchCount > 0 ? Input.GetTouch(0).fingerId : -1;
        if (EventSystem.current.IsPointerOverGameObject(pointerId))
        {
            if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
            {
                ResetInteractionState();
            }
            return;
        }

        if (Input.touchCount >= 2)
        {
            HandlePinchZoom();
            ResetInteractionState();
            return;
        }
        HandleMouseScrollZoom();

        bool isMouseDown = Input.GetMouseButtonDown(0);
        bool isTouchBegan = (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began);

        bool isMouseHold = Input.GetMouseButton(0);
        bool isTouchHold = (Input.touchCount == 1 && (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Stationary));

        bool isMouseUp = Input.GetMouseButtonUp(0);
        bool isTouchEnded = (Input.touchCount == 1 && (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled));

        Vector2 currentInputPosition = Input.touchCount > 0 ? Input.GetTouch(0).position : (Vector2)Input.mousePosition;

        if (isMouseDown || isTouchBegan)
        {
            OnPointerDown(currentInputPosition);
        }
        else if (isMouseHold || isTouchHold)
        {
            OnPointerDrag(currentInputPosition);
        }
        else if (isMouseUp || isTouchEnded)
        {
            OnPointerUp();
        }
    }

    private void OnPointerDown(Vector2 screenPosition)
    {
        lastInputScreenPosition = screenPosition;
        lastCameraWorldPosition = mainCamera.transform.position;

        isDraggingForPaint = false;
        isCameraPanning = false;
        currentPixelBeingPainted = null;

        Pixel initialHitPixel = GetPixelAtScreenPosition(screenPosition);

        if (initialHitPixel != null && !initialHitPixel.IsFilledIn)
        {
            currentPixelBeingPainted = initialHitPixel;
            ProcessPainting(screenPosition, isInitialTap: true);
        }
        else
        {
            isCameraPanning = true;
        }
    }

    private void OnPointerDrag(Vector2 screenPosition)
    {
        Vector2 deltaScreenPosition = screenPosition - lastInputScreenPosition;

        if (isCameraPanning)
        {
            HandleCameraPan(deltaScreenPosition);
        }
        else
        {
            if (!isDraggingForPaint && Vector2.Distance(screenPosition, lastInputScreenPosition) > dragToPaintThreshold)
            {
                isDraggingForPaint = true;
            }

            if (isDraggingForPaint)
            {
                ProcessPainting(screenPosition, isInitialTap: false);
            }
        }

        lastInputScreenPosition = screenPosition;
    }

    private void OnPointerUp()
    {
        ResetInteractionState();
    }

    private void ResetInteractionState()
    {
        isDraggingForPaint = false;
        isCameraPanning = false;
        currentPixelBeingPainted = null;
    }

    private void HandleMouseScrollZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.001f)
        {
            float newZoom = mainCamera.orthographicSize - scroll * zoomSpeed * mainCamera.orthographicSize;
            ApplyZoom(newZoom);
        }
    }

    private void HandlePinchZoom()
    {
        Touch touch0 = Input.GetTouch(0);
        Touch touch1 = Input.GetTouch(1);

        Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
        Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

        float prevMagnitude = (touch0PrevPos - touch1PrevPos).magnitude;
        float currentMagnitude = (touch0.position - touch1.position).magnitude;

        float deltaMagnitude = currentMagnitude - prevMagnitude;

        float newZoom = mainCamera.orthographicSize - deltaMagnitude * zoomSpeed * 0.01f;
        ApplyZoom(newZoom);
    }

    private void ApplyZoom(float targetZoom)
    {
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);

        mainCamera.DOKill();
        mainCamera.DOOrthoSize(targetZoom, zoomTweenDuration).SetEase(Ease.OutQuad);
    }

    private void HandleCameraPan(Vector2 screenDelta)
    {
        float unitsPerPixel = mainCamera.orthographicSize * 2f / Screen.height;
        Vector3 worldDelta = new Vector3(screenDelta.x * unitsPerPixel, screenDelta.y * unitsPerPixel, 0);

        Vector3 newPosition = mainCamera.transform.position - worldDelta * panSensitivity;

        newPosition.x = Mathf.Clamp(newPosition.x, panLimitMin.x, panLimitMax.x);
        newPosition.y = Mathf.Clamp(newPosition.y, panLimitMin.y, panLimitMax.y);
        newPosition.z = mainCamera.transform.position.z;

        mainCamera.transform.position = newPosition;
    }

    private void ProcessPainting(Vector2 screenPosition, bool isInitialTap)
    {
        Pixel hitPixel = GetPixelAtScreenPosition(screenPosition);

        if (hitPixel != null && !hitPixel.IsFilledIn)
        {
            if (isInitialTap || hitPixel != currentPixelBeingPainted)
            {
                currentPixelBeingPainted = hitPixel;

                if (hitPixel.ID == LevelManager.Ins.IDSelected)
                {
                    hitPixel.Fill();
                    LevelManager.Ins.OnPixelFilled(hitPixel);
                }
                else
                {
                    hitPixel.FillWrong();
                }
            }
        }
        else if (hitPixel == null && !isInitialTap && isDraggingForPaint)
        {
            currentPixelBeingPainted = null;
        }
    }

    private Pixel GetPixelAtScreenPosition(Vector2 screenPosition)
    {
        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(screenPosition);
        int numHits = Physics2D.OverlapPointNonAlloc(worldPoint, overlapResultsBuffer);

        for (int i = 0; i < numHits; i++)
        {
            if (overlapResultsBuffer[i] == null) continue;

            if (overlapResultsBuffer[i].TryGetComponent<Pixel>(out var pixel))
            {
                return pixel;
            }
        }
        return null;
    }
}