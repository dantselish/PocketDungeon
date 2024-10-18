using UnityEngine;

public class Stats
{
    public OneStat CurrentHp { get; private set; }
    public OneStat MaxHp     { get; private set; }

    public OneStat Speed       { get; private set; }
    public OneStat Attack      { get; private set; }
    public OneStat Defence     { get; private set; }
    public OneStat AttackRange { get; private set; }

    public OneStat AdditionalSpeed       { get; private  set; }
    public OneStat AdditionalAttack      { get; private  set; }
    public OneStat AdditionalDefence     { get; private  set; }
    public OneStat AdditionalAttackRange { get; private  set; }

    public OneStat RemainingSpeed       { get; private set; }
    public OneStat RemainingAttack      { get; private set; }
    public OneStat RemainingDefence     { get; private set; }
    public OneStat RemainingAttackRange { get; private set; }

    public OneStat TotalMaxSpeed       { get; private set; }
    public OneStat TotalMaxAttack      { get; private set; }
    public OneStat TotalMaxDefence     { get; private set; }
    public OneStat TotalMaxAttackRange { get; private set; }


    #region ConstructorAndEvents
    public Stats(int hp, int speed, int attack, int defence, int attackRange, LevelManager levelManager)
    {
        MaxHp     = new OneStat(hp);
        CurrentHp = new OneStat(hp);

        Speed           = new OneStat(speed, "Basic");
        RemainingSpeed  = new OneStat(speed, "Remaining");
        AdditionalSpeed = new OneStat(0, "Additional");
        TotalMaxSpeed   = new OneStat(speed, "Total");

        Attack           = new OneStat(attack);
        RemainingAttack  = new OneStat(attack);
        AdditionalAttack = new OneStat(0);
        TotalMaxAttack   = new OneStat(attack);

        Defence           = new OneStat(defence);
        RemainingDefence  = new OneStat(defence);
        AdditionalDefence = new OneStat(0);
        TotalMaxDefence   = new OneStat(defence);
        
        AttackRange           = new OneStat(attackRange);
        RemainingAttackRange  = new OneStat(attackRange);
        AdditionalAttackRange = new OneStat(0);
        TotalMaxAttackRange   = new OneStat(attackRange);

        levelManager.TurnStateChanged += LevelManagerOnTurnStateChanged;

        Speed.ValueChanged += OnBasicOrAdditionalSpeedOnValueChanged;
        AdditionalSpeed.ValueChanged += OnBasicOrAdditionalSpeedOnValueChanged;
        TotalMaxSpeed.ValueChanged += OnTotalMaxSpeedChanged;

        Attack.ValueChanged += OnBasicOrAdditionalAttackOnValueChanged;
        AdditionalAttack.ValueChanged += OnBasicOrAdditionalAttackOnValueChanged;
        TotalMaxAttack.ValueChanged += OnTotalMaxAttackChanged;

        Defence.ValueChanged += OnBasicOrAdditionalDefenceOnValueChanged;
        AdditionalDefence.ValueChanged += OnBasicOrAdditionalDefenceOnValueChanged;
        TotalMaxDefence.ValueChanged += OnTotalMaxDefenceChanged;

        AttackRange.ValueChanged += OnBasicOrAdditionalAttackRangeOnValueChanged;
        AdditionalAttackRange.ValueChanged += OnBasicOrAdditionalAttackRangeOnValueChanged;
        TotalMaxAttackRange.ValueChanged += OnTotalMaxAttackRangeChanged;
    }

    private void OnBasicOrAdditionalSpeedOnValueChanged(int value)
    {
        TotalMaxSpeed.Value = Speed.Value + AdditionalSpeed.Value;
    }

    private void OnTotalMaxSpeedChanged(int value)
    {
        RemainingSpeed.Value = value;
    }

    private void OnBasicOrAdditionalAttackOnValueChanged(int value)
    {
        TotalMaxAttack.Value = Attack.Value + AdditionalAttack.Value;
    }

    private void OnTotalMaxAttackChanged(int value)
    {
        RemainingAttack.Value = value;
    }

    private void OnBasicOrAdditionalDefenceOnValueChanged(int value)
    {
        TotalMaxDefence.Value = Defence.Value + AdditionalDefence.Value;
    }

    private void OnTotalMaxDefenceChanged(int value)
    {
        RemainingDefence.Value = value;
    }

    private void OnBasicOrAdditionalAttackRangeOnValueChanged(int value)
    {
        TotalMaxAttackRange.Value = AttackRange.Value + AdditionalAttackRange.Value;
    }

    private void OnTotalMaxAttackRangeChanged(int value)
    {
        RemainingAttackRange.Value = value;
    }

    private void LevelManagerOnTurnStateChanged(TurnState turnState)
    {
        if (turnState == TurnState.NONE || turnState == TurnState.ENERGY || turnState == TurnState.LEVEL_WON)
        {
            ClearAdditionalStats();
        }
    }
    #endregion

    public bool TrySpendMovement(int points)
    {
        if (RemainingSpeed.Value < points)
        {
            Debug.Log($"The is not enough {nameof(RemainingSpeed)}");
            return false;
        }

        RemainingSpeed.Value -= points;
        return true;
    }

    public bool CanAttack(Stats enemyStats, int distanceToEnemy)
    {
        return enemyStats.RemainingDefence.Value <= RemainingAttack.Value && RemainingAttackRange.Value >= distanceToEnemy;
    }

    public int AttackEnemy(Stats enemyStats, bool useMaxAttackPoints = false)
    {
        if (enemyStats.RemainingDefence.Value > RemainingAttack.Value)
        {
            Debug.LogError($"Not enough {nameof(RemainingAttack)}");
        }

        int attackPointsUsed = enemyStats.RemainingDefence.Value;
        if (useMaxAttackPoints)
        {
            int attacksCount = RemainingAttack.Value / enemyStats.RemainingDefence.Value;
            attackPointsUsed = enemyStats.RemainingDefence.Value * attacksCount;
        }

        RemainingAttack.Value -= attackPointsUsed;
        return attackPointsUsed;
    }

    public void TakeDamage(int totalAttackPoints, out int totalDamage)
    {
        totalDamage = 0;
        if (RemainingDefence.Value > totalAttackPoints)
        {
            return;
        }

        totalDamage = Mathf.Min(totalAttackPoints / RemainingDefence.Value, CurrentHp.Value);
        CurrentHp.Value -= totalDamage;
    }

    public void SetAdditionalStat(StatType statType, int value)
    {
        switch (statType)
        {
            case StatType.SPEED  : AdditionalSpeed.Value   = value; break;
            case StatType.ATTACK : AdditionalAttack.Value  = value; break;
            case StatType.DEFENCE: AdditionalDefence.Value = value; break;

            default:
                Debug.LogError($"Cannot add additional value to {nameof(StatType)} {statType.ToString()}!");
                return;
        }
    }

    public void AddUpgradeToStat(StatType statType)
    {
        switch (statType)
        {
            case StatType.SPEED:        ++Speed.Value;       break;
            case StatType.ATTACK:       ++Attack.Value;      break;
            case StatType.DEFENCE:      ++Defence.Value;     break;
            case StatType.ATTACK_RANGE: ++AttackRange.Value; break;
        }
    }

    public void Heal()
    {
        CurrentHp.Value = MaxHp.Value;
    }

    public OneStat GetRemaining(StatType statType)
        => statType switch
           {
               StatType.HP           => CurrentHp,
               StatType.SPEED        => RemainingSpeed,
               StatType.ATTACK       => RemainingAttack,
               StatType.DEFENCE      => RemainingDefence,
               StatType.ATTACK_RANGE => RemainingAttackRange,

               _ => null
           };

    public OneStat GetTotalMax(StatType statType)
        => statType switch
           {
               StatType.HP           => MaxHp,
               StatType.SPEED        => TotalMaxSpeed,
               StatType.ATTACK       => TotalMaxAttack,
               StatType.DEFENCE      => TotalMaxDefence,
               StatType.ATTACK_RANGE => TotalMaxAttackRange,

               _ => null
           };

    private void ClearAdditionalStats()
    {
        AdditionalSpeed.Value       = 0;
        AdditionalAttack.Value      = 0;
        AdditionalDefence.Value     = 0;
        AdditionalAttackRange.Value = 0;
    }
}