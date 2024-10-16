using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public abstract class CharacterManager : MyMonoBehaviour
{
    [SerializeField] private HealthBar healthBar;

    private IEnumerator _movementCoroutine;
    private Tile _currentTile;

    public Stats Stats { get; protected set; }

    public Tile CurrentTile
    {
        get => _currentTile;
        private set
        {
            if (CurrentTile)
            {
                _currentTile.RemoveCharacterFromTile(this);
            }

            _currentTile = value;
            _currentTile.SetCharacterOnTile(this);
        }
    }

    public bool IsMoving => _movementCoroutine != null;

    public Vector2Int Coordinates
    {
        get
        {
            if (CurrentTile)
            {
                return CurrentTile.Coordinates;
            }

            return new Vector2Int(-1, -1);
        }
    }

    public abstract bool IsHero { get; }

    public event Action<CharacterManager> CharacterDied;


    public void Init(Tile startTile, Stats stats)
    {
        Stats = stats;

        if (healthBar)
        {
            Stats.CurrentHp.ValueChanged             += healthBar.OnHealthChanged;
            Stats.RemainingSpeed.ValueChanged        += healthBar.OnSpeedChanged;
            Stats.RemainingAttack.ValueChanged       += healthBar.OnAttackChanged;
            Stats.RemainingDefence.ValueChanged      += healthBar.OnDefenceChanged;
            Stats.RemainingAttackRange.ValueChanged  += healthBar.OnAttackRangeChanged;
            healthBar.Init(stats);
        }

        Move(new List<Tile>(){ startTile }, true);
    }

    protected Tile GetMyTile()
    {
        return GM.GridManager.GetTileByCoordinates(Coordinates);
    }

    #region Movement
    public void Move(List<Tile> path, bool immediate = false)
    {
        if (immediate)
        {
            path = new List<Tile>(){ path.Last() };
        }

        if (path != null && path.Count > 0)
        {
            MovePath(path);
        }
    }

    private void MoveToTileImmediately(Tile tile)
    {
        MoveToPositionImmediately(tile.CharacterPosition);
    }

    private void MovePath(List<Tile> path)
    {
        CurrentTile = path.Last();

        _movementCoroutine = moveCoroutine();
        StartCoroutine(_movementCoroutine);

        IEnumerator moveCoroutine()
        {
            foreach (Tile tile in path)
            {
                MoveToTileImmediately(tile);
                if (tile != path.Last())
                {
                    yield return new WaitForSeconds(1f);
                }

            }

            _movementCoroutine = null;
        }
    }

    private void MoveToPositionImmediately(Vector3 position)
    {
        transform.position = position;
    }
    #endregion

    #region Attack
    public void TakeDamage(int totalAttackPoint)
    {
        Stats.TakeDamage(totalAttackPoint);

        if (Stats.CurrentHp.Value <= 0)
        {
            Die();
        }
    }

    protected bool CanAttack(Stats enemyStats, Tile enemyTile)
    {
        if (!CurrentTile.IsTileInLos(enemyTile))
        {
            return false;
        }

        int distance = GM.GridManager.GetDistance(CurrentTile.Coordinates, enemyTile.Coordinates, false);
        if (!Stats.CanAttack(enemyStats, distance))
        {
            return false;
        }

        return true;
    }

    protected bool TryAttack(CharacterManager enemyManager, Tile enemyTile, bool useMaxAttackPoints = false)
    {
        if (!CanAttack(enemyManager.Stats, enemyTile))
        {
            return false;
        }

        int attackPointsUsed = Stats.AttackEnemy(enemyManager.Stats, useMaxAttackPoints);
        enemyManager.TakeDamage(attackPointsUsed);
        return true;
    }
    #endregion

    protected virtual void Die()
    {
        CurrentTile.RemoveCharacterFromTile(this);

        CharacterDied?.Invoke(this);
    }
}