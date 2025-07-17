using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public enum ToolTipDirection
{
    Above,
    Below,
    Left,
    Right,
}

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject tooltip;
    private TextMeshProUGUI tooltipText;
    [SerializeField] private string tooltipMessage;
    [SerializeField] private ToolTipDirection toolTipDirection;
    [SerializeField] private int buffer = 0;

    private GameObject tooltipObject;
    private RectTransform rectTransform;
    private Vector2 size;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        size = rectTransform.sizeDelta;
        tooltipObject = Instantiate(tooltip, transform);
        arrangeTooltip();
        tooltipText = tooltipObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        tooltipText.text = tooltipMessage;
        hideTooltip();
    }

    void arrangeTooltip()
    {
        switch (toolTipDirection)
        {
            case ToolTipDirection.Above:
                tooltipObject.transform.localPosition = new Vector3(0, size.y + buffer, 0);
                break;
            case ToolTipDirection.Below:
                tooltipObject.transform.localPosition = new Vector3(0, -size.y - buffer, 0);
                break;
            case ToolTipDirection.Left:
                tooltipObject.transform.localPosition = new Vector3(-size.x - buffer, 0, 0);
                break;
            case ToolTipDirection.Right:
                tooltipObject.transform.localPosition = new Vector3(size.x + buffer, 0, 0);
                break;
        }
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
        Debug.Log("Show ToolTip");
        tooltipObject.SetActive(true);
    }

    public void hideTooltip()
    {
        tooltipObject.SetActive(false);
    }
}