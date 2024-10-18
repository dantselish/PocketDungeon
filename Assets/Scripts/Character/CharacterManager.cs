using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;


public abstract class CharacterManager : MyMonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int Hp;
    [SerializeField] private int Speed;
    [SerializeField] private int Attack;
    [SerializeField] private int Defence;
    [SerializeField] private int AttackRange;

    [Space]
    [SerializeField] private Vector2Int startCoordinates;

    [FormerlySerializedAs("HealthBar")]
    [Header("References")]
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private CharacterAnimationManager animationManager;

    private IEnumerator _movementCoroutine;
    private Tile _currentTile;
    private GridManager _gridManager;

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


    public void Init(LevelManager levelManager)
    {
        Stats = new Stats(Hp, Speed, Attack, Defence, AttackRange, levelManager);

        if (healthBar)
        {
            Stats.CurrentHp.ValueChanged             += healthBar.OnHealthChanged;
            Stats.RemainingSpeed.ValueChanged        += healthBar.OnSpeedChanged;
            Stats.RemainingAttack.ValueChanged       += healthBar.OnAttackChanged;
            Stats.RemainingDefence.ValueChanged      += healthBar.OnDefenceChanged;
            Stats.RemainingAttackRange.ValueChanged  += healthBar.OnAttackRangeChanged;
            healthBar.Init(Stats);
        }

        GM.LevelManager.TurnStateChanged += LevelManagerOnTurnStateChanged;

        Move(new List<Tile>(){ GM.GridManager.GetTileByCoordinates(startCoordinates) }, true);
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
            animationManager.Move(true);
            foreach (Tile tile in path)
            {
                MoveToTileImmediately(tile);
                if (tile != path.Last())
                {
                    yield return new WaitForSeconds(1f);
                }
            }
            animationManager.Move(false);

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
        Stats.TakeDamage(totalAttackPoint, out int totalDamage);
        animationManager.TakeDamage(totalDamage);

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

        animationManager.Attack();
        int attackPointsUsed = Stats.AttackEnemy(enemyManager.Stats, useMaxAttackPoints);
        enemyManager.TakeDamage(attackPointsUsed);
        return true;
    }
    #endregion

    public void Heal()
    {
        animationManager.Heal();
        Stats.Heal();
    }

    public void ApplyAdditionalStat(StatType statType, int value)
    {
        Stats.SetAdditionalStat(statType, value);
    }

    public void ApplyUpgradeStat(StatType statType)
    {
        animationManager.Cheer();
        Stats.AddUpgradeToStat(statType);
    }

    protected virtual void Die()
    {
        animationManager.Die();

        if (healthBar)
        {
            healthBar.gameObject.SetActive(false);
        }

        CurrentTile.RemoveCharacterFromTile(this);

        CharacterDied?.Invoke(this);
    }

    private void LevelManagerOnTurnStateChanged(TurnState state)
    {
        if (state == TurnState.NONE || state >= TurnState.AFTER_LAST_DEFAULT_TURN_STATE)
        {
            animationManager.Battle(false);
        }
        else
        {
            animationManager.Battle(true);
        }
    }
}