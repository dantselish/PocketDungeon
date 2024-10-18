using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] private Vector2Int HeroStartCoordinates;
    [SerializeField] private EnemyManager[] Enemies;

    public Vector2Int HeroStartPosition => HeroStartCoordinates;

    public List<Tile> GetTiles()
    {
        return GetComponentsInChildren<Tile>().ToList();
    }

    public List<EnemyManager> GetAllEnemies()
    {
        return Enemies.ToList();
    }

    public void SetLevelActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
