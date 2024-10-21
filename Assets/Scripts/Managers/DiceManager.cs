using System;
using System.Collections.Generic;
using UnityEngine;
using Random=UnityEngine.Random;

public class DiceManager : MyMonoBehaviour
{
    [SerializeField] private Dice DicePrefab;
    [SerializeField] private List<Transform> DiceSpawnPositions;

    private List<int> _results;
    private List<Dice> _spawnedDices = new List<Dice>();

    public event Action<List<int>> DicesRolled;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            ThrowDice();
        }
    }

    public void RollDices()
    {
        DestroyDices();

        _results = new List<int>();

        for (int i = 0; i < 3; i++)
        {
            Dice dice = ThrowDice();
            dice.DiceStopped += DiceOnDiceStopped;
        }
    }

    private Dice ThrowDice()
    {
        Vector3 spawnPosition = DiceSpawnPositions[Random.Range(0, DiceSpawnPositions.Count)].position;
        Dice dice = Instantiate(DicePrefab, spawnPosition, Quaternion.identity, transform);
        dice.Throw();
        _spawnedDices.Add(dice);
        return dice;
    }

    private void DestroyDices()
    {
        foreach (Dice spawnedDice in _spawnedDices)
        {
            Destroy(spawnedDice.gameObject);
        }
        _spawnedDices.Clear();
    }

    private void DiceOnDiceStopped(int result)
    {
        _results.Add(result);

        if (_results.Count >= 3)
        {
            DicesRolled?.Invoke(_results);
            Invoke(nameof(DestroyDices), 1f);
        }
    }
}
