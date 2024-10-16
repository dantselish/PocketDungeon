using System.Collections.Generic;
using UnityEngine;

public class UI : MyMonoBehaviour
{
    [SerializeField] private StatsScreen StatsScreen;
    [SerializeField] private HeroHPUI    HeroHP;


    public void Init()
    {
        Stats heroStats = GM.LevelManager.Hero.Stats;

        StatsScreen.Init(heroStats);
        HeroHP.Init(heroStats);

        GM.DiceManager.DicesRolled += DiceManagerOnDicesRolled;
    }

    private void DiceManagerOnDicesRolled(List<int> results)
    {
        StatsScreen.SpawnBonuses(results);
    }
}