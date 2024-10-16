using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugUI : MyMonoBehaviour
{
    [SerializeField] private TMP_Text HeroSpeed;
    [SerializeField] private EnergyDebugScreen EnergyScreen;


    public void Init()
    {
        GM.LevelManager.TurnStateChanged += LevelManagerOnTurnStateChanged;
        GM.DiceManager.DicesRolled += DiceManagerOnDicesRolled;
    }

    private void Update()
    {
        if (GM.LevelManager.Hero)
        {
            Stats stats = GM.LevelManager.Hero.Stats;
            HeroSpeed.SetText($"Speed: {stats.RemainingSpeed.Value}/{stats.TotalMaxSpeed.Value}");
        }
    }

    private void SetEnergyScreenActive(bool isActive)
    {
        EnergyScreen.gameObject.SetActive(isActive);
    }

    private void LevelManagerOnTurnStateChanged(TurnState state)
    {
        SetEnergyScreenActive(state == TurnState.ENERGY);
    }

    private void DiceManagerOnDicesRolled(List<int> results)
    {
        EnergyScreen.SpawnBonuses(results);
    }
}