using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using System;
using UnityEngine.Events;
public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] LevelList listLevelData;
    [SerializeField] LevelData currentLevelData;
    [SerializeField] List<Pixel> activePixels = new List<Pixel>(); 
    public LevelData CurrentLevelData => currentLevelData;
    public void OnLoadLevel(int levelIndex, UnityAction onComplete = null)
    {
        ClearCurrentLevelPixels();
        currentLevelData = listLevelData.GetLevel(levelIndex);
        if (currentLevelData == null)
        {
            return;
        }
        for (int i = 0; i < currentLevelData.pixels.Count; i++)
        {
            PixelData pixelData = currentLevelData.pixels[i];
            Pixel pixel = SimplePool.Spawn<Pixel>(PoolType.Pixel, new Vector3(pixelData.position.x, pixelData.position.y, 0), Quaternion.identity);
            pixel.SetData(pixelData.color, pixelData.id);
            activePixels.Add(pixel);
        }
        //StartCoroutine(LoadLevelAsync(levelIndex));
        onComplete?.Invoke();
    }
 private IEnumerator LoadLevelAsync(int levelIndex)
 {
  for (int i = 0; i < currentLevelData.pixels.Count; i++)
        {
            PixelData pixelData = currentLevelData.pixels[i];
            Pixel pixel = SimplePool.Spawn<Pixel>(PoolType.Pixel, new Vector3(pixelData.position.x, pixelData.position.y, 0), Quaternion.identity);
            pixel.SetData(pixelData.color, pixelData.id);
            activePixels.Add(pixel);
            yield return new WaitForSeconds(0.01f);
        }
 }
    public void OnLoadNextLevel(UnityAction onComplete = null)
    {
        OnLoadLevel(currentLevelData.levelIndex + 1, onComplete);
    }

    public void OnLoadPreviousLevel(UnityAction onComplete = null)
    {
        OnLoadLevel(currentLevelData.levelIndex - 1, onComplete);
    }
    private void ClearCurrentLevelPixels()
    {
        if (activePixels != null)
        {
            foreach (Pixel p in activePixels)
            {
                if (p != null)
                {
                    SimplePool.Despawn(p); 
                }
            }
            activePixels.Clear(); 
        }
    }
}