using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsScreen : MyMonoBehaviour
{
    [SerializeField] private DiceResultUI diceResultUIPrefab;
    [SerializeField] private LayoutGroup diceContainer;


    public void Init(Stats heroStats)
    {
        StatBox[] statBoxes = GetComponentsInChildren<StatBox>();
        foreach (StatBox statBox in statBoxes)
        {
            statBox.Init(heroStats);
        }
    }

    public void SpawnBonuses(List<int> bonuses)
    {
        foreach (int bonus in bonuses)
        {
            DiceResultUI go = Instantiate(diceResultUIPrefab, diceContainer.transform);
            go.UpdateValue(bonus);
        }
    }
}
