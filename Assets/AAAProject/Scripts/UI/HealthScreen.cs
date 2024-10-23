using UnityEngine;

public class HealthScreen : MonoBehaviour
{
    [SerializeField] private HealButton  HealButton;
    [SerializeField] private HeroHPUI    HeroHp;
    [SerializeField] private TurnStateUI TurnState;


    public void Init(LevelManager levelManager, Stats heroStats)
    {
        HeroHp.Init(heroStats);
        HealButton.Init(levelManager);
        TurnState.Init(levelManager);
    }
}
