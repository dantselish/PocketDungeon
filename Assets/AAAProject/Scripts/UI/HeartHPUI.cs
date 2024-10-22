using UnityEngine;
using UnityEngine.UI;

public class HeartHPUI : MonoBehaviour
{
    [SerializeField] private Image FullImage;
    [SerializeField] private Image EmptyImage;


    public void SetState(bool isFull)
    {
        FullImage.gameObject.SetActive(isFull);
        EmptyImage.gameObject.SetActive(!isFull);
    }
}
