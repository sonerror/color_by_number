using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class LevelBuilder : EditorWindow
{
    private Texture2D sourceTexture;
    private LevelData targetLevelData;
    private string levelDataFileName = "NewLevelData";
    private string levelName = "Level 1"; // Thêm trường tên level

    [MenuItem("Tools/Level Builder")]
    public static void ShowWindow()
    {
        GetWindow<LevelBuilder>("Level Builder");
    }

    void OnGUI()
    {
        GUILayout.Label("Level Generation Settings", EditorStyles.boldLabel);

        sourceTexture = (Texture2D)EditorGUILayout.ObjectField("Source Texture", sourceTexture, typeof(Texture2D), false);
        targetLevelData = (LevelData)EditorGUILayout.ObjectField("Target Level Data (Optional)", targetLevelData, typeof(LevelData), false);
        levelDataFileName = EditorGUILayout.TextField("File Name (if new)", levelDataFileName);
        levelName = EditorGUILayout.TextField("Level Name", levelName); // Nhập tên level

        if (GUILayout.Button("Generate Level Data"))
        {
            if (sourceTexture == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign a Source Texture.", "OK");
                return;
            }
            if (string.IsNullOrEmpty(levelName))
            {
                EditorUtility.DisplayDialog("Error", "Please enter a Level Name.", "OK");
                return;
            }

            Generate();
        }

        EditorGUILayout.HelpBox("Ensure your Source Texture has 'Read/Write Enabled' checked in its Import Settings, and 'Compression' set to 'None' for accurate color reading. Set Filter Mode to Point (No Filter).", MessageType.Info);
    }

    void Generate()
    {
        if (!sourceTexture.isReadable)
        {
            EditorUtility.DisplayDialog("Error", "Source Texture must have 'Read/Write Enabled' checked in its Import Settings.", "OK");
            return;
        }

        LevelData newLevelData;
        if (targetLevelData != null)
        {
            newLevelData = targetLevelData;
            Debug.Log($"Updating existing LevelData: {targetLevelData.name}");
        }
        else
        {
            newLevelData = ScriptableObject.CreateInstance<LevelData>();
            string path = $"Assets/{levelDataFileName}.asset";
            AssetDatabase.CreateAsset(newLevelData, path);
            Debug.Log($"Created new LevelData: {path}");
        }

        newLevelData.levelIndex = 0;
        newLevelData.levelWidth = sourceTexture.width;
        newLevelData.levelHeight = sourceTexture.height;
        newLevelData.pixels = new List<PixelData>();
        newLevelData.colorSwatches = new List<LevelData.ColorSwatchData>();

        Dictionary<Color, int> uniqueColors = new Dictionary<Color, int>();
        int currentID = 1;

        Color[] colors = sourceTexture.GetPixels();

        for (int y = 0; y < sourceTexture.height; y++)
        {
            for (int x = 0; x < sourceTexture.width; x++)
            {
                Color pixelColor = colors[x + y * sourceTexture.width];

                if (pixelColor.a < 0.01f) // Kiểm tra alpha để bỏ qua pixel trong suốt
                {
                    continue;
                }

                Color32 color32 = pixelColor;
                Color normalizedColor = color32; // Sử dụng Color32 để chuẩn hóa và tránh sai số float

                int id;
                if (uniqueColors.ContainsKey(normalizedColor))
                {
                    id = uniqueColors[normalizedColor];
                }
                else
                {
                    id = currentID++;
                    uniqueColors.Add(normalizedColor, id);
                    newLevelData.colorSwatches.Add(new LevelData.ColorSwatchData(id, normalizedColor));
                }

                newLevelData.pixels.Add(new PixelData(id, pixelColor, new Vector2Int(x, y)));
            }
        }

        newLevelData.colorSwatches.Sort((a, b) => a.id.CompareTo(b.id));

        EditorUtility.SetDirty(newLevelData);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Success", $"Level Data '{levelName}' generated successfully!", "OK");
    }
}