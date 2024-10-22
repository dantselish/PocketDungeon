using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random=UnityEngine.Random;

public class GridManager : MyMonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector2Int GridSize;
    [SerializeField] private Vector2Int TileSize;

    [Space]
    [Header("References")]
    [SerializeField] private Tile TilePrefab;

    private List<Tile> _tiles = new List<Tile>();


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

    public bool TryGetHighlightedTile(out Tile tile)
    {
        const string LAYER_NAME = "Tile";

        tile = null;

        if (GM.CameraManager.RaycastFromCamera(Single.MaxValue, LayerMask.GetMask(new []{LAYER_NAME}), out RaycastHit hit))
        {
            Transform objectHit = hit.transform;

            tile = objectHit.GetComponent<Tile>();
            if (!tile)
            {
                Debug.LogError($"No {nameof(Tile)} component on object with {LAYER_NAME} layer!");
                return false;
            }

            return true;
        }

        return false;
    }

    public int GetDistance(Vector2Int start, Vector2Int end, bool allowWalkThroughMonsters)
    {
        Pathfinding pathfinding = new Pathfinding(GridSize, this);
        List<PathfindingNode> nodes = pathfinding.FindPath(start, end, Int32.MaxValue, allowWalkThroughMonsters, true);
        return pathfinding.CalculatePathDistance(nodes);
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

    public void InitTiles(IEnumerable<Tile> tiles)
    {
        _tiles.Clear();
        foreach (Tile tile in tiles)
        {
            Vector3 tilePos = tile.transform.position;
            Vector2Int coordinates = new Vector2Int((int) (tilePos.x / TileSize.x - 0.5f), (int) (tilePos.z / TileSize.y - 0.5f));
            tile.Init(coordinates);
            _tiles.Add(tile);
        }
    }

    private Tile GetTile(PathfindingNode node)
    {
        return GetTileByCoordinates(node.Coordinates.x, node.Coordinates.y);
    }
}
