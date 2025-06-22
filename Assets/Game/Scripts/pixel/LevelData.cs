using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Level/Level Data")]
public class LevelData : ScriptableObject
{
    public int levelIndex;
    public int levelWidth;
    public int levelHeight;
    public List<PixelData> pixels; // Danh sách các pixel có màu (không trong suốt)
    public List<ColorSwatchData> colorSwatches; // Danh sách các ColorSwatch

    // Nested class để lưu trữ dữ liệu ColorSwatch
    [System.Serializable]
    public class ColorSwatchData
    {
        public int id;
        public Color color;

        public ColorSwatchData(int id, Color color)
        {
            this.id = id;
            this.color = color;
        }
    }
}