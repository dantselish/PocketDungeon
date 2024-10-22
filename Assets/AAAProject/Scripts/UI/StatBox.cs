using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatBox : MyMonoBehaviour, IDropHandler, IPointerClickHandler
{
    [SerializeField] private TMP_Text Text;
    [SerializeField] private Image    Image;

    private bool _isWaitingForEnergyUpgrade;
    private bool _isWaitingForLevelUpgrade;
    private bool _canUpgradeByEnergy;

    public StatType StatType;

    public event Action<StatBoxBonusAppliedParams> BonusApplied;


    public void Init(Stats heroStats)
    {
        OneStat oneStat = heroStats.GetRemaining(StatType);
        SetText(oneStat.Value);

        _canUpgradeByEnergy = StatsExtensions.GetEnergyUpgradableStatTypes().Contains(StatType);

        GM.LevelManager.RegisterDiceBonus(this);

        SetWaitingForEnergyUpgradeStatus(true);

        oneStat.ValueChanged += OnTotalValueChanged;
        GM.LevelManager.TurnStateChanged += LevelManagerOnTurnStateChanged;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!_isWaitingForEnergyUpgrade)
        {
            return;
        }

        GameObject go = eventData.pointerDrag;
        DiceResultUI diceResultUI = go.GetComponent<DiceResultUI>();
        if (diceResultUI)
        {
            BonusApplied?.Invoke(new StatBoxBonusAppliedParams(){ isEnergyBonus = true, isLevelBonus = false, diceValue = diceResultUI.Value, statType = StatType });
            SetWaitingForEnergyUpgradeStatus(false);
            Destroy(diceResultUI.gameObject);
        }
    }

    private void SetText(int value)
    {
        Text.SetText(value.ToString());
    }

    private void SetWaitingForEnergyUpgradeStatus(bool isActive)
    {
        _isWaitingForEnergyUpgrade = isActive;

        Color color = Image.color;
        color.a = isActive ? 1f : 0.7f;
        Image.color = color;
    }

    private void SetWaitingForLevelUpgradeStatus(bool isActive)
    {
        _isWaitingForLevelUpgrade = isActive;

        Color color = Image.color;
        color.a = isActive ? 1f : 0.7f;
        Image.color = color;
    }

    private void LevelManagerOnTurnStateChanged(TurnState state)
    {
        if (state == TurnState.ENERGY)
        {
            SetWaitingForEnergyUpgradeStatus(_canUpgradeByEnergy);
        }

        if (state == TurnState.LEVEL_WON)
        {
            SetWaitingForLevelUpgradeStatus(true);
        }
    }

    private void OnTotalValueChanged(int value)
    {
        SetText(value);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_isWaitingForLevelUpgrade)
        {
            BonusApplied?.Invoke(new StatBoxBonusAppliedParams(){ isEnergyBonus = false, isLevelBonus = true, diceValue = 0, statType = StatType });
        }
    }
}

public struct StatBoxBonusAppliedParams
{
    public bool isEnergyBonus;
    public bool isLevelBonus;
    public int  diceValue;
    public StatType statType;
}
