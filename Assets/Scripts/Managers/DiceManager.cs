using System;
using System.Collections.Generic;
using Random=UnityEngine.Random;

public class DiceManager : MyMonoBehaviour
{
    public event Action<List<int>> DicesRolled;


    public List<int> GetDiceValues()
    {
        List<int> result = new List<int>();
        for (int i = 0; i < 3; i++)
        {
            result.Add(Random.Range(1, 7));
        }

        DicesRolled?.Invoke(result);
        return result;
    }
}
