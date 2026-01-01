using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardSelectionHandler : MonoBehaviour,
IPointerEnterHandler,
IPointerExitHandler,
IPointerClickHandler,
ISelectHandler,
IDeselectHandler,
IBeginDragHandler,
IDragHandler,
IEndDragHandler
{
    [SerializeField] private float verticalMoveAmount = 300f;
    [SerializeField] private GameObject cardTooltip;

    private Vector3 startPos;
    private int originalIndex = -1;

    private HandUIController handUIController;
    private GameObject centerOfUI;
    private GameObject discardPile;

    private float cardMoveSpeed = 0.6f;
    private float cardHoverSpeed = 0.1f;

    private ParentSceneController sceneController;
    private bool toggledForDiscard = false;
    private string tooltipDescription = "";

    private void Start()
    {
        discardPile = GameObject.FindGameObjectWithTag("DiscardPile");
        handUIController = HandManager.instance.getHandUIContoller();
        centerOfUI = GameObject.FindGameObjectWithTag("CenterOfUI");

        originalIndex = transform.parent.gameObject.transform.GetSiblingIndex();

        tooltipDescription = EffectTooltipFactory.createCardTooltipDescription(gameObject.GetComponent<Card>().getCardEffects());

        if (tooltipDescription != "")
            cardTooltip.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = tooltipDescription;

        cardTooltip.SetActive(false);
        StartCoroutine(getController());
    }

    private IEnumerator getController()
    {
        yield return null;
        sceneController = GameManager.instance.getCurrentSceneController();

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
        if (HandManager.instance.getDiscardInProgress())
            return;

        // Hide tooltip
        cardTooltip.SetActive(false);

        // bring card in front
        transform.SetAsLastSibling();
        HandManager.instance.setSelectedCard(gameObject);

        // Adjust card rotation offset
        // Quaternion targetRotation = Quaternion.Euler(0, 0, -transform.parent.transform.rotation.z);
        // transform.rotation = Quaternion.Slerp(transform.rotation,targetRotation,1);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (HandManager.instance.getDiscardInProgress())
            return;

        // Move card with cursor
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (HandManager.instance.getDiscardInProgress())
            return;

        Debug.Log(eventData.pointerEnter.tag);
        Debug.Log(eventData.pointerEnter.gameObject.name);
        Debug.Log(HandManager.instance.getDiscardInProgress());
        if (eventData.pointerEnter == null)
        {
            Debug.LogError("Error dragging card");
        }

        // If dropped somewhere invalid, return to start position
        if (eventData.pointerEnter.CompareTag("DragZone") && !HandManager.instance.getDiscardInProgress())
        {
            checkCardUsageRequirements(eventData);
        }
        else
        {
            transform.position = startPos;

            // Set layering back to original
            transform.SetSiblingIndex(originalIndex);
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

    // (hover cards)
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (HandManager.instance.getDiscardInProgress())
            return;

        // Show tooltip is needed
        if (tooltipDescription != "")
        {
            cardTooltip.SetActive(true);
        }

        // Only allow card animation if a card is not selected for playing
        if (!HandManager.instance.hasSelectedCard())
        {
            eventData.selectedObject = gameObject;

            // Layer card infront of all others
            transform.SetAsLastSibling();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (HandManager.instance.getDiscardInProgress())
            return;


        // Hide tooltip
        cardTooltip.SetActive(false);

        if (!HandManager.instance.hasSelectedCard())
        {
            eventData.selectedObject = null;

            // Set layering back to original
            transform.SetSiblingIndex(originalIndex);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!HandManager.instance.getDiscardInProgress())
            return;

        Debug.Log("Pointer Click");
        if (!DiscardManager.instance.maxCardsSelected() || toggledForDiscard)
            toggleCardForDiscard();
    }

    public void OnSelect(BaseEventData eventData)
    {
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

    private void useCard(List<Enemy> enemies, Player player, int cardEnergy)
    {
        // Check if discards needed
        Card card = HandManager.instance.getSelectedCard();
        int numDiscards = card.getCardsDiscarded();

        if (numDiscards > 0)
        {
            Debug.Log("Discard started");
            DiscardManager.instance.startDiscard(numDiscards, gameObject, enemies, player);
            return;
        }

        continueCardUse(card, enemies, player, cardEnergy, false);
    }

    public void toggleCardForDiscard()
    {
        Card card = gameObject.GetComponent<Card>();
        DiscardManager.instance.toggleCard(card);
        if (toggledForDiscard)
        {
            toggledForDiscard = false;
            handUIController.animateCardMovement(transform, startPos, transform.localScale, cardHoverSpeed, null);
            card.toggleCardOutline(false);
        }
        else
        {
            toggledForDiscard = true;
            handUIController.animateCardMovement(transform, startPos + new Vector3(0f, verticalMoveAmount, 0f), transform.localScale, cardHoverSpeed, null);

            // Highlight outline of card
            card.toggleCardOutline(true);
        }
    }

    public void continueCardUse(Card card, List<Enemy> enemies, Player player, int cardEnergy, bool fromDiscard)
    {
        HandManager.instance.setSelectedCard(gameObject);

        // Card animation
        StartCoroutine(playCardAnimation(card));

        // use card
        List<CardEffect> effects = HandManager.instance.useSelectedCard(enemies);

        if (player != null)
            player.processCardEffects(effects);
        else if (enemies != null)
        {
            foreach (Enemy enemy in enemies)
            {
                if (enemy.checkifTargetable() || fromDiscard)
                    enemy.processCardEffects(effects);
            }
        }

        // update player energy
        sceneController.usePlayerEnergy(cardEnergy);

        // Clear card
        HandManager.instance.clearSelectedCard();
    }

    public IEnumerator playCardAnimation(Card card)
    {
        if (card.isLithe())
        {
            yield return StartCoroutine(handUIController.animateLitheCardPlayed(transform, centerOfUI.transform.position, transform.localScale, cardMoveSpeed, () => handUIController.removeCardFromHand(card)));
        }
        else
        {
            yield return StartCoroutine(handUIController.animateCardPlayed(transform, discardPile.transform.position, transform.localScale, cardMoveSpeed, () => handUIController.removeCardFromHand(card)));
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        handUIController.animateCardMovement(transform, startPos, transform.localScale, cardHoverSpeed, null);
    }

    public void setStartPos(Vector3 pos)
    {
        startPos = pos;
    }

    public void returnToStartPos()
    {
        handUIController.animateCardMovement(transform, startPos, transform.localScale, cardHoverSpeed, null);
    }

    public void moveCardToPosition(Vector3 target)
    {
        handUIController.animateCardMovement(transform, target, transform.localScale, cardHoverSpeed, null);
    }

    public void setToggledForDiscard(bool toggled)
    {
        toggledForDiscard = toggled;
        gameObject.GetComponent<Card>().toggleCardOutline(false);
    }
}
