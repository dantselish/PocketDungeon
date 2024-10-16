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

    public event Action<TurnState> TurnStateChanged;

    public HeroManager Hero => _hero;
    public TurnState TurnState => _currentTurnState;


    public void InitLevel()
    {
        _enemies = new List<EnemyManager>();

        SpawnHeroAndInit();
        SpawnEnemies();

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
        return new Vector2Int(0, 0);
    }

    public List<Vector2Int> GetEnemiesSpawnPos()
    {
        return new List<Vector2Int>() { new Vector2Int(3, 4), new Vector2Int( 4, 3) };
    }

    public void RegisterDiceEnergyBonus(StatBox statBox)
    {
        statBox.BonusApplied += StatTextOnBonusApplied;
    }

    private void SpawnHeroAndInit()
    {
        _hero = Instantiate(HeroPrefab, GM.GridManager.GetTileByCoordinates(GetHeroSpawnPos()).CharacterPosition, Quaternion.identity, transform);
        _hero.Init(GM.GridManager.GetTileByCoordinates(GetHeroSpawnPos()), new Stats(6, 1, 1, 1, 2, this));
    }

    private void SpawnEnemies()
    {
        Tile spawnTile = GM.GridManager.GetTileByCoordinates(3, 4);
        EnemyManager enemyGo = Instantiate(EnemyPrefab, spawnTile.CharacterPosition, Quaternion.identity, transform);
        enemyGo.Init(spawnTile, new Stats(2, 5, 4, 4, 3, this));
        enemyGo.CharacterDied += EnemyGoOnCharacterDied;
        _enemies.Add(enemyGo);
        spawnTile = GM.GridManager.GetTileByCoordinates(4, 3);
        enemyGo = Instantiate(EnemyPrefab, spawnTile.CharacterPosition, Quaternion.identity, transform);
        enemyGo.Init(spawnTile, new Stats(2, 5, 4, 4, 3, this));
        _enemies.Add(enemyGo);
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
        if (_currentTurnState >= TurnState.AFTER_LAST)
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

    private void StatTextOnBonusApplied(int value)
    {
        if (_currentTurnState != TurnState.ENERGY)
        {
            return;
        }

        _energyDiceBonuses.Remove(value);

        if (!_energyDiceBonuses.Any())
        {
            NextTurnState();
        }
    }

    private void EnemyGoOnCharacterDied(CharacterManager enemy)
    {
        EnemyManager enemyManager = enemy as EnemyManager;

        if (!enemyManager)
        {
            Debug.LogError($"Method {nameof(EnemyGoOnCharacterDied)} was called by non enemy!");
            return;
        }

        _enemies.Remove(enemyManager);

        if (!_enemies.Any())
        {
            Debug.Log("WIN");
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

    AFTER_LAST
}
