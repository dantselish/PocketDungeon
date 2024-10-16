using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatBox : MyMonoBehaviour, IDropHandler
{
    [SerializeField] private TMP_Text Text;
    [SerializeField] private Image    Image;

    private bool _isActive;
    private bool _canUpgradeByEnergy;

    public StatType StatType;

    public event Action<int> BonusApplied;


    public void Init(Stats heroStats)
    {
        OneStat oneStat = heroStats.GetRemaining(StatType);
        SetText(oneStat.Value);

        _canUpgradeByEnergy = StatsExtensions.GetEnergyUpgradableStatTypes().Contains(StatType);

        if (_canUpgradeByEnergy)
        {
            GM.LevelManager.RegisterDiceEnergyBonus(this);
        }

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
        DiceResultUI diceResultUI = go.GetComponent<DiceResultUI>();
        if (diceResultUI)
        {
            GM.LevelManager.Hero.Stats.SetAdditionalStat(StatType, diceResultUI.Value);
            BonusApplied?.Invoke(diceResultUI.Value);
            SetActiveStatus(false);
            Destroy(diceResultUI.gameObject);
        }
    }

    private void SetText(int value)
    {
        Text.SetText(value.ToString());
    }

    private void SetActiveStatus(bool isActive)
    {
        if (!_canUpgradeByEnergy)
        {
            _isActive = false;
            return;
        }

        _isActive = isActive;

        Color color = Image.color;
        color.a = isActive ? 1f : 0.7f;
        Image.color = color;
    }

    private void LevelManagerOnTurnStateChanged(TurnState state)
    {
        SetActiveStatus(state == TurnState.ENERGY);
    }

    private void OnTotalValueChanged(int value)
    {
        SetText(value);
    }

}
