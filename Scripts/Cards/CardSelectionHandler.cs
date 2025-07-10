using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CardSelectionHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, IPointerClickHandler
{
    [SerializeField] private float verticalMoveAmount = 300f;

    private Vector3 startPos;
    private Vector3 startScale;
    private int originalIndex = -1;

    private HandUIController handUIController;
    private GameObject centerOfUI;

    private float cardMoveSpeed = 0.4f;
    private float cardHoverSpeed = 0.1f;
    private float cardFadeSpeed = 0.3f;

    private void Start()
    {
        handUIController = HandManager.instance.getHandUIContoller();
        centerOfUI = GameObject.FindGameObjectWithTag("CenterOfUI");

        startPos = transform.position;
        startScale = transform.localScale;
        originalIndex = transform.parent.gameObject.transform.GetSiblingIndex();
    }

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

    public void OnPointerClick(PointerEventData eventData)
    {
        HandManager.instance.setSelectedCard(gameObject);
        eventData.selectedObject = gameObject;
        transform.parent.gameObject.transform.SetAsLastSibling();
    }

    public void OnSelect(BaseEventData eventData)
    {
        // Card Animation
        handUIController.animateCardMovement(transform, startPos + new Vector3(0f, verticalMoveAmount, 0f), transform.localScale, cardHoverSpeed, null);
    }

    private GameObject getGameObjectedClicked()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Mouse.current.position.ReadValue()
        };

        // Raycast UI
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        if (results.Count > 0)
        {
            GameObject clickedUI = results[0].gameObject;
            return findParent(clickedUI);
        }

        return null;
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

    // if a card is selected, use card on clicked object if possible 
    public void OnDeselect(BaseEventData eventData)
    {
        GameObject clickedGameObject;
        if (HandManager.instance.hasSelectedCard())
        {
            clickedGameObject = getGameObjectedClicked();
            if (clickedGameObject != null)
            {
                Player player = clickedGameObject.GetComponent<Player>();
                if (player != null && player.checkifTargetable())
                {
                    // Check if player has enough energy to use the card
                    int cardEnergy = HandManager.instance.getSelectedCard().getCardModel().energy;
                    if (BaseLevelSceneController.instance.getCurrentPlayerEnergy() >= cardEnergy)
                    {
                        // Use card
                        Debug.Log(HandManager.instance.getSelectedCard().getCardModel().name + " used on player");
                        List<CardEffect> effects = HandManager.instance.useSelectedCard();
                        player.processCardEffects(effects);

                        // update player energy
                        //GameManager.instance.usePlayerEnergy(cardEnergy);
                        BaseLevelSceneController.instance.usePlayerEnergy(cardEnergy);

                        // card animation
                        StartCoroutine(handUIController.animateCardPlayed(transform, centerOfUI.transform.position, transform.localScale, cardMoveSpeed));
                        

                        HandManager.instance.clearSelectedCard();
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
                        List<Enemy> enemies = BaseLevelSceneController.instance.getEnemies();
                        int cardEnergy = HandManager.instance.getSelectedCard().getCardModel().energy;
                        if (BaseLevelSceneController.instance.getCurrentPlayerEnergy() < cardEnergy)
                        {
                            // If player does not have energy for the card, but tries to play, display feedback message
                            StartCoroutine(HandManager.instance.displayFeedbackMessage("Not enough energy!"));
                        }
                        else
                        {
                            // Use card
                            Debug.Log(HandManager.instance.getSelectedCard().getCardModel().name + " used on all enemies");
                            List<CardEffect> effects = HandManager.instance.useSelectedCard();

                            foreach (Enemy enemy in enemies)
                            {
                                if (enemy.checkifTargetable())
                                {
                                    enemy.processCardEffects(effects);
                                }
                            }

                            // update player energy
                            BaseLevelSceneController.instance.usePlayerEnergy(cardEnergy);

                            // card animation
                            StartCoroutine(handUIController.animateCardPlayed(transform, centerOfUI.transform.position, transform.localScale, cardMoveSpeed));

                            HandManager.instance.clearSelectedCard();
                        }
                    }
                    else
                    {
                        Enemy enemy = clickedGameObject.GetComponent<Enemy>();
                        if (enemy != null && enemy.checkifTargetable())
                        {
                            // Check if player has enough energy to use the card
                            int cardEnergy = HandManager.instance.getSelectedCard().getCardModel().energy;
                            if (BaseLevelSceneController.instance.getCurrentPlayerEnergy() >= cardEnergy)
                            {
                                // Use card
                                Debug.Log(HandManager.instance.getSelectedCard().getCardModel().name + " used on enemy");
                                List<CardEffect> effects = HandManager.instance.useSelectedCard();
                                enemy.processCardEffects(effects);

                                // update player energy
                                //GameManager.instance.usePlayerEnergy(cardEnergy);
                                BaseLevelSceneController.instance.usePlayerEnergy(cardEnergy);

                                // card animation
                                StartCoroutine(handUIController.animateCardPlayed(transform, centerOfUI.transform.position, transform.localScale, cardMoveSpeed));

                                HandManager.instance.clearSelectedCard();
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

    public void setStartPos(Vector3 pos)
    {
        startPos = pos;
    }
}
