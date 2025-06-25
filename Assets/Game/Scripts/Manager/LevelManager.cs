using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using System;
using UnityEngine.Events;
using static LevelData;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] LevelList listLevelData;
    [SerializeField] LevelData currentLevelData;
    [SerializeField] List<ColorSwatchData> listColorSwitch;
    [SerializeField] List<Pixel> activePixels = new List<Pixel>();

    private Dictionary<int, List<Pixel>> pixelGroupsByID = new Dictionary<int, List<Pixel>>();

    public LevelData CurrentLevelData => currentLevelData;
    [SerializeField] int idSelected = 0;
    public int IDSelected => idSelected;

    public static event Action<int> OnColorSwatchSelected;

    public static event Action<int> OnColorGroupCompleted;

    public void OnLoadLevel(int levelIndex, UnityAction onComplete = null)
    {
        ClearCurrentLevelPixels();
        currentLevelData = listLevelData.GetLevelByIdIndex(levelIndex);
        listColorSwitch = currentLevelData.colorSwatches;

        if (currentLevelData == null)
        {
            Debug.LogError($"LevelData for index {levelIndex} not found!");
            return;
        }

        pixelGroupsByID.Clear();

        for (int i = 0; i < currentLevelData.pixels.Count; i++)
        {
            PixelData pixelData = currentLevelData.pixels[i];
            Pixel pixel = SimplePool.Spawn<Pixel>(PoolType.Pixel, new Vector3(pixelData.position.x, pixelData.position.y, 0), Quaternion.identity);
            pixel.SetData(pixelData.id,pixelData.color);

            activePixels.Add(pixel);

            if (!pixelGroupsByID.ContainsKey(pixelData.id))
            {
                pixelGroupsByID.Add(pixelData.id, new List<Pixel>());
            }
            pixelGroupsByID[pixelData.id].Add(pixel);
        }
        UIManager.Ins.GetUI<UIGameplay>().SpawnColorSwitch(listColorSwitch);

        onComplete?.Invoke();
    }

    public void SetSelectedColorID(int newID)
    {
        if(newID != idSelected)
        {
            if (idSelected != 0 && pixelGroupsByID.ContainsKey(idSelected))
            {
                foreach (Pixel p in pixelGroupsByID[idSelected])
                {
                    p.SetSelected(false);
                }
            }
            idSelected = newID;
            if (pixelGroupsByID.ContainsKey(idSelected))
            {
                foreach (Pixel p in pixelGroupsByID[idSelected])
                {
                    p.SetSelected(true);
                }
            }
            OnColorSwatchSelected?.Invoke(idSelected);
        }
    }

    public void OnPixelFilled(Pixel filledPixel)
    {
        if (filledPixel.ID == idSelected)
        {
            if (CheckIfCurrentColorGroupComplete())
            {
                Debug.Log($"Color ID {idSelected} completed!");
                OnColorGroupCompleted?.Invoke(idSelected);
            }
        }
    }

    private bool CheckIfCurrentColorGroupComplete()
    {
        if (pixelGroupsByID.ContainsKey(idSelected))
        {
            foreach (Pixel p in pixelGroupsByID[idSelected])
            {
                if (!p.IsFilledIn)
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }

    public bool CheckIfLevelComplete()
    {
        foreach (var group in pixelGroupsByID.Values)
        {
            foreach (Pixel p in group)
            {
                if (!p.IsFilledIn)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private IEnumerator LoadLevelAsync(int levelIndex, UnityAction onComplete = null)
    {
        for (int i = 0; i < currentLevelData.pixels.Count; i++)
        {
            PixelData pixelData = currentLevelData.pixels[i];
            Pixel pixel = SimplePool.Spawn<Pixel>(PoolType.Pixel, new Vector3(pixelData.position.x, pixelData.position.y, 0), Quaternion.identity);
            pixel.SetData(pixelData.id,pixelData.color);
            activePixels.Add(pixel);

            if (!pixelGroupsByID.ContainsKey(pixelData.id))
            {
                pixelGroupsByID.Add(pixelData.id, new List<Pixel>());
            }
            pixelGroupsByID[pixelData.id].Add(pixel);

            yield return new WaitForSeconds(0.01f);
        }
        UIManager.Ins.GetUI<UIGameplay>().SpawnColorSwitch(listColorSwitch);
        onComplete?.Invoke();
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
        pixelGroupsByID.Clear();
    }
}