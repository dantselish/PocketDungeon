using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyDebugScreen : MyMonoBehaviour
{
    [SerializeField] private DebugDiceResult DiceResultPrefab;
    [SerializeField] private DebugStatText StatTextPrefab;

    [SerializeField] private LayoutGroup statsContainer;
    [SerializeField] private LayoutGroup diceContainer;

    private List<DebugStatText> StatTexts = new List<DebugStatText>();
    private List<DebugDiceResult> DiceResults = new List<DebugDiceResult>();


    private void Start()
    {
        Init();
    }

    private void Init()
    {
        foreach (StatType statType in StatsExtensions.GetEnergyUpgradableStatTypes())
        {
            DebugStatText go = Instantiate(StatTextPrefab, statsContainer.transform);
            StatTexts.Add(go);
            go.Init(statType, GM.LevelManager.Hero.Stats);
        }
    }

    public void SpawnBonuses(List<int> bonuses)
    {
        foreach (int bonus in bonuses)
        {
            DebugDiceResult go = Instantiate(DiceResultPrefab, diceContainer.transform);
            go.UpdateValue(bonus);
        }
    }
}
