using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HandUIController : MonoBehaviour
{
    [SerializeField] private GameObject centerOfUI;
    [SerializeField] private GameObject feedbackMessage;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GameObject noAnimationCardPrefab;
    private Vector3 cardSpawnPoint;

    private GameObject cardSlots;
    private List<GameObject> cardSlotsList = new();
    private List<Card> cardsInHand = new List<Card>();
    [SerializeField] private float handWidth = 900f;
    [SerializeField] private float cardSpacing = 160f;
    [SerializeField] private float animationDuration = 0.25f;



    public void Initialize()
    {
        centerOfUI = GameObject.FindGameObjectWithTag("CenterOfUI");
        cardSpawnPoint = new Vector3(centerOfUI.transform.localPosition.x, centerOfUI.transform.localPosition.y + 180, 0);
        cardSlots = GameObject.FindGameObjectWithTag("CardSlots");

        feedbackMessage = Resources.Load<GameObject>("UI/General/Feedback/FeedbackMessage");
        cardPrefab = Resources.Load<GameObject>("UI/Cards/CardPrefab");
        noAnimationCardPrefab = Resources.Load<GameObject>("UI/Cards/CardNoAnimation");

        cardSlotsList.Clear();
        for (int i = 0; i < cardSlots.transform.childCount; i++)
        {
            string slotName = "CardSlot" + (i + 1);
            cardSlotsList.Add(cardSlots.transform.Find(slotName).gameObject);
        }
    }

    public void updateCardDisplay(CardModelSO card)
    {
        Card cardToUpdate = cardsInHand.Find((cardToChange) => cardToChange.getCardTitle() == card.title);
        if (cardToUpdate == null)
        {
            Debug.LogError($"Could not update card display for {card.title}");
            return;
        }

        cardToUpdate.setCardDisplayInformation(card);
        Debug.Log($"Card display updated for {card.title}");
        // TO-DO: Add animation for this update
    }

    public void addCardToHand(CardModelSO cardModel)
    {
        GameObject cardObj = Instantiate(cardPrefab, centerOfUI.transform);
        cardObj.transform.position = cardSpawnPoint;
        Card card = cardObj.GetComponent<Card>();
        card.setCardDisplayInformation(cardModel);

        cardsInHand.Add(card);
        reflowHand();
    }

    public void removeCardFromHand(Card card)
    {   
        cardsInHand.Remove(card);
        Destroy(card.gameObject);
        reflowHand();
    }

    public void reflowHand()
    {
        if (cardsInHand.Count == 0)
            return;

        float totalWidth = (cardsInHand.Count - 1) * cardSpacing;
        float startX = -totalWidth / 2f;

        for (int i=0; i<cardsInHand.Count; i++)
        {
            Card card = cardsInHand[i];
            RectTransform rt = card.GetComponent<RectTransform>();

            Vector3 targetPos = new Vector3(startX + i * cardSpacing, rt.anchoredPosition.y, 0);

            StartCoroutine(moveCard(rt, targetPos, animationDuration, card.gameObject.GetComponent<CardSelectionHandler>()));
        }
    }

    private IEnumerator moveCard(RectTransform card, Vector3 target, float duration, CardSelectionHandler cardSelectionHandler)
    {
        Vector3 startPos = card.anchoredPosition;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            card.anchoredPosition = Vector3.Lerp(startPos, target, t / duration);
            yield return null;
        }

        card.anchoredPosition = target;

        cardSelectionHandler.setNewStartPos(card.transform.position);
    }

    public int getNumCardsInHand()
    {
        int count = 0;
        foreach (var slot in cardSlotsList)
        {
            if (slot.transform.childCount != 0)
                count++;
        }
        return count;
    }

    public void shuffleHandIntoDiscard(ObservableDeck discardPile)
    {
        List<Card> handCopy = new List<Card>(cardsInHand);
        foreach (Card card in handCopy)
        {
            discardPile.Add(card.getCardModel());
            cardsInHand.Remove(card);
            Destroy(card.gameObject);
        }
    }

    public IEnumerator enemyCardAnimation(CardModelSO model)
    {
        GameObject card = Instantiate(noAnimationCardPrefab, centerOfUI.transform);
        card.GetComponent<Card>().setCardDisplayInformation(model);

        CanvasGroup cg = card.GetComponent<CanvasGroup>() ?? card.AddComponent<CanvasGroup>();

        yield return new WaitForSeconds(0.5f);

        float fadeDuration = 1f;
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            float t = elapsedTime / fadeDuration;
            cg.alpha = Mathf.Lerp(1f, 0f, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        Destroy(card);
    }

    public IEnumerator displayFeedbackMessage(string message)
    {
        GameObject msgObj = Instantiate(feedbackMessage, centerOfUI.transform);
        msgObj.GetComponent<TextMeshProUGUI>().text = message;
        yield return new WaitForSeconds(1.0f);
        Destroy(msgObj);
    }

    public void animateCardMovement(Transform card, Vector3 targetPosition, Vector3 targetScale, float duration, Action onComplete = null)
    {
        StartCoroutine(moveCardCoroutine(card, targetPosition, targetScale, duration, onComplete));
    }

    public IEnumerator animateLitheCardPlayed(Transform card, Vector3 targetPosition, Vector3 targetScale, float duration, Action onComplete = null)
    {
        yield return StartCoroutine(litheCardAnimation(card, targetPosition, targetScale, duration, onComplete));
    }

    private IEnumerator litheCardAnimation(Transform card, Vector3 targetPosition, Vector3 targetScale, float duration, Action onComplete = null)
    {
        CanvasGroup cg = card.GetComponent<CanvasGroup>() ?? card.gameObject.AddComponent<CanvasGroup>();
        cg.alpha = 1f;

        Vector3 startPos = card.position;
        Vector3 startScale = card.localScale;
        Quaternion startRot = card.rotation;

        // Add a soft tilt
        Quaternion targetRot = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-10f, 10f));

        float t = 0f;

        // PHASE 1 — Move to target position + scale
        while (t < duration)
        {
            float lerp = t / duration;

            // Elegant movement & scale
            card.position = Vector3.Lerp(startPos, targetPosition, lerp);
            card.localScale = Vector3.Lerp(startScale, targetScale, lerp);
            card.rotation = Quaternion.Slerp(startRot, targetRot, lerp);

            t += Time.deltaTime;
            yield return null;
        }

        // Snap to final transform
        card.position = targetPosition;
        card.localScale = targetScale;
        card.rotation = targetRot;

        // Small delay to let the player "see" the card
        yield return new WaitForSeconds(0.1f);

        // PHASE 2 — Light upward drift + fade out
        float fadeDuration = 0.4f;
        float fadeTimer = 0f;

        Vector3 driftStart = card.position;
        Vector3 driftTarget = driftStart + new Vector3(0, 60f, 0); // gentle lift

        while (fadeTimer < fadeDuration)
        {
            float f = fadeTimer / fadeDuration;

            card.position = Vector3.Lerp(driftStart, driftTarget, f);
            cg.alpha = Mathf.Lerp(1f, 0f, f);

            fadeTimer += Time.deltaTime;
            yield return null;
        }

        cg.alpha = 0f;

        // Remove card from hand
        Card cardComponent = card.GetComponent<Card>();
        if (cardComponent != null)
            removeCardFromHand(cardComponent);

        // Final callback
        onComplete?.Invoke();
    }

    public IEnumerator animateCardPlayed(Transform card, Vector3 targetPosition, Vector3 targetScale, float duration, Action onComplete = null)
    {
        yield return StartCoroutine(moveCardCoroutine(card, targetPosition, targetScale, duration, onComplete));
        yield return StartCoroutine(fadeOutCoroutine(card.gameObject, duration));
    }

    IEnumerator moveCardCoroutine(Transform card, Vector3 targetPos, Vector3 targetScale, float duration, Action onComplete)
    {
        Vector3 startPos = card.position;
        Vector3 startScale = card.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (card == null)
            {
                yield break;
            }
            
            float t = elapsedTime / duration;
            card.position = Vector3.Lerp(startPos, targetPos, t);
            card.localScale = Vector3.Lerp(startScale, targetScale, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        card.position = targetPos;
        card.localScale = targetScale;
        onComplete?.Invoke();
    }

    public void fadeAndDestroy(GameObject card, float duration)
    {
        StartCoroutine(fadeOutCoroutine(card, duration));
    }

    IEnumerator fadeOutCoroutine(GameObject card, float duration)
    {
        CanvasGroup cg = card.GetComponent<CanvasGroup>() ?? card.AddComponent<CanvasGroup>();
        float elapsed = 0f;
        while (elapsed < duration)
        {
            cg.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cg.alpha = 0f;
        Destroy(card);
        yield return null;

        //reorganizeCardsInHand();
    }
    
}