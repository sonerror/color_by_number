
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static LevelData;

public class UIGameplay : UICanvas
{
    [SerializeField] Transform content;
    [SerializeField] ColorSwatch colorSwatchPrefab;
    [SerializeField] List<ColorSwatch> listColorSwatch;
    MiniPool<ColorSwatch> miniPool = new MiniPool<ColorSwatch>();

    private void Awake()
    {
        miniPool.OnInit(colorSwatchPrefab, 10, content);
    }
    public void SpawnColorSwitch(List<ColorSwatchData> colorSwatches)
    {
        foreach(ColorSwatchData colorSwatch in colorSwatches)
        {
            ColorSwatch color = miniPool.Spawn();
            color.SetData(colorSwatch.id, colorSwatch.color);
            listColorSwatch.Add(color);
        }
    }
}
