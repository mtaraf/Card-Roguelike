using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardInteractions : MonoBehaviour//, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button removalButton;
    private GameObject cardObj;
    private Card card;
    private Transform cardTransform;
    private Vector3 cardStartingPosition;
    private Vector3 cardAnimationEndingPosition;
    private int animationDistance = 50;
    private HoverButton upgradeHoverButton;
    private CardModelSO currentCard;

    void Awake()
    {
        cardObj = transform.GetChild(0).gameObject;
        card = cardObj.GetComponent<Card>();
        cardTransform = cardObj.transform;

        if (card == null || cardObj == null)
        {
            Debug.LogError("Could not find card in CardInteractions");
        }

        if (upgradeButton == null || removalButton == null)
        {
            Debug.LogError("Upgrade/Removal button for a card in the forge scene is null");
        }

        upgradeHoverButton = upgradeButton.GetComponent<HoverButton>();
    }

    private void showUpgradedCardOnHover() {
        CardModelSO upgradedCard = card.getUpgradedCard();

        card.setCardDisplayInformation(upgradedCard);
    }

    // void getUpdatedPositions()
    // {
    //     cardStartingPosition = cardTransform.position;
    //     cardAnimationEndingPosition = new Vector3(cardStartingPosition.x, cardStartingPosition.y + animationDistance, cardStartingPosition.z);
    // }

    // public void OnPointerEnter(PointerEventData eventData)
    // {
    //     //getUpdatedPositions();
    //     //StartCoroutine(cardAnimation(0.2f, cardAnimationEndingPosition));
    // }

    // public void OnPointerExit(PointerEventData eventData)
    // {
    //     //StartCoroutine(cardAnimation(0.2f, cardStartingPosition));
    // }

    // IEnumerator cardAnimation(float duration, Vector3 targetPos, Action onComplete = null)
    // {
    //     Vector3 startPos = cardTransform.position;
    //     float elapsedTime = 0f;

    //     while (elapsedTime < duration)
    //     {
    //         if (cardTransform == null)
    //         {
    //             yield break;
    //         }

    //         float t = elapsedTime / duration;
    //         cardTransform.position = Vector3.Lerp(startPos, targetPos, t);
    //         elapsedTime += Time.deltaTime;
    //         yield return null;
    //     }

    //     cardTransform.position = targetPos;
    //     onComplete?.Invoke();
    // }

    public void fillCardInformation(CardModelSO model)
    {
        currentCard = model;
        card.setCardDisplayInformation(model);

        if (model.upgradedCard == null)
        {
            upgradeHoverButton.setButtonText("Fully Upgraded");
        }
        else
        {
            upgradeHoverButton.setHoverEnterAction(() => showUpgradedCardOnHover());
            upgradeHoverButton.setHoverExitAction(() => fillCardInformation(currentCard));
        }
    }
}
