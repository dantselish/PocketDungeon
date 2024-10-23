using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class LevelManager : MyMonoBehaviour
{
    [SerializeField] private HeroManager HeroPrefab;
    [SerializeField] private EnemyManager EnemyPrefab;

    private HeroManager _hero;
    private List<EnemyManager> _enemies;
    private TurnState _currentTurnState;
    private List<int> _energyDiceBonuses;
    private Level _level;
    private int _levelIndex;

    public event Action<TurnState> TurnStateChanged;

    public HeroManager Hero => _hero;
    public TurnState TurnState => _currentTurnState;


    public void InitFirstLevel()
    {
        _levelIndex = -1;
        InitHero();
        SetTurnState(TurnState.LOADING_NEXT_LEVEL);
    }

    private void InitNextLevel()
    {
        Level nextLevelPrefab = GM.LevelsContainer.GetNextLevelPrefab(++_levelIndex);
        Level nextLevel = Instantiate(nextLevelPrefab, transform);
        if (nextLevel)
        {
            if (_level)
            {
                _level.SetLevelActive(false);
            }

            _level = nextLevel;
            _level.SetLevelActive(true);

            GM.GridManager.InitTiles(_level.GetTiles());
        }

        MoveHeroToNewLevel();

        InitEnemies(_level.GetAllEnemies());

        SetTurnState(TurnState.ENERGY, 2f);
    }

    public void EndHeroTurn()
    {
        if (_currentTurnState == TurnState.HERO)
        {
            NextTurnState();
        }
    }

    public bool TryGetHighlightedEnemy(out EnemyManager enemyManager)
    {
        const string LAYER_NAME = "Enemy";

        enemyManager = null;

        if (GM.CameraManager.RaycastFromCamera(Single.MaxValue, LayerMask.GetMask(LAYER_NAME), out RaycastHit hit))
        {
            Transform objectHit = hit.transform;

            enemyManager = objectHit.GetComponent<EnemyManager>();
            if (!enemyManager)
            {
                Debug.LogError($"No {nameof(EnemyManager)} component on object with {LAYER_NAME} layer!");
                return false;
            }

            return true;
        }

        return false;
    }

    public void RegisterDiceBonus(StatBox statBox)
    {
        statBox.BonusApplied += StatBoxOnBonusApplied;
    }

    public void RegisterLevelHeal(HealButton healButton)
    {
        healButton.Healed += HealButtonOnHealed;
    }

    public List<Tile> GetPathToNextLevelTile()
    {
        Tile targetTile = GM.GridManager.GetTileByCoordinates(_level.EndPosition);
        if (targetTile == Hero.CurrentTile)
        {
            return new List<Tile>();
        }

        return GM.GridManager.GetPathToTile(Hero.Coordinates, _level.EndPosition, Int32.MaxValue, true, out _);
    }

    private Vector2Int GetHeroSpawnPos()
    {
        return _level.HeroStartPosition;
    }

    private void InitHero()
    {
        _hero = Instantiate(HeroPrefab, transform);
        _hero.Init(this);
        _hero.LevelBonusChosen += HeroOnLevelBonusChosen;
    }

    private void MoveHeroToNewLevel()
    {
        List<Tile> path = new List<Tile>(){ GM.GridManager.GetTileByCoordinates(GetHeroSpawnPos()) };
        _hero.Move(path, true);
    }

    private void InitEnemies(List<EnemyManager> enemies)
    {
        _enemies = enemies;

        foreach (EnemyManager enemy in _enemies)
        {
            enemy.InitEnemy(this);

            enemy.CharacterDied += EnemyOnCharacterDied;
        }
    }

    private void SetTurnState(TurnState turnState, float delay = 0.0f)
    {
        StartCoroutine(changeStateCoroutine());

        IEnumerator changeStateCoroutine()
        {
            yield return new WaitForSeconds(delay);

            _currentTurnState = turnState;

            if (turnState == TurnState.ENERGY)
            {
                RollDicesAndSpawnEnergy();
            }

            if (turnState == TurnState.ENEMY_MOVEMENT)
            {
                MoveEnemies();
            }

            if (turnState == TurnState.ENEMY_ATTACK)
            {
                EnemiesTriggerAttack();
            }

            if (turnState == TurnState.LOADING_NEXT_LEVEL)
            {
                InitNextLevel();
            }

            TurnStateChanged?.Invoke(turnState);
        }
    }

    private void NextTurnState()
    {
        ++_currentTurnState;
        if (_currentTurnState >= TurnState.AFTER_LAST_DEFAULT_TURN_STATE)
        {
            _currentTurnState = TurnState.ENERGY;
        }

        SetTurnState(_currentTurnState);
    }

    private void MoveEnemies()
    {
        StartCoroutine(moveEnemiesCoroutine());

        IEnumerator moveEnemiesCoroutine()
        {
            foreach (EnemyManager enemyManager in _enemies)
            {
                if (!enemyManager)
                {
                    continue;
                }

                Tile attackTile = GM.GridManager.GetAttackHeroTile(enemyManager.Coordinates, enemyManager.Stats.AttackRange.Value);
                enemyManager.MoveToPosition(attackTile);
                while (enemyManager.IsMoving)
                {
                    yield return new WaitForSeconds(0.25f);
                }
                yield return new WaitForSeconds(0.75f);
            }

            while (_enemies.Any(enemy => enemy.IsMoving))
            {
                yield return new WaitForSeconds(1f);
            }

            NextTurnState();
        }
    }

    private void EnemiesTriggerAttack()
    {
        StartCoroutine(attackCoroutine());

        IEnumerator attackCoroutine()
        {
            foreach (EnemyManager enemyManager in _enemies)
            {
                enemyManager.TryAttackPlayer(Hero);
                while (enemyManager.IsAttacking)
                {
                    yield return new WaitForSeconds(0.25f);
                }
                yield return new WaitForSeconds(0.75f);
            }

            NextTurnState();
        }
    }

    private void RollDicesAndSpawnEnergy()
    {
        GM.DiceManager.RollDices();
        GM.DiceManager.DicesRolled += DiceManagerOnDicesRolled;
    }

    private void DiceManagerOnDicesRolled(List<int> results)
    {
        _energyDiceBonuses = results;
    }

    private void StatBoxOnBonusApplied(StatBoxBonusAppliedParams eventParams)
    {
        if (eventParams.isEnergyBonus)
        {
            if (_currentTurnState != TurnState.ENERGY)
            {
                return;
            }

            Hero.ApplyAdditionalStat(eventParams.statType, eventParams.diceValue);

            _energyDiceBonuses.Remove(eventParams.diceValue);

            if (!_energyDiceBonuses.Any())
            {
                NextTurnState();
            }
        }

        if (eventParams.isLevelBonus)
        {
            if (_currentTurnState != TurnState.LEVEL_WON)
            {
                return;
            }

            Hero.ApplyUpgradeStat(eventParams.statType);
        }
    }

    private void HealButtonOnHealed()
    {
        if (_currentTurnState != TurnState.LEVEL_WON)
        {
            return;
        }

        Hero.Heal();
    }


    private void EnemyOnCharacterDied(CharacterManager enemy)
    {
        EnemyManager enemyManager = enemy as EnemyManager;

        if (!enemyManager)
        {
            Debug.LogError($"Method {nameof(EnemyOnCharacterDied)} was called by non enemy!");
            return;
        }

        _enemies.Remove(enemyManager);

        if (!_enemies.Any())
        {
            SetTurnState(TurnState.LEVEL_WON);
        }
    }

    private void HeroOnLevelBonusChosen()
    {
        if (TurnState == TurnState.LEVEL_WON)
        {
            SetTurnState(TurnState.GOING_TO_NEXT_LEVEL, 2.5f);
            SetTurnState(TurnState.LOADING_NEXT_LEVEL, 5f);
        }
    }
}

public enum TurnState
{
    NONE,

    ENERGY,
    HERO,
    ENEMY_MOVEMENT,
    ENEMY_ATTACK,

    AFTER_LAST_DEFAULT_TURN_STATE,
    LEVEL_WON,
    GOING_TO_NEXT_LEVEL,
    LOADING_NEXT_LEVEL
}
