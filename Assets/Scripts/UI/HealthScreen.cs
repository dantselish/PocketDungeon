using UnityEngine;

public class HealthScreen : MyMonoBehaviour
{
    [SerializeField] private HealButton HealButton;


    public void Init()
    {
        HealButton.Init(GM.LevelManager);
    }
}
