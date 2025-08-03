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

public enum ToolTipSize
{
    Large,
    Medium,
    Small
}

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject tooltip;
    private TextMeshProUGUI tooltipDescriptionComponent;
    private TextMeshProUGUI tooltipTitleComponent;
    [SerializeField] private string tooltipMessage;
    [SerializeField] private string tooltipTitle;
    [SerializeField] private ToolTipSize tooltipSize;
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
        tooltipTitleComponent = tooltipObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        tooltipTitleComponent.text = tooltipTitle;
        
        if (tooltipSize == ToolTipSize.Large)
        {
            tooltipDescriptionComponent = tooltipObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            tooltipDescriptionComponent.text = tooltipMessage;
        }
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

    public void setTooltipData(string title, string message)
    {
        tooltipTitle = title;
        tooltipMessage = message;
        tooltipTitleComponent.text = tooltipTitle;
        tooltipDescriptionComponent.text = tooltipMessage;
    }
}