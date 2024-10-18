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

    public event Action<TurnState> TurnStateChanged;

    public HeroManager Hero => _hero;
    public TurnState TurnState => _currentTurnState;


    public void InitNextLevel()
    {
        Level nextLevelPrefab = GM.LevelsContainer.GetNextLevelPrefab(_level);
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

        SetTurnState(TurnState.ENERGY, 1f);
    }

    public void EndHeroTurn()
    {
        if (_currentTurnState == TurnState.HERO)
        {
            NextTurnState();
        }
    }

    public List<Vector2Int> GetAllSpawnPos()
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        positions.Add(GetHeroSpawnPos());
        positions.AddRange(GetEnemiesSpawnPos());
        return positions;
    }

    public Vector2Int GetHeroSpawnPos()
    {
        return _level.HeroStartPosition;
    }

    public List<Vector2Int> GetEnemiesSpawnPos()
    {
        return new List<Vector2Int>() { new Vector2Int(3, 4), new Vector2Int( 4, 3) };
    }

    public void RegisterDiceBonus(StatBox statBox)
    {
        statBox.BonusApplied += StatBoxOnBonusApplied;
    }

    public void RegisterLevelHeal(HealButton healButton)
    {
        healButton.Healed += HealButtonOnHealed;
    }

    private void MoveHeroToNewLevel()
    {
        if (!_hero)
        {
            _hero = Instantiate(HeroPrefab, transform);
            _hero.Init(this);
        }

        List<Tile> path = new List<Tile>(){ GM.GridManager.GetTileByCoordinates(GetHeroSpawnPos()) };
        _hero.Move(path, true);
    }

    private void InitEnemies(List<EnemyManager> enemies)
    {
        _enemies = enemies;

        foreach (EnemyManager enemy in _enemies)
        {
            enemy.Init(this);

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
                yield return new WaitForSeconds(3f);
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
                yield return new WaitForSeconds(3f);
            }

            NextTurnState();
        }
    }

    private void RollDicesAndSpawnEnergy()
    {
        _energyDiceBonuses = GM.DiceManager.GetDiceValues();
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

            SetTurnState(TurnState.LOADING_NEXT_LEVEL);
        }
    }

    private void HealButtonOnHealed()
    {
        if (_currentTurnState != TurnState.LEVEL_WON)
        {
            return;
        }

        Hero.Heal();

        SetTurnState(TurnState.LOADING_NEXT_LEVEL);
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
    LOADING_NEXT_LEVEL
}
