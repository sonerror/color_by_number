using UnityEngine;
using System; 

[Serializable]
public class PixelData
{
    public int id;
    public Color color;
    public Vector2Int position; 

    public PixelData(int id, Color color, Vector2Int position)
    {
        this.id = id;
        this.color = color;
        this.position = position;
    }
}