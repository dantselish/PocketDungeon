using System;
using System.Collections.Generic;
using System.Linq;

public static class StatsExtensions
{
    public static List<StatType> GetAllStatTypes() => Enum.GetValues(typeof(StatType)).Cast<StatType>().Except(new [] {StatType.NONE}).ToList();

    public static List<StatType> GetEnergyUpgradableStatTypes() => GetAllStatTypes().Except(new [] {StatType.HP, StatType.ATTACK_RANGE}).ToList();

    public static List<StatType> GetUpgradableStatTypes() => GetAllStatTypes().Except(new [] {StatType.HP}).ToList();
}