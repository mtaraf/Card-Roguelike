using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CardSelectionHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, IPointerClickHandler
{
    [SerializeField] private float verticalMoveAmount = 300f;
    [SerializeField] private float moveTime = 0.1f;
    [Range(0f, 2f), SerializeField] private float scaleAmount = 1.1f;

    private Vector3 startPos;
    private Vector3 startScale;
    private int originalIndex = -1;

    private void Start()
    {
        startPos = transform.position;
        startScale = transform.localScale;
        originalIndex = transform.parent.gameObject.transform.GetSiblingIndex();
    }

    private IEnumerator moveCard(bool startingAnimation)
    {
        Vector3 endPos;
        Vector3 endScale;

        float elapsedTime = 0f;
        while (elapsedTime < moveTime)
        {
            elapsedTime += Time.deltaTime;

            if (startingAnimation)
            {
                endPos = startPos + new Vector3(0f, verticalMoveAmount, 0f);
                endScale = startScale * scaleAmount;
            }
            else
            {
                endPos = startPos;
                endScale = startScale;
            }

            // Lerppppp
            Vector3 lerpPos = Vector3.Lerp(transform.position, endPos, elapsedTime / moveTime);
            Vector3 lerpScale = Vector3.Lerp(transform.localScale, endScale, elapsedTime / moveTime);

            transform.position = lerpPos;
            transform.localScale = lerpScale;

            yield return null;
        }
    }

    // Move card to center of screen then fade out when card is used
    private IEnumerator onUseCard()
    {
        // Move time of the card
        moveTime = 0.2f;

        // Move card to center of screen (UI space)
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        Vector3 worldCenter;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            transform.parent as RectTransform, screenCenter, null, out worldCenter);

        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = worldCenter;

        Vector3 startScale = transform.localScale;
        Vector3 endScale = startScale * 1.2f;

        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // Step 1: Move and scale
        while (elapsedTime < moveTime)
        {
            float t = elapsedTime / moveTime;
            transform.position = Vector3.Lerp(startPosition, endPosition, t);
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final position and scale are exact
        transform.position = endPosition;
        transform.localScale = endScale;

        // Step 2: Fade out
        float fadeDuration = 0.3f;
        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            float t = elapsedTime / fadeDuration;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Destroy the game object after it fades out
        Destroy(gameObject);
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
        StartCoroutine(moveCard(true));
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
                        StartCoroutine(onUseCard());

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
                            StartCoroutine(onUseCard());

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
                                StartCoroutine(onUseCard());

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

                    // Enemy enemy = clickedGameObject.GetComponent<Enemy>();
                    // if (enemy != null && enemy.checkifTargetable())
                    // {
                    //     // Check if player has enough energy to use the card
                    //     int cardEnergy = HandManager.instance.getSelectedCard().getCardModel().energy;
                    //     if (GameManager.instance.getCurrentPlayerEnergy() >= cardEnergy)
                    //     {
                    //         // Use card
                    //         Debug.Log(HandManager.instance.getSelectedCard().getCardModel().name + " used on enemy");
                    //         List<CardEffect> effects = HandManager.instance.useSelectedCard();
                    //         enemy.processCardEffects(effects);

                    //         // update player energy
                    //         //GameManager.instance.usePlayerEnergy(cardEnergy);
                    //         BaseLevelSceneController.instance.usePlayerEnergy(cardEnergy);

                    //         // card animation
                    //         StartCoroutine(onUseCard());

                    //         HandManager.instance.clearSelectedCard();
                    //         return;
                    //     }
                    //     else
                    //     {
                    //         // If player does not have energy for the card, but tries to play, display feedback message
                    //         StartCoroutine(HandManager.instance.displayFeedbackMessage("Not enough energy!"));
                    //     }
                    // }
                }
            }
        }

        // Card Animation
        StartCoroutine(moveCard(false));

        // Remove from game manager and reorder
        HandManager.instance.clearSelectedCard();
        transform.parent.gameObject.transform.SetSiblingIndex(originalIndex);

    }
}
