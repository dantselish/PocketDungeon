using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MyMonoBehaviour
{
    [SerializeField] private StatsScreen StatsScreen;
    [SerializeField] private HeroHPUI    HeroHP;
    [SerializeField] private Button      EndHeroTurnButton;


    public void Init()
    {
        Stats heroStats = GM.LevelManager.Hero.Stats;

        StatsScreen.Init(heroStats);
        HeroHP.Init(heroStats);

        EndHeroTurnButton.onClick.AddListener(OnEndHeroTurnButtonClicked);

        GM.DiceManager.DicesRolled += DiceManagerOnDicesRolled;
        GM.LevelManager.TurnStateChanged += LevelManagerOnTurnStateChanged;
    }

    private void LevelManagerOnTurnStateChanged(TurnState state)
    {
        EndHeroTurnButton.gameObject.SetActive(state == TurnState.HERO);
    }

    private void DiceManagerOnDicesRolled(List<int> results)
    {
        StatsScreen.SpawnBonuses(results);
    }

    private void OnEndHeroTurnButtonClicked()
    {
        GM.LevelManager.EndHeroTurn();
    }
}