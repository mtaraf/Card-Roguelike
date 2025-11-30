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
    private DiscardUI discardUI;
    private bool discardInProgress = false;
    private Vector3 discardCardHoldPosition;

    private void Start()
    {
        handUIController = HandManager.instance.getHandUIContoller();
        centerOfUI = GameObject.FindGameObjectWithTag("CenterOfUI");

        originalIndex = transform.parent.gameObject.transform.GetSiblingIndex();
        StartCoroutine(getController());
    }

    private IEnumerator getController()
    {
        yield return null;
        sceneController = GameManager.instance.getCurrentSceneController();

        discardCardHoldPosition = centerOfUI.transform.position;
        discardCardHoldPosition.x -= 150;
        discardCardHoldPosition.y += 150;

        yield return new WaitForSeconds(0.5f);
        startPos = transform.position;
    }

    public void setNewStartPos(Vector3 newPos)
    {
        startPos = newPos;
    }

    // Dragging
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (discardInProgress)
        {
            return;
        }
        // bring card in front
        transform.SetAsLastSibling();
        HandManager.instance.setSelectedCard(gameObject);

        // Adjust card rotation offset
        // Quaternion targetRotation = Quaternion.Euler(0, 0, -transform.parent.transform.rotation.z);
        // transform.rotation = Quaternion.Slerp(transform.rotation,targetRotation,1);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (discardInProgress)
        {
            return;
        }
        // Move card with cursor
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (discardInProgress)
        {
            return;
        }

        Debug.Log(eventData.pointerEnter.tag);
        if (eventData.pointerEnter == null)
        {
            Debug.LogError("Error dragging card");
        }

        // If dropped somewhere invalid, return to start position
        if (eventData.pointerEnter.CompareTag("DragZone") && !sceneController.getDiscardInProgress())
        {
            checkCardUsageRequirements(eventData);
        }
        else if (eventData.pointerEnter.CompareTag("DiscardUI"))
        {
            Vector3 discardPileLocation = GameObject.FindGameObjectWithTag("DiscardPile").transform.position;
            sceneController.getDiscardUI().discardCard(HandManager.instance.getSelectedCard());

            // TO-DO: Change this to a different animation
            StartCoroutine(handUIController.animateCardPlayed(transform, discardPileLocation, transform.localScale, cardMoveSpeed));
        }
        else
        {
            transform.position = startPos;

            HandManager.instance.clearSelectedCard();
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
                        StartCoroutine(useCard(null, player, cardEnergy));
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
                            StartCoroutine(useCard(enemies, null, cardEnergy));
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
                                StartCoroutine(useCard(new List<Enemy> { enemy }, null, cardEnergy));
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
        if (!HandManager.instance.hasSelectedCard() && !discardInProgress)
        {
            eventData.selectedObject = gameObject;

            // Layer card infront of all others
            //transform.parent.gameObject.transform.SetAsLastSibling();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!HandManager.instance.hasSelectedCard())
        {
            eventData.selectedObject = null;

            // Set layering back to original
            //transform.parent.gameObject.transform.SetSiblingIndex(originalIndex);
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        // Card Animation
        handUIController.animateCardMovement(transform, startPos + new Vector3(0f, verticalMoveAmount, 0f), transform.localScale, cardHoverSpeed, null);
    }

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

    private IEnumerator useCard(List<Enemy> enemies, Player player, int cardEnergy)
    {
        // Check if discards needed
        Card card = HandManager.instance.getSelectedCard();
        int numDiscards = card.getCardsDiscarded();

        if (numDiscards > 0)
        {
            discardInProgress = true;

            // Move card above discard pile and save it for after this
            handUIController.animateCardMovement(transform, discardCardHoldPosition, transform.localScale, cardHoverSpeed, null);


            sceneController.startDiscard(numDiscards);
            discardUI = sceneController.getDiscardUI();

            if (discardUI == null)
            {
                Debug.LogError("Error retrieving discardUI component");
            }

            discardUI.setDiscardNum(numDiscards);

            yield return new WaitUntil(() => discardUI.getDiscardNum() == 0);
            discardUI.setInactive();
        }

        sceneController.setDiscardInProgress(false);

        HandManager.instance.setSelectedCard(gameObject);

        // Card animation
        StartCoroutine(handUIController.animateCardPlayed(transform, centerOfUI.transform.position, transform.localScale, cardMoveSpeed));

        // use card
        List<CardEffect> effects = HandManager.instance.useSelectedCard(enemies);
        if (player != null)
            player.processCardEffects(effects);
        else if (enemies != null)
        {
            foreach (Enemy enemy in enemies)
            {
                if (enemy.checkifTargetable())
                    enemy.processCardEffects(effects);
            }
        }

        // update player energy
        sceneController.usePlayerEnergy(cardEnergy);

        // Clear card
        HandManager.instance.clearSelectedCard();

        discardInProgress = false;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (!discardInProgress)
            handUIController.animateCardMovement(transform, startPos, transform.localScale, cardHoverSpeed, null);
    }

    public void setStartPos(Vector3 pos)
    {
        startPos = pos;
    }
    

}
