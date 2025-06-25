using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Để sử dụng LINQ cho GetLevelByName

[CreateAssetMenu(fileName = "NewLevelList", menuName = "Level/Level List")]
public class LevelList : ScriptableObject
{
    public List<LevelData> levels = new List<LevelData>();

    public LevelData GetLevelByIdIndex(int index)
    {
        return levels.Find(l => l.levelIndex == index);
    }
    public LevelData GetLevel(int index)
    {
        return levels[index];
    }
}