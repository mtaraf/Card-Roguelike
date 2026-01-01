using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VictoryCardChoice : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerClickHandler
{
    Card card;

    void Start()
    {
        card = gameObject.GetComponent<Card>();
        if (card == null)
            Debug.LogError($"Could not find selected card in Victory Card Choice");
    }

    public void OnSelect(BaseEventData eventData)
    {
        GameManager.instance.setVictoryCardSelection(card);
        card.toggleCardOutline(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        PointerEventData pointerData = eventData as PointerEventData;
        if (pointerData != null)
        {
            GameObject clickedObj = pointerData.pointerCurrentRaycast.gameObject;
            Debug.Log(clickedObj.name);

            if (clickedObj != null && clickedObj.name == "Continue")
            {
                Debug.Log("Blocked deselection because " + clickedObj.name + " was clicked");
                return;
            }
        }

        card.toggleCardOutline(false);
        GameManager.instance.setVictoryCardSelection(null);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        eventData.selectedObject = gameObject;
    }
}