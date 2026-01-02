using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Sprite buttonImage;
    private Sprite hoverButtonImage;
    private Button button;
    private Image image;
    private Action hoverEnterAction = null;
    private Action hoverExitAction = null;

    void Awake()
    {
        buttonImage = Resources.Load<Sprite>("UI/Art/background/button_background_1");
        hoverButtonImage = Resources.Load<Sprite>("UI/Art/background/button_hover_1");

        image = GetComponent<Image>();
        button = GetComponent<Button>();

        image.sprite = buttonImage;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button.interactable == false)
            return;

        transform.localScale = new Vector3(1.2f, 1.2f, 1.0f);
        image.sprite = hoverButtonImage;
        if (hoverEnterAction != null)
        {
            hoverEnterAction();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (button.interactable == false)
            return;

        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        image.sprite = buttonImage;
        if (hoverExitAction != null)
        {
            hoverExitAction();
        }
    }

    public void setButtonFunction(Action action)
    {
        button.onClick.RemoveAllListeners();
        if (action != null)
        {
            button.onClick.AddListener(() => action());
        }
    }

    public void setButtonText(string buttonText)
    {
        button.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = buttonText;
    }

    public void setHoverEnterAction(Action action)
    {
        hoverEnterAction = action;
    }

    public void setHoverExitAction(Action action)
    {
        hoverExitAction = action;
    }
}
