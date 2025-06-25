using UnityEngine;
using UnityEngine.EventSystems;

public class Player : Singleton<Player>
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float dragSensitivity = 0.1f;

    private Pixel currentPixelBeingPainted;
    private Vector2 lastTouchScreenPosition;
    private bool isDraggingActive = false;

    private const int MAX_OVERLAP_HITS = 8;
    private Collider2D[] overlapResultsBuffer = new Collider2D[MAX_OVERLAP_HITS];

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            OnPointerDown(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            OnPointerHold(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            OnPointerUp();
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) return;

            if (touch.phase == TouchPhase.Began)
            {
                OnPointerDown(touch.position);
            }
            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                OnPointerHold(touch.position);
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                OnPointerUp();
            }
        }
    }

    private void OnPointerDown(Vector2 screenPosition)
    {
        lastTouchScreenPosition = screenPosition;
        isDraggingActive = false;
        ProcessInteractionAndPaint(screenPosition, true);
        Debug.Log("Hit");
    }

    private void OnPointerHold(Vector2 screenPosition)
    {
        if (Vector2.Distance(screenPosition, lastTouchScreenPosition) > dragSensitivity)
        {
            isDraggingActive = true;
        }

        if (isDraggingActive)
        {
            ProcessInteractionAndPaint(screenPosition, false);
        }
        lastTouchScreenPosition = screenPosition;
    }

    private void OnPointerUp()
    {
        isDraggingActive = false;
        currentPixelBeingPainted = null;
    }

    private void ProcessInteractionAndPaint(Vector2 screenPosition, bool isInitialTap)
    {
        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(screenPosition);

        int numHits = Physics2D.OverlapPointNonAlloc(worldPoint, overlapResultsBuffer);
        Debug.Log(numHits);
        for (int i = 0; i < numHits; i++)
        {
            Collider2D hitCollider = overlapResultsBuffer[i];

            if (hitCollider == null) continue;

            Pixel hitPixel = hitCollider.GetComponent<Pixel>();

            if (hitPixel != null && !hitPixel.IsFilledIn)
            {
                if (isInitialTap || hitPixel != currentPixelBeingPainted)
                {
                    currentPixelBeingPainted = hitPixel;

                    if (hitPixel.ID == LevelManager.Ins.IDSelected)
                    {
                        hitPixel.Fill();
                        LevelManager.Ins.OnPixelFilled(hitPixel);
                        hitPixel.SetSelected(false);
                    }
                    else
                    {
                        hitPixel.FillWrong();
                    }

                    return;
                }
            }
        }

        bool noPixelHitInThisInteraction = true;
        for (int i = 0; i < numHits; i++)
        {
            if (overlapResultsBuffer[i] != null && overlapResultsBuffer[i].GetComponent<Pixel>() != null)
            {
                noPixelHitInThisInteraction = false;
                break;
            }
        }
        if (noPixelHitInThisInteraction)
        {
            currentPixelBeingPainted = null;
        }
    }
}