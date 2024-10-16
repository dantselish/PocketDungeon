using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DebugDiceResult : MyMonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private TMP_Text Text;
    [SerializeField] private Image Image;

    public int Value { get; private set; }


    public void UpdateValue(int value)
    {
        Value = value;
        SetText(value.ToString());
    }

    private void SetText(string text)
    {
        Text.SetText(text);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Image.raycastTarget = true;
    }
}
