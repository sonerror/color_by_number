
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static LevelData;
using System.Collections;
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
    public void DespawnColorSwatch(int id)
    {
        foreach(ColorSwatch color in listColorSwatch)
        {
            if(color.ID == id)
            {
                color.PlayOnCompleteParticle();
                StartCoroutine(IE_PlayOnCompleteParticle(color));
                return; 
            }
        }
    }

    IEnumerator IE_PlayOnCompleteParticle(ColorSwatch color)
    {
        yield return new WaitForSeconds(0.45f);
        miniPool.Despawn(color);
        listColorSwatch.Remove(color);
    }
}
