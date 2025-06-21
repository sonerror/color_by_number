using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager :   Singleton<DataManager>
{
    public bool isLoaded = false;
    public PlayerData playerData;
    public const string PLAYER_DATA = "PLAYER_DATA";
    private void OnApplicationPause(bool pause) { SaveData(); }
    private void OnApplicationQuit() { SaveData(); }
    public void LoadData()
    {
        Debug.Log("START LOAD DATA");
        string d = PlayerPrefs.GetString(PLAYER_DATA, "");
        if (d != "")
        {
            playerData = JsonUtility.FromJson<PlayerData>(d);
        }
        else
        {
            playerData = new PlayerData();
            FirstLoad();
        }
        isLoaded = true;
    }

    public void SaveData()
    {
        if (!isLoaded) return;
        string json = JsonUtility.ToJson(playerData);
        PlayerPrefs.SetString(PLAYER_DATA, json);
        Debug.Log("SAVE DATA");
    }
    void FirstLoad()
    {
      
    }
    public void ChangeGold(int newGold)
    {
        playerData.gold += newGold;
        if (playerData.gold < 0)
        {
            playerData.gold = 0;
        }
    }
}
[System.Serializable]
public class PlayerData
{
    public int levelCurrent;
    public int gold;
    public PlayerData()
    {
        levelCurrent = 0;

        gold = 10000;
    }
}

