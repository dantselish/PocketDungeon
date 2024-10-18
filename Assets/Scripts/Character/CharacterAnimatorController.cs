using UnityEditor.Animations;
using UnityEngine;

public class CharacterAnimationManager : MonoBehaviour
{
    [SerializeField] private Animator Animator;

    private static readonly int _attack      = Animator.StringToHash("Attack");
    private static readonly int _take_damage = Animator.StringToHash("TakeDamage");
    private static readonly int _block       = Animator.StringToHash("Block");
    private static readonly int _die         = Animator.StringToHash("Die");
    private static readonly int _move        = Animator.StringToHash("Move");
    private static readonly int _battle      = Animator.StringToHash("Battle");
    private static readonly int _heal        = Animator.StringToHash("Heal");
    private static readonly int _cheer       = Animator.StringToHash("Cheer");
    private static readonly int _reset       = Animator.StringToHash("Reset");


    public void Attack()
    {
        Animator.SetTrigger(_attack);
    }

    public void TakeDamage(int totalDamage)
    {
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
        Animator.SetTrigger(_heal);
    }

    public void Cheer()
    {
        Battle(false);
        Animator.SetTrigger(_cheer);
    }

    private void Reset()
    {
        Animator.SetTrigger(_reset);
    }
}