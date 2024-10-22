using System;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    public event Action AttackConnected;
    public event Action CheerEnded;
    public event Action HealEnded;


    public void DealDamage()
    {
        AttackConnected?.Invoke();
    }

    public void EndCheer()
    {
        CheerEnded?.Invoke();
    }

    public void EndHeal()
    {
        HealEnded?.Invoke();
    }
}
