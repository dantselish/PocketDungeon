using System;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelContainer", menuName = "ScriptableObjects/LevelContainer")]
public class LevelsContainer : ScriptableObject
{
    [SerializeField] private Level[] Levels;


    public Level GetLevel(int index)
    {
        if (index < 0 || index >= Levels.Length)
        {
            Debug.LogError($"No level with index {index}");
            return null;
        }

        return Levels[index];
    }

    public Level GetNextLevelPrefab(Level level)
    {
        int levelIndex = Array.IndexOf(Levels, level);
        if (levelIndex < 0)
        {
            return Levels[0];
        }

        ++levelIndex;
        if (levelIndex >= Levels.Length)
        {
            return null;
        }

        return Levels[levelIndex];
    }
}
