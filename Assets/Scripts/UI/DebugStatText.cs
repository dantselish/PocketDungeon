using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DebugStatText : MyMonoBehaviour, IDropHandler
{
    [SerializeField] private TMP_Text Text;
    [SerializeField] private Image    Image;

    private bool _isActive;

    public StatType StatType { get; private set; }

    public event Action<int> BonusApplied;


    public void Init(StatType statType, Stats heroStats)
    {
        StatType = statType;

        OneStat oneStat = heroStats.GetTotalMax(statType);
        SetText(oneStat.Value);

        GM.LevelManager.RegisterDiceEnergyBonus(this);

        SetActiveStatus(true);

        oneStat.ValueChanged += OnTotalValueChanged;
        GM.LevelManager.TurnStateChanged += LevelManagerOnTurnStateChanged;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!_isActive)
        {
            return;
        }

        GameObject go = eventData.pointerDrag;
        DebugDiceResult diceResult = go.GetComponent<DebugDiceResult>();
        if (diceResult)
        {
            GM.LevelManager.Hero.Stats.SetAdditionalStat(StatType, diceResult.Value);
            BonusApplied?.Invoke(diceResult.Value);
            SetActiveStatus(false);
            Destroy(diceResult.gameObject);
        }
    }

    private void SetText(int value)
    {
        Text.SetText(value.ToString());
    }

    private void SetActiveStatus(bool isActive)
    {
        _isActive = isActive;

        Color color = Image.color;
        color.a = isActive ? 1f : 0.7f;
        Image.color = color;
    }

    private void LevelManagerOnTurnStateChanged(TurnState state)
    {
        _isActive = state == TurnState.ENERGY;
    }

    private void OnTotalValueChanged(int value)
    {
        SetText(value);
    }

}
