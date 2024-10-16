using UnityEngine;

public class PathfindingNode
{
    public int GCost { get; set; }
    public int HCost { get; set; }
    public PathfindingNode PreviousNode { get; set; }

    public Tile Tile { get; private set; }


    public Vector2Int Coordinates => Tile.Coordinates;
    public int FCost => GCost + HCost;


    public PathfindingNode(Tile tile)
    {
        Tile = tile;
    }

    public bool IsWalkable(bool allowWalkThroughMonsters)
    {
        if (!allowWalkThroughMonsters && Tile.IsOccupied)
        {
            return false;
        }

        if (allowWalkThroughMonsters && (Tile.IsObstacle || Tile.HasHero))
        {
            return false;
        }

        return true;
    }
}
