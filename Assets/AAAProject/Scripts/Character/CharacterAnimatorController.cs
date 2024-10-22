using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CharacterAnimationManager : MonoBehaviour
{
    [SerializeField] private Animator Animator;
    [SerializeField] private AnimationEvents AnimationEvents;
    [SerializeField] private List<GameObject> Weapons;
    [SerializeField] private GameObject Shield;
    [SerializeField] private GameObject HealBottle;

    private static readonly int _attack      = Animator.StringToHash("Attack");
    private static readonly int _take_damage = Animator.StringToHash("TakeDamage");
    private static readonly int _block       = Animator.StringToHash("Block");
    private static readonly int _die         = Animator.StringToHash("Die");
    private static readonly int _move        = Animator.StringToHash("Move");
    private static readonly int _battle      = Animator.StringToHash("Battle");
    private static readonly int _heal        = Animator.StringToHash("Heal");
    private static readonly int _cheer       = Animator.StringToHash("Cheer");
    private static readonly int _reset       = Animator.StringToHash("Reset");


    public void Init()
    {
        Subscribe();
    }

    private void Subscribe()
    {
        AnimationEvents.CheerEnded += () => HideShowWeaponsAndShield(true);
        AnimationEvents.HealEnded  += () => HideShowWeapons(true);
        AnimationEvents.HealEnded  += () => HideShowHealBottle(false);
    }

    public void Attack(Vector3 targetPos, Action onConnect = null)
    {
        SetLookAtTarget(targetPos);
        Animator.SetTrigger(_attack);

        if (onConnect != null)
        {
            AnimationEvents.AttackConnected += callback;

            void callback()
            {
                onConnect.Invoke();
                AnimationEvents.AttackConnected -= callback;
            }
        }
    }

    public void TakeDamage(int totalDamage, Vector3 attackerPos)
    {
        SetLookAtTarget(attackerPos);

        if (totalDamage > 0)
        {
            Animator.SetTrigger(_take_damage);
        }
        else
        {
            Animator.SetTrigger(_block);
        }
    }

    public void Die()
    {
        Animator.SetTrigger(_die);
    }

    public void Move(bool value)
    {
        Animator.SetBool(_move, value);
    }

    public void Battle(bool value)
    {
        Animator.SetBool(_battle, value);
    }

    public void Heal()
    {
        Battle(false);
        HideShowHealBottle(true);
        Animator.SetTrigger(_heal);
        HideShowWeapons(false);
    }

    public void Cheer()
    {
        Battle(false);
        Animator.SetTrigger(_cheer);
        HideShowWeaponsAndShield(false);
    }

    public void SetLookAtTarget(Vector3 lookTarget)
    {
        Rotate(lookTarget);
    }

    public void MoveTo(Vector3 position, float duration)
    {
        transform.DOMove(position, duration).SetEase(Ease.Linear);
    }

    private void Rotate(Vector3 target)
    {
        Vector3 lookAt = target;
        lookAt.y = transform.position.y;
        Animator.transform.DOLookAt(lookAt, 0.3f, ~AxisConstraint.Y);
    }

    private void Reset()
    {
        Animator.SetTrigger(_reset);
    }

    private void HideShowWeapons(bool show)
    {
        foreach (GameObject weapon in Weapons)
        {
            weapon.SetActive(show);
        }
    }

    private void HideShowShield(bool show)
    {
        if (Shield)
        {
            Shield.SetActive(show);
        }
    }

    private void HideShowWeaponsAndShield(bool show)
    {
        HideShowWeapons(show);
        HideShowShield(show);
    }

    private void HideShowHealBottle(bool show)
    {
        HealBottle.SetActive(show);
    }
}