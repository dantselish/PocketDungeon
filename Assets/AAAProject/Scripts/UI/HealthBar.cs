using DG.Tweening;
using TMPro;
using UnityEngine;

public class HealthBar : MyMonoBehaviour
{
    [SerializeField] private TMP_Text HealthText;
    [SerializeField] private TMP_Text SpeedText;
    [SerializeField] private TMP_Text AttackText;
    [SerializeField] private TMP_Text DefenceText;
    [SerializeField] private TMP_Text AttackRangeText;


    public void Init(Stats stats)
    {
        HealthText.SetText(stats.CurrentHp.Value.ToString());
        SpeedText.SetText(stats.RemainingSpeed.Value.ToString());
        AttackText.SetText(stats.RemainingAttack.Value.ToString());
        DefenceText.SetText(stats.RemainingDefence.Value.ToString());
        AttackRangeText.SetText(stats.RemainingAttackRange.Value.ToString());
    }

    private void Update()
    {
        Vector3 lookPosition = Camera.main.transform.position;
        lookPosition.x = transform.position.x;
        Vector3 euler = transform.rotation.eulerAngles;
        transform.LookAt(lookPosition);
        //transform.rotation = Quaternion.Euler(euler);
    }

    public void OnHealthChanged(int newValue)
    {
        HealthText.SetText(newValue.ToString());
    }

    public void OnSpeedChanged(int newValue)
    {
        SpeedText.SetText(newValue.ToString());
    }

    public void OnAttackChanged(int newValue)
    {
        AttackText.SetText(newValue.ToString());
    }

    public void OnDefenceChanged(int newValue)
    {
        DefenceText.SetText(newValue.ToString());
    }

    public void OnAttackRangeChanged(int newValue)
    {
        AttackRangeText.SetText(newValue.ToString());
    }
}
