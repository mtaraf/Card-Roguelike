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
    private HoverButton upgradeHoverButton;
    private HoverButton removalHoverButton;
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
        removalHoverButton = removalButton.GetComponent<HoverButton>();
    }

    private void showUpgradedCardOnHover()
    {
        CardModelSO upgradedCard = card.getUpgradedCard();
        card.setCardDisplayInformation(upgradedCard);
    }

    public IEnumerator cardUpgradeAnimation(Action onComplete = null)
    {
        float duration = 1.0f;
        float tiltSpeed = 5.0f;
        float tiltAngle = 15f;

        Quaternion startRotation = cardTransform.rotation;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (cardTransform == null)
            {
                yield break;
            }

            float zRotation = Mathf.Sin(elapsedTime * tiltSpeed) * tiltAngle;
            cardTransform.rotation = Quaternion.Euler(0f, 0f, zRotation);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // flip
        cardTransform.rotation = Quaternion.Euler(0f, 90f, 0f);

        onComplete?.Invoke();

        // Rotate back to original rotation
        elapsedTime = 0f;
        duration = 0.5f;
        while (elapsedTime < duration)
        {
            if (cardTransform == null) yield break;

            cardTransform.rotation = Quaternion.Lerp(
                Quaternion.Euler(0f, 90f, 0f),
                startRotation,
                elapsedTime / duration
            );

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cardTransform.rotation = startRotation;
    }

    public IEnumerator cardRemovalAnimation(Action onComplete = null)
    {
        float duration = 1.0f;
        float tiltSpeed = 5.0f;
        float tiltAngle = 15f;
        CanvasGroup cg = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();


        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            if (cardTransform == null)
            {
                yield break;
            }
            cg.alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            float zRotation = Mathf.Sin(elapsedTime * tiltSpeed) * tiltAngle;
            cardTransform.rotation = Quaternion.Euler(0f, 0f, zRotation);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cg.alpha = 0f;

        // destroy obj
        onComplete?.Invoke();
    }

    public void fillCardInformation(CardModelSO model)
    {
        currentCard = model;
        card.setCardDisplayInformation(model);

        if (model.upgradedCard == null)
        {
            upgradeHoverButton.setButtonText("Fully Upgraded");
            upgradeHoverButton.setHoverEnterAction(null);
            upgradeHoverButton.setHoverExitAction(null);
            upgradeHoverButton.setButtonFunction(null);
        }
        else
        {
            upgradeHoverButton.setHoverEnterAction(() => showUpgradedCardOnHover());
            upgradeHoverButton.setHoverExitAction(() => fillCardInformation(currentCard));
            upgradeHoverButton.setButtonFunction(() => ForgeSceneController.instance.instatiateForgeUpgradeCardDisplay(model, model.upgradedCard));
        }

        removalHoverButton.setButtonFunction(() => ForgeSceneController.instance.instatiateForgeRemovalCardConfirmation(model));
    }

    public CardModelSO getCurrentCard()
    {
        return currentCard;
    }
}
