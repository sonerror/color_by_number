using UnityEngine;

public class CameraController : Singleton<CameraController>
{
    [SerializeField] private float zoomPadding = 1f; 
    [SerializeField] private float cameraZPosition = -10f; 

    private Camera mainCamera;

    void Awake()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
        }
    }
    public void CenterCameraOnLevel(LevelData levelData)
    {
        if (levelData == null || mainCamera == null)
        {
            return;
        }

        int levelWidth = levelData.levelWidth;
        int levelHeight = levelData.levelHeight;
       Vector3 center = new Vector3((levelWidth - 1) / 2f, (levelHeight - 1) / 2f, cameraZPosition);
        mainCamera.transform.position = center;
        float screenRatio = (float)Screen.width / Screen.height;
        float levelRatio = (float)levelWidth / levelHeight;
        float orthographicSize;
        if (levelRatio >= screenRatio)
        {
            orthographicSize = (levelWidth / 2f / screenRatio) + zoomPadding;
        }
        else
        {
            orthographicSize = (levelHeight / 2f) + zoomPadding;
        }
        mainCamera.orthographicSize = orthographicSize;
    }
}