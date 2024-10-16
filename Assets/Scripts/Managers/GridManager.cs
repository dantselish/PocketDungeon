using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random=UnityEngine.Random;

public class GridManager : MyMonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector2Int GridSize;

    [Space]
    [Header("References")]
    [SerializeField] private Tile TilePrefab;

    private List<Tile> _tiles = new List<Tile>();


    public void Init()
    {
        SpawnTiles();
    }

    public List<Tile> GetPathToTile(Vector2Int start, Vector2Int end, int maxDistance, bool allowWalkThroughMonsters, out int distance)
    {
        Pathfinding pathfinding = new Pathfinding(GridSize, this);
        List<PathfindingNode> nodes = pathfinding.FindPath(start, end, maxDistance, allowWalkThroughMonsters);
        distance = pathfinding.CalculatePathDistance(nodes);

        List<Tile> path = new List<Tile>();
        foreach (PathfindingNode node in nodes)
        {
            path.Add(GetTile(node));
        }
        return path;
    }

    public bool GetHighlightedTile(out Tile tile)
    {
        tile = null;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask(new []{"Tile"})))
        {
            Transform objectHit = hit.transform;

            if (objectHit.transform.CompareTag("Tile"))
            {
                tile = objectHit.GetComponent<Tile>();
                if (!tile)
                {
                    Debug.LogError($"No {nameof(Tile)} component on object with Tile tag!");
                    return false;
                }

                return true;
            }
        }

        return false;
    }

    public Tile GetTileByCoordinates(Vector2Int coordinates)
    {
        return GetTileByCoordinates(coordinates.x, coordinates.y);
    }

    public Tile GetTileByCoordinates(int x, int y)
    {
        Vector2Int coordinates = new Vector2Int(x, y);
        Tile result = _tiles.FirstOrDefault(tile => tile.Coordinates == coordinates);
        if (!result)
        {
            Debug.LogWarning($"No {nameof(Tile)} found for coordinates {x},{y}!");
        }

        return result;
    }

    public Tile GetAttackHeroTile(Vector2Int startCoordinates, int maxAttackDistance)
    {
        Pathfinding pathfinding = new Pathfinding(GridSize, this);
        AttackNode attackNode = pathfinding.FindBestAttackNode(startCoordinates, GM.LevelManager.Hero.CurrentTile, maxAttackDistance);
        return attackNode.PathfindingNode.Tile;
    }

    private void SpawnTiles()
    {
        for (int curX = 0; curX < GridSize.x; curX++)
        {
            for (int curY = 0; curY < GridSize.y; curY++)
            {
                Vector3 position = transform.position + new Vector3(curX * TilePrefab.SizeOnGrid.x, 0, curY * TilePrefab.SizeOnGrid.y);
                Tile tile = Instantiate(TilePrefab, position, Quaternion.identity, transform);
                bool isObstacle = Random.Range(0, 5) == 0;
                if (GM.LevelManager.GetAllSpawnPos().Contains(new Vector2Int(curX, curY)))
                {
                    isObstacle = false;
                }
                else if (curX == 1 && curY == 0)
                {
                    isObstacle = true;
                }
                tile.Init(curX, curY, isObstacle);
                _tiles.Add(tile);
            }
        }
    }

    private Tile GetTile(PathfindingNode node)
    {
        return GetTileByCoordinates(node.Coordinates.x, node.Coordinates.y);
    }
}
