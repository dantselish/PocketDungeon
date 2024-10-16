using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroHPUI : MonoBehaviour
{
    [SerializeField] private HeartHPUI HeartHPUIPrefab;
    [SerializeField] private LayoutGroup HeartsContainer;

    private List<HeartHPUI> _hearts;


    public void Init(Stats heroStats)
    {
        _hearts = new List<HeartHPUI>();

        for (int i = 0; i < heroStats.MaxHp.Value; i++)
        {
            HeartHPUI go = Instantiate(HeartHPUIPrefab, HeartsContainer.transform);
            go.SetState(true);
            _hearts.Add(go);
        }

        heroStats.CurrentHp.ValueChanged += CurrentHpOnValueChanged;
    }

    private void CurrentHpOnValueChanged(int newValue)
    {
        for (int i = 0; i < _hearts.Count; i++)
        {
            _hearts[i].SetState(i < newValue);
        }
    }
}
