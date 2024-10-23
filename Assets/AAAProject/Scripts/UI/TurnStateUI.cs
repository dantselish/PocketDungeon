using UnityEngine;

public class TurnStateUI : MonoBehaviour
{
    [SerializeField] private RectTransform Indicator;
    [SerializeField] private RectTransform Energy;
    [SerializeField] private RectTransform Hero;
    [SerializeField] private RectTransform EnemyMovement;
    [SerializeField] private RectTransform EnemyAttack;


    public void Init(LevelManager levelManager)
    {
        levelManager.TurnStateChanged += LevelManagerOnTurnStateChanged;
    }

    private void LevelManagerOnTurnStateChanged(TurnState state)
    {
        switch (state)
        {
            case TurnState.ENERGY:
                Indicator.position = Energy.position;
                gameObject.SetActive(true);
                break;
            case TurnState.HERO: Indicator.position = Hero.position; break;
            case TurnState.ENEMY_MOVEMENT: Indicator.position = EnemyMovement.position; break;
            case TurnState.ENEMY_ATTACK: Indicator.position = EnemyAttack.position; break;

            default: gameObject.SetActive(false); break;
        }
    }
}
