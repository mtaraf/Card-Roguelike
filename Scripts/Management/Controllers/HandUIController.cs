using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HandUIController : MonoBehaviour
{
    [SerializeField] private GameObject centerOfUI;
    private GameObject discardPile;
    [SerializeField] private GameObject feedbackMessage;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GameObject noAnimationCardPrefab;
    private Vector3 cardSpawnPoint;
    private List<Card> cardsInHand = new List<Card>();
    [SerializeField] private float handWidth = 900f;
    [SerializeField] private float cardSpacing = 160f;
    [SerializeField] private float animationDuration = 0.25f;
    private float cardMoveSpeed = 0.6f;



    public void Initialize()
    {
        centerOfUI = GameObject.FindGameObjectWithTag("CenterOfUI");
        discardPile = GameObject.FindGameObjectWithTag("DiscardPile");

        if (centerOfUI == null || discardPile == null)
        {
            Debug.LogError("Could not find centerofUI or discard pile object in hand UI");
        }

        cardSpawnPoint = new Vector3(centerOfUI.transform.localPosition.x, centerOfUI.transform.localPosition.y + 180, 0);

        feedbackMessage = Resources.Load<GameObject>("UI/General/Feedback/FeedbackMessage");
        cardPrefab = Resources.Load<GameObject>("UI/Cards/CardPrefab");
        noAnimationCardPrefab = Resources.Load<GameObject>("UI/Cards/CardNoAnimation");
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
    }

    public void addCardToHand(CardModelSO cardModel)
    {
        GameObject cardObj = Instantiate(cardPrefab, centerOfUI.transform);
        cardObj.transform.position = cardSpawnPoint;
        Card card = cardObj.GetComponent<Card>();
        card.setCardDisplayInformation(cardModel);

        cardsInHand.Add(card);
        StartCoroutine(reflowHand());
    }

    public void moveCardToDiscardPile(Card card, Action onComplete = null)
    {
        Transform cardTransform = cardsInHand.Find((cardInHand) => cardInHand == card).transform;
        animateCardMovement(cardTransform, discardPile.transform.position, cardTransform.localScale, cardMoveSpeed, onComplete);
        cardsInHand.Remove(card);
    }

    public void removeCardFromHand(Card card)
    {
        cardsInHand.Remove(card);
        Destroy(card.gameObject);
        StartCoroutine(reflowHand());
    }

    public IEnumerator reflowHand()
    {
        yield return new WaitForSeconds(0.1f);

        if (cardsInHand.Count == 0)
            yield break;

        float totalWidth = (cardsInHand.Count - 1) * cardSpacing;
        float startX = -totalWidth / 2f;

        for (int i = 0; i < cardsInHand.Count; i++)
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
        return cardsInHand.Count;
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

        yield return new WaitForSeconds(3f);

        float fadeDuration = 1f;
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            float t = elapsedTime / fadeDuration;
            cg.alpha = Mathf.Lerp(1f, 0f, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.7f);
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
        yield return new WaitForSeconds(0.2f);

        // PHASE 2 — Light upward drift + fade out
        float fadeDuration = 0.5f;
        float fadeTimer = 0f;

        Vector3 driftStart = card.position;
        Vector3 driftTarget = driftStart + new Vector3(0, 60f, 0); // gentle lift

        Vector3 startScaleFade = card.localScale;
        Vector3 endScaleFade = startScaleFade * 0.5f;

        while (fadeTimer < fadeDuration)
        {
            float f = fadeTimer / fadeDuration;

            card.position = Vector3.Lerp(driftStart, driftTarget, f);
            cg.alpha = Mathf.Lerp(1f, 0f, f);
            card.localScale = Vector3.Lerp(startScaleFade, endScaleFade, f);

            fadeTimer += Time.deltaTime;
            yield return null;
        }

        cg.alpha = 0f;
        card.localScale = endScaleFade;

        // Final callback
        onComplete?.Invoke();
    }

    public IEnumerator animateCardPlayed(Transform card, Vector3 targetPosition, Vector3 targetScale, float duration, Action onComplete = null)
    {
        yield return StartCoroutine(moveCardCoroutine(card, targetPosition, targetScale, duration));
        yield return StartCoroutine(fadeOutCoroutine(card.gameObject, duration, onComplete));
    }

    IEnumerator moveCardCoroutine(Transform card, Vector3 targetPos, Vector3 targetScale, float duration, Action onComplete = null)
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

    IEnumerator fadeOutCoroutine(GameObject card, float duration, Action onComplete = null)
    {
        CanvasGroup cg = card.GetComponent<CanvasGroup>() ?? card.AddComponent<CanvasGroup>();

        Transform cardTransform = card.transform;
        Vector3 startScale = cardTransform.localScale;
        Vector3 endScale = startScale * 0.5f;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float f = elapsed / duration;

            cg.alpha = Mathf.Lerp(1f, 0f, f);

            cardTransform.localScale = Vector3.Lerp(startScale, endScale, f);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cg.alpha = 0f;
        cardTransform.localScale = endScale;

        yield return null;

        onComplete?.Invoke();
    }

    public List<Card> getCurrentHand()
    {
        return cardsInHand;
    }

}