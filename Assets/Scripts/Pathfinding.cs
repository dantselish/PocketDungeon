using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinding
{
    private const int STRAIGHT_MOVE_COST = 2;
    private const int DIAGONAL_MOVE_COST = 3;

    private Vector2Int _gridSize;
    private List<PathfindingNode> _nodes;


    public Pathfinding(Vector2Int gridSize, GridManager gridManager)
    {
        _gridSize = gridSize;
        _nodes = new List<PathfindingNode>();

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Tile tile = gridManager.GetTileByCoordinates(x, y);
                _nodes.Add(new PathfindingNode(tile));
            }
        }
    }

    //Used for enemies only
    public List<AttackNode> FindAttackNodes(Vector2Int targetCoordinates, Vector2Int startCoordinates, int maxAttackDistance)
    {
        PathfindingNode targetNode = GetNode(targetCoordinates);

        List<AttackNode> attackNodes = new List<AttackNode>();

        List<PathfindingNode> openList = new List<PathfindingNode>();
        List<PathfindingNode> closedList = new List<PathfindingNode>();

        openList.AddRange(GetNeighbours(targetNode));

        while (openList.Any())
        {
            PathfindingNode currentNode = openList.First();

            openList.Remove(currentNode);
            if (closedList.Contains(currentNode))
            {
                continue;
            }

            closedList.Add(currentNode);

            if (!currentNode.IsWalkable(false))
            {
                continue;
            }

            int distance = CalculatePathDistance(FindPath(targetCoordinates, currentNode.Coordinates, Int32.MaxValue, true));
            if (distance > maxAttackDistance)
            {
                continue;
            }

            if (!currentNode.Tile.IsTileInLos(targetNode.Tile))
            {
                continue;
            }

            if (currentNode.IsWalkable(false))
            {
                 attackNodes.Add(new AttackNode(){DistanceToTarget = distance, PathfindingNode = currentNode, DistanceToEnemy = CalculateRealDistance(startCoordinates, currentNode.Coordinates, true)});
            }

            List<PathfindingNode> neighbours = GetNeighbours(currentNode);
            foreach (PathfindingNode neighbour in neighbours)
            {
                IEnumerable<PathfindingNode> commonNeighbours = GetNeighbours(neighbour).Intersect(neighbours);

                if (commonNeighbours.All(node => !node.IsWalkable(true)))
                {
                    continue;
                }

                if (!openList.Contains(neighbour))
                {
                    openList.Add(neighbour);
                }
            }
        }

        return attackNodes;
    }

    public AttackNode FindBestAttackNode(Vector2Int startCoordinates, Tile targetTile, int maxAttackDistance)
    {
        List<AttackNode> attackNodes = FindAttackNodes(targetTile.Coordinates, startCoordinates, maxAttackDistance);

        AttackNode resultNode = attackNodes.First();
        foreach (AttackNode attackNode in attackNodes)
        {
            if (attackNode.DistanceToTarget < resultNode.DistanceToTarget)
            {
                continue;
            }

            if (attackNode.DistanceToEnemy > resultNode.DistanceToEnemy)
            {
                continue;
            }

            resultNode = attackNode;
        }

        return resultNode;
    }

    public List<PathfindingNode> FindPath(Vector2Int start, Vector2Int end, int maxDistance,  bool allowWalkThroughMonsters)
    {
        PathfindingNode startNode = GetNode(start);
        PathfindingNode endNode   = GetNode(end);

        List<PathfindingNode> openList = new List<PathfindingNode>(){ startNode };
        List<PathfindingNode> closedList = new List<PathfindingNode>();

        foreach (PathfindingNode node in _nodes)
        {
            node.GCost = Int32.MaxValue;
            node.PreviousNode = null;
        }

        startNode.GCost = 0;
        startNode.HCost = CalculateDistance(startNode, endNode);

        while (openList.Any())
        {
            PathfindingNode currentNode = GetLowestFCostNode(openList);

            if (currentNode == endNode)
            {
                return CalculatePath(endNode, maxDistance);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            List<PathfindingNode> neighbours = GetNeighbours(currentNode);
            foreach (PathfindingNode neighbourNode in neighbours)
            {
                if (closedList.Contains(neighbourNode))
                {
                    continue;
                }

                if (!neighbourNode.IsWalkable(allowWalkThroughMonsters && neighbourNode != endNode))
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                int distanceBetween = CalculateDistance(currentNode, neighbourNode);
                int tentativeGCost = currentNode.GCost + distanceBetween;
                if (tentativeGCost >= neighbourNode.GCost)
                {
                    continue;
                }

                IEnumerable<PathfindingNode> commonNeighbours = GetNeighbours(neighbourNode).Intersect(neighbours);

                if (commonNeighbours.All(node => !node.IsWalkable(true)) && distanceBetween > 2)
                {
                    continue;
                }

                neighbourNode.PreviousNode = currentNode;
                neighbourNode.GCost = tentativeGCost;
                neighbourNode.HCost = CalculateDistance(neighbourNode, endNode);

                if (!openList.Contains(neighbourNode))
                {
                    openList.Add(neighbourNode);
                }
            }
        }

        Debug.LogWarning($"No path from {start.x},{start.y} to {end.x},{end.y} found!");
        return new List<PathfindingNode>();
    }

    public int CalculatePathDistance(List<PathfindingNode> path)
    {
        int distance = 0;

        for (int i = 1; i < path.Count; i++)
        {
            distance += CalculateDistance(path[i - 1], path[i]);
        }

        return distance;
    }

    private List<PathfindingNode> CalculatePath(PathfindingNode endNode, int maxDistance = -1)
    {
        List<PathfindingNode> path = new List<PathfindingNode>();
        path.Add(endNode);

        PathfindingNode currentNode = endNode;
        while (currentNode.PreviousNode != null)
        {
            path.Add(currentNode.PreviousNode);
            currentNode = currentNode.PreviousNode;
        }

        path.Reverse();

        if (maxDistance > 0)
        {
            while (CalculatePathDistance(path) > maxDistance && path.Last().IsWalkable(false))
            {
                path.RemoveAt(path.Count - 1);
            }
        }

        return path;
    }

    private int CalculateRealDistance(PathfindingNode a, PathfindingNode b, bool allowWalkThroughMonsters)
    {
        return CalculateRealDistance(a.Coordinates, b.Coordinates, allowWalkThroughMonsters);
    }

    private int CalculateRealDistance(Vector2Int a, Vector2Int b, bool allowWalkThroughMonsters)
    {
        List<PathfindingNode> path = FindPath(a, b, Int32.MaxValue, allowWalkThroughMonsters);
        return CalculatePathDistance(path);
    }

    private int CalculateDistance(PathfindingNode a, PathfindingNode b)
    {
        int xDistance = Mathf.Abs(a.Coordinates.x - b.Coordinates.x);
        int yDistance = Mathf.Abs(a.Coordinates.y - b.Coordinates.y);

        int diagonalMovement = Mathf.Min(xDistance, yDistance);
        int straightMovement = Mathf.Abs(xDistance - yDistance);

        return DIAGONAL_MOVE_COST * diagonalMovement + STRAIGHT_MOVE_COST * straightMovement;
    }

    private PathfindingNode GetNode(int x, int y)
    {
        return GetNode(new Vector2Int(x, y));
    }

    private PathfindingNode GetNode(Vector2Int coordinates)
    {
        PathfindingNode node = _nodes.FirstOrDefault(node => node.Coordinates == coordinates);
        if (node == null)
        {
            Debug.LogWarning($"No {nameof(PathfindingNode)} found for coordinates {coordinates.x},{coordinates.y}!");
            return null;
        }

        return node;
    }

    private List<PathfindingNode> GetNeighbours(PathfindingNode node)
    {
        List<PathfindingNode> result = new List<PathfindingNode>();

        //Up
        if (node.Coordinates.y + 1 < _gridSize.y)
        {
            result.Add(GetNode(node.Coordinates.x, node.Coordinates.y + 1));

            //Upper Left
            if (node.Coordinates.x - 1 >= 0)
            {
                result.Add(GetNode(node.Coordinates.x - 1, node.Coordinates.y + 1));
            }

            //Upper Right
            if (node.Coordinates.x + 1 < _gridSize.x)
            {
                result.Add(GetNode(node.Coordinates.x + 1, node.Coordinates.y + 1));
            }
        }

        //Down
        if (node.Coordinates.y - 1 >= 0)
        {
            result.Add(GetNode(node.Coordinates.x, node.Coordinates.y - 1));

            //Down Left
            if (node.Coordinates.x - 1 >= 0)
            {
                result.Add(GetNode(node.Coordinates.x - 1, node.Coordinates.y - 1));
            }

            //Down Right
            if (node.Coordinates.x + 1 < _gridSize.x)
            {
                result.Add(GetNode(node.Coordinates.x + 1, node.Coordinates.y - 1));
            }
        }

        //Left
        if (node.Coordinates.x - 1 >= 0)
        {
            result.Add(GetNode(node.Coordinates.x - 1, node.Coordinates.y));
        }

        //Right
        if (node.Coordinates.x + 1 < _gridSize.x)
        {
            result.Add(GetNode(node.Coordinates.x + 1, node.Coordinates.y));
        }

        return result;
    }

    private PathfindingNode GetLowestFCostNode(List<PathfindingNode> nodes) => nodes.First(x => x.FCost == nodes.Min(y => y.FCost));
}

public class AttackNode
{
    public PathfindingNode PathfindingNode { get; set; }
    public int             DistanceToTarget { get; set; }
    public int             DistanceToEnemy { get; set; }
}
