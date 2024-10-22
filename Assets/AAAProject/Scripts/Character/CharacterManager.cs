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
    [SerializeField] protected Vector2Int startCoordinates;

    [FormerlySerializedAs("HealthBar")]
    [Header("References")]
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private CharacterAnimationManager animationManager;

    private IEnumerator _movementCoroutine;
    private Tile _currentTile;
    private GridManager _gridManager;

    protected bool isDead;

    public event Action LevelBonusChosen;

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

    public bool IsAttacking { get; private set; }
    public bool IsMoving => _movementCoroutine != null;
    public bool CanMoveOrAttack => !IsMoving && !IsAttacking;


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


    public virtual void Init(LevelManager levelManager)
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

        animationManager.Init();

        GM.LevelManager.TurnStateChanged += LevelManagerOnTurnStateChanged;
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

    private void MovePath(List<Tile> path)
    {
        CurrentTile = path.Last();

        _movementCoroutine = moveCoroutine();
        StartCoroutine(_movementCoroutine);

        IEnumerator moveCoroutine()
        {
            animationManager.Move(true);
            for (int i = 0; i < path.Count; i++)
            {
                Tile tile = path[i];

                if (i != 0)
                {
                    animationManager.SetLookAtTarget(path[i].CharacterPosition);
                }

                MoveToPosition(tile.CharacterPosition, i == 0 ? 0f : 1f);
                if (i != 0)
                {
                    yield return new WaitForSeconds(1f);
                }
            }
            animationManager.Move(false);

            _movementCoroutine = null;
        }
    }

    private void MoveToPosition(Vector3 position, float duration)
    {
        animationManager.MoveTo(position, duration);
    }

    private void LookAt(Vector3 position)
    {
        animationManager.SetLookAtTarget(position);
    }
    #endregion

    #region Attack
    public void TakeDamage(int totalAttackPoint, Vector3 attackerPos)
    {
        Stats.TakeDamage(totalAttackPoint, out int totalDamage);
        animationManager.TakeDamage(totalDamage, attackerPos);

        if (Stats.CurrentHp.Value <= 0)
        {
            Die();
        }
    }

    private bool CanAttack(Stats enemyStats, Tile enemyTile, out int distance)
    {
        distance = 0;

        if (!CurrentTile.IsTileInLos(enemyTile))
        {
            return false;
        }

        distance = GM.GridManager.GetDistance(CurrentTile.Coordinates, enemyTile.Coordinates, false);
        if (!Stats.CanAttack(enemyStats, distance))
        {
            return false;
        }

        return true;
    }

    protected bool TryAttack(CharacterManager enemyManager, bool useMaxAttackPoints = false)
    {
        if (!CanAttack(enemyManager.Stats, enemyManager.CurrentTile, out int distance))
        {
            return false;
        }

        IsAttacking = true;
        enemyManager.LookAt(transform.position);
        animationManager.Attack(enemyManager.transform.position + Vector3.up, distance, onConnect);
        return true;

        void onConnect()
        {
            int attackPointsUsed = Stats.AttackEnemy(enemyManager.Stats, useMaxAttackPoints);
            enemyManager.TakeDamage(attackPointsUsed, transform.position);
            IsAttacking = false;
        }
    }
    #endregion

    public void Heal()
    {
        animationManager.Heal();
        Stats.Heal();
        Invoke(nameof(InvokeLevelBonusChosen), 3f);
    }

    public void ApplyAdditionalStat(StatType statType, int value)
    {
        Stats.SetAdditionalStat(statType, value);
    }

    public void ApplyUpgradeStat(StatType statType)
    {
        animationManager.Cheer();
        Stats.AddUpgradeToStat(statType);
        Invoke(nameof(InvokeLevelBonusChosen), 3f);
    }

    private void InvokeLevelBonusChosen()
    {
        LevelBonusChosen?.Invoke();
    }

    protected virtual void Die()
    {
        animationManager.Die();

        if (healthBar)
        {
            healthBar.gameObject.SetActive(false);
        }

        CurrentTile.RemoveCharacterFromTile(this);

        isDead = true;
        CharacterDied?.Invoke(this);
    }

    private void LevelManagerOnTurnStateChanged(TurnState state)
    {
        if (state == TurnState.LEVEL_WON)
        {
            animationManager.Battle(false);
        }
        else
        {
            animationManager.Battle(true);
        }

        if (state == TurnState.LOADING_NEXT_LEVEL)
        {
            Move(GM.LevelManager.GetPathToNextLevelTile());
        }
    }
}