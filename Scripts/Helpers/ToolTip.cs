using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject tooltip;
    private TextMeshProUGUI tooltipText;
    [SerializeField] private string tooltipMessage;

    private void Awake()
    {
        tooltipText = tooltip.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        tooltipText.text = tooltipMessage;
        hideTooltip();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        showTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hideTooltip();
    }

    public void showTooltip()
    {
        tooltip.SetActive(true);
    }

    public void hideTooltip()
    {
        tooltip.SetActive(false);
    }
}