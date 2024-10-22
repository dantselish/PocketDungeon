using System;
using DG.Tweening;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Transform StartPos;

    public event Action Connected;

    public void Fly(Vector3 target, float duration)
    {
        transform.position = StartPos.position;
        gameObject.SetActive(true);
        transform.LookAt(target);
        transform.DOMove(target, duration).SetEase(Ease.Linear).onComplete += OnConnect;
    }

    private void OnConnect()
    {
        Connected?.Invoke();
        gameObject.SetActive(false);
    }
}
