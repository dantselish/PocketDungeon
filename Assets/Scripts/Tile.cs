using System.Collections.Generic;
using UnityEngine;

public class Tile : MyMonoBehaviour
{
    [SerializeField] private List<Transform> TileCorners;
    [SerializeField] private bool            Obstacle;

    private Vector2Int _coordinates = Vector2Int.zero;
    private CharacterManager _characterOnTile;

    public Vector2 SizeOnGrid => Vector2.one;
    public Vector2Int Coordinates => _coordinates;
    public Vector3 CharacterPosition => transform.position + Vector3.up * 0.01f;
    public bool IsOccupied => Obstacle || HasCharacter;
    public bool IsObstacle => Obstacle;
    public bool HasCharacter => _characterOnTile;
    public bool HasHero
    {
        get
        {
            if (_characterOnTile)
            {
                return _characterOnTile.IsHero;
            }

            return false;
        }
    }
    public bool HasEnemy => HasCharacter && !HasHero;


    public void Init(Vector2Int coordinates)
    {
        _coordinates = coordinates;
    }

    public void SetCharacterOnTile(CharacterManager character)
    {
        _characterOnTile = character;
    }

    public void RemoveCharacterFromTile(CharacterManager character)
    {
        if (_characterOnTile == character)
        {
            _characterOnTile = null;
        }
    }

    public bool IsTileInLos(Tile targetTile)
    {
        foreach (Transform myCorner in TileCorners)
        {
            foreach (Transform targetCorner in targetTile.TileCorners)
            {
                Ray ray = new Ray(myCorner.position, targetCorner.position - myCorner.position);
                RaycastHit[] raycastHits = new RaycastHit[10]; 
                Physics.RaycastNonAlloc(ray, raycastHits, Vector3.Distance(myCorner.position, targetCorner.position), LayerMask.GetMask(new [] {"Tile"}));

                bool hasLoS = true;
                foreach (RaycastHit raycastHit in raycastHits)
                {
                    if (!raycastHit.transform)
                    {
                        continue;
                    }

                    Tile hitTile = raycastHit.transform.gameObject.GetComponent<Tile>();
                    if (!hitTile)
                    {
                        Debug.LogWarning($"Ray hit {raycastHit.transform.name} without {nameof(Tile)} component!");
                        return false;
                    }

                    if (hitTile.IsOccupied && hitTile != targetTile && hitTile != this)
                    {
                        hasLoS = false;
                        break;
                    }
                }

                if (hasLoS)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool TryGetEnemy(out EnemyManager enemyManager)
    {
        enemyManager = null;

        if (!HasEnemy)
        {
            return false;
        }

        enemyManager = _characterOnTile as EnemyManager;
        return enemyManager;
    }
}
