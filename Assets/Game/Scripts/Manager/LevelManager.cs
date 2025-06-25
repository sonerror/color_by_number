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
    private Dictionary<int, int> remainingPixelsInGroup = new Dictionary<int, int>();

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
        remainingPixelsInGroup.Clear();

        for (int i = 0; i < currentLevelData.pixels.Count; i++)
        {
            PixelData pixelData = currentLevelData.pixels[i];
            Pixel pixel = SimplePool.Spawn<Pixel>(PoolType.Pixel, new Vector3(pixelData.position.x, pixelData.position.y, 0), Quaternion.identity);
            pixel.SetData(pixelData.id, pixelData.color);

            activePixels.Add(pixel);

            if (!pixelGroupsByID.ContainsKey(pixelData.id))
            {
                pixelGroupsByID.Add(pixelData.id, new List<Pixel>());
                remainingPixelsInGroup.Add(pixelData.id, 0);
            }
            pixelGroupsByID[pixelData.id].Add(pixel);
            remainingPixelsInGroup[pixelData.id]++;
        }
        UIManager.Ins.GetUI<UIGameplay>().SpawnColorSwitch(listColorSwitch);

        //SelectNextUncompletedColorID();
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
                    if (!p.IsFilledIn)
                    {
                        p.SetSelected(false);
                    }
                }
            }
            idSelected = newID;
            if (pixelGroupsByID.ContainsKey(idSelected))
            {
                foreach (Pixel p in pixelGroupsByID[idSelected])
                {
                    if (!p.IsFilledIn)
                    {
                        p.SetSelected(true);
                    }
                }
            }
            OnColorSwatchSelected?.Invoke(idSelected);
        }
    }

    public void OnPixelFilled(Pixel filledPixel)
    {
        filledPixel.SetSelected(false); 

        if (remainingPixelsInGroup.ContainsKey(filledPixel.ID))
        {
            remainingPixelsInGroup[filledPixel.ID]--;

            if (remainingPixelsInGroup[filledPixel.ID] <= 0)
            {
                Debug.Log($"Color ID {filledPixel.ID} completed!");
                OnColorGroupCompleted?.Invoke(filledPixel.ID);
                if (!CheckIfLevelComplete())
                {
                    UIManager.Ins.GetUI<UIGameplay>().DespawnColorSwatch(filledPixel.ID);
                    SelectNextUncompletedColorID();
                } else {
                    Debug.Log("Level Completed!");
                }
            }
        }
    }

    private void SelectNextUncompletedColorID()
    {
        foreach (var entry in currentLevelData.colorSwatches)
        {
            int colorID = entry.id;
            
            if (remainingPixelsInGroup.ContainsKey(colorID) && remainingPixelsInGroup[colorID] > 0)
            {
                SetSelectedColorID(colorID);
                return;
            }
        }
        SetSelectedColorID(0);
    }

    public bool CheckIfLevelComplete()
    {
        foreach (var remainingCount in remainingPixelsInGroup.Values)
        {
            if (remainingCount >= 0)
            {
                return false;
            }
        }
        return true;
    }

    private IEnumerator LoadLevelAsync(int levelIndex, UnityAction onComplete = null)
    {
        yield return null; 
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
        remainingPixelsInGroup.Clear();
    }
}