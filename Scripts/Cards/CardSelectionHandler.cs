using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CardSelectionHandler : MonoBehaviour,
IPointerEnterHandler,
IPointerExitHandler,
ISelectHandler,
IDeselectHandler,
IBeginDragHandler,
IDragHandler,
IEndDragHandler
{
    [SerializeField] private float verticalMoveAmount = 300f;

    private Vector3 startPos;
    private int originalIndex = -1;

    private HandUIController handUIController;
    private GameObject centerOfUI;

    private float cardMoveSpeed = 0.4f;
    private float cardHoverSpeed = 0.1f;

    private ParentSceneController sceneController;

    private void Start()
    {
        handUIController = HandManager.instance.getHandUIContoller();
        centerOfUI = GameObject.FindGameObjectWithTag("CenterOfUI");

        startPos = transform.position;
        originalIndex = transform.parent.gameObject.transform.GetSiblingIndex();
        StartCoroutine(getController());
    }

    private IEnumerator getController()
    {
        yield return null;
        sceneController = GameManager.instance.getCurrentSceneController();
    }

    // Dragging
    public void OnBeginDrag(PointerEventData eventData)
    {
        // bring card in front
        transform.SetAsLastSibling();
        HandManager.instance.setSelectedCard(gameObject);

        // Adjust card rotation offset
        // Quaternion targetRotation = Quaternion.Euler(0, 0, -transform.parent.transform.rotation.z);
        // transform.rotation = Quaternion.Slerp(transform.rotation,targetRotation,1);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Move card with cursor
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log(eventData.pointerEnter.tag);

        // If dropped somewhere invalid, return to start position
        if (eventData.pointerEnter == null || !eventData.pointerEnter.CompareTag("DragZone"))
        {
            transform.position = startPos;

            HandManager.instance.clearSelectedCard();
        }
        else
        {
            checkCardUsageRequirements(eventData);
        }
    }

    private void checkCardUsageRequirements(PointerEventData eventData)
    {
        // Get parent object
        GameObject parentObj = eventData.pointerEnter.transform.parent.transform.parent.gameObject;

        if (HandManager.instance.hasSelectedCard())
        {
            if (parentObj != null)
            {
                Player player = parentObj.GetComponent<Player>();
                if (player != null && player.checkifTargetable())
                {
                    // Check if player has enough energy to use the card
                    int cardEnergy = HandManager.instance.getSelectedCard().getCardModel().energy;
                    if (sceneController.getCurrentPlayerEnergy() >= cardEnergy)
                    {
                        useCard(null, player, cardEnergy);
                        return;
                    }
                    else
                    {
                        // If player does not have energy for the card, but tries to play, display feedback message
                        StartCoroutine(HandManager.instance.displayFeedbackMessage("Not enough energy!"));
                    }
                }
                else
                {
                    Card selectedCardModel = HandManager.instance.getSelectedCard();
                    if (selectedCardModel.getCardTarget() == Target.Enemy_Multiple)
                    {
                        List<Enemy> enemies = sceneController.getEnemies();
                        int cardEnergy = HandManager.instance.getSelectedCard().getCardModel().energy;
                        if (sceneController.getCurrentPlayerEnergy() < cardEnergy)
                        {
                            // If player does not have energy for the card, but tries to play, display feedback message
                            StartCoroutine(HandManager.instance.displayFeedbackMessage("Not enough energy!"));
                        }
                        else
                        {
                            useCard(enemies, null, cardEnergy);
                            return;
                        }
                    }
                    else
                    {
                        Enemy enemy = parentObj.GetComponent<Enemy>();
                        if (enemy != null && enemy.checkifTargetable())
                        {
                            // Check if player has enough energy to use the card
                            int cardEnergy = HandManager.instance.getSelectedCard().getCardModel().energy;
                            if (sceneController.getCurrentPlayerEnergy() >= cardEnergy)
                            {
                                useCard(new List<Enemy> { enemy }, null, cardEnergy);
                                return;
                            }
                            else
                            {
                                // If player does not have energy for the card, but tries to play, display feedback message
                                StartCoroutine(HandManager.instance.displayFeedbackMessage("Not enough energy!"));
                            }
                        }
                    }
                }
            }
        }

        // Card Animation
        handUIController.animateCardMovement(transform, startPos, transform.localScale, cardHoverSpeed, null);

        // Remove from game manager and reorder
        HandManager.instance.clearSelectedCard();
        transform.parent.gameObject.transform.SetSiblingIndex(originalIndex);
    }

    // // Selecting (hover cards)
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Only allow card animation if a card is not selected for playing
        if (!HandManager.instance.hasSelectedCard())
        {
            eventData.selectedObject = gameObject;

            // Layer card infront of all others
            transform.parent.gameObject.transform.SetAsLastSibling();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!HandManager.instance.hasSelectedCard())
        {
            eventData.selectedObject = null;

            // Set layering back to original
            transform.parent.gameObject.transform.SetSiblingIndex(originalIndex);
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        // Card Animation
        handUIController.animateCardMovement(transform, startPos + new Vector3(0f, verticalMoveAmount, 0f), transform.localScale, cardHoverSpeed, null);
    }

    // private GameObject getGameObjectedClicked()
    // {
    //     PointerEventData pointerData = new PointerEventData(EventSystem.current)
    //     {
    //         position = Mouse.current.position.ReadValue()
    //     };

    //     // Raycast UI
    //     List<RaycastResult> results = new List<RaycastResult>();
    //     EventSystem.current.RaycastAll(pointerData, results);

    //     if (results.Count > 0)
    //     {
    //         GameObject clickedUI = results[0].gameObject;
    //         return findParent(clickedUI);
    //     }

    //     return null;
    // }

    private GameObject findParent(GameObject obj)
    {
        if (obj.transform.parent == null || obj.transform.parent.gameObject.CompareTag("BaseLevelCanvas"))
        {
            return null;
        }
        else if (obj.transform.parent.gameObject.CompareTag("Player") || obj.transform.parent.gameObject.CompareTag("Enemy"))
        {
            return obj.transform.parent.gameObject;
        }
        else
        {
            return findParent(obj.transform.parent.gameObject);
        }
    }

    private void useCard(List<Enemy> enemies, Player player, int cardEnergy)
    {
        // Card animation
        StartCoroutine(handUIController.animateCardPlayed(transform, centerOfUI.transform.position, transform.localScale, cardMoveSpeed));

        // Check if discards needed

        // use card
        if (player == null)
        {
            List<CardEffect> effects = HandManager.instance.useSelectedCard(enemies);

            foreach (Enemy enemy in enemies)
            {
                if (enemy.checkifTargetable())
                {
                    enemy.processCardEffects(effects);
                }
            }
        }
        else
        {
            List<CardEffect> effects = HandManager.instance.useSelectedCard(null);
            player.processCardEffects(effects);
        }

        // update player energy
        sceneController.usePlayerEnergy(cardEnergy);

        // Clear card
        HandManager.instance.clearSelectedCard();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        handUIController.animateCardMovement(transform, startPos, transform.localScale, cardHoverSpeed, null);
    }

    public void setStartPos(Vector3 pos)
    {
        startPos = pos;
    }
}
