using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MyMonoBehaviour
{
    [SerializeField] private StatsScreen   StatsScreen;
    [SerializeField] private HealthScreen  HealthScreen;
    [SerializeField] private HeroHPUI      HeroHP;
    [SerializeField] private Button        EndHeroTurnButton;
    [SerializeField] private LoadingScreen LoadingScreen;


    public void Init()
    {
        Stats heroStats = GM.LevelManager.Hero.Stats;

        StatsScreen.Init(heroStats);
        HealthScreen.Init();
        HeroHP.Init(heroStats);

        EndHeroTurnButton.onClick.AddListener(OnEndHeroTurnButtonClicked);

        GM.DiceManager.DicesRolled += DiceManagerOnDicesRolled;
        GM.LevelManager.TurnStateChanged += LevelManagerOnTurnStateChanged;
    }

    private void LevelManagerOnTurnStateChanged(TurnState state)
    {
        EndHeroTurnButton.gameObject.SetActive(state == TurnState.HERO);

        if (state == TurnState.LOADING_NEXT_LEVEL)
        {
            LoadingScreen.Show();
        }
        else
        {
            LoadingScreen.Hide();
        }
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