using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class HealButton : MyMonoBehaviour
{
    private Button _button;

    public event Action Healed; 


    public void Init(LevelManager levelManager)
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClick);
        levelManager.RegisterLevelHeal(this);
        levelManager.TurnStateChanged += LevelManagerOnTurnStateChanged;
    }

    private void LevelManagerOnTurnStateChanged(TurnState state)
    {
        if (state == TurnState.LEVEL_WON)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void OnClick()
    {
        Healed?.Invoke();
        gameObject.SetActive(false);
    }
}