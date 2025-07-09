using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HandUIController : MonoBehaviour
{
    [SerializeField] private GameObject centerOfUI;
    [SerializeField] private GameObject feedbackMessage;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GameObject noAnimationCardPrefab;

    private GameObject cardSlots;
    private List<GameObject> cardSlotsList = new();



    public void Initialize()
    {
        centerOfUI = GameObject.FindGameObjectWithTag("CenterOfUI");
        cardSlots = GameObject.FindGameObjectWithTag("CardSlots");

        cardSlotsList.Clear();
        for (int i = 0; i < cardSlots.transform.childCount; i++)
        {
            string slotName = "CardSlot" + (i + 1);
            cardSlotsList.Add(cardSlots.transform.Find(slotName).gameObject);
        }
    }

    public bool addCardToSlot(CardModelSO cardModel)
    {
        foreach (var slot in cardSlotsList)
        {
            if (slot.transform.childCount == 0)
            {
                GameObject card = Instantiate(cardPrefab, slot.transform);
                card.GetComponent<Card>().setCardDisplayInformation(cardModel);
                return true;
            }
        }
        return false;
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

    public void shuffleHandIntoDiscard(DeckModelSO discardPile)
    {
        foreach (var slot in cardSlotsList)
        {
            if (slot.transform.childCount > 0)
            {
                GameObject card = slot.transform.GetChild(0).gameObject;
                discardPile.cards.Add(card.GetComponent<Card>().getCardModel());
                Destroy(card);
            }
        }
    }

    public IEnumerator enemyCardAnimation(CardModelSO model)
    {
        GameObject card = Instantiate(noAnimationCardPrefab, centerOfUI.transform);
        card.GetComponent<Card>().setCardDisplayInformation(model);

        CanvasGroup cg = card.GetComponent<CanvasGroup>() ?? card.AddComponent<CanvasGroup>();

        yield return new WaitForSeconds(1.0f);

        float fadeDuration = 2f;
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

    public void reorganizeCardsInHand()
    {
        Debug.Log("Reorganize!");
        List<GameObject> cardsInHand = new List<GameObject>();

        // Step 1: Collect all existing cards
        foreach (GameObject slot in cardSlotsList)
        {
            if (slot.transform.childCount > 0)
            {
                Transform child = slot.transform.GetChild(0);
                if (child != null && child.gameObject != null)
                {
                    cardsInHand.Add(child.gameObject);
                }
            }
        }

        // Step 2: Clear all slots
        foreach (GameObject slot in cardSlotsList)
        {
            foreach (Transform child in slot.transform)
            {
                child.SetParent(null); // detach
            }
        }

        // Step 3: Reassign cards to the first available slots
        for (int i = 0; i < cardsInHand.Count; i++)
        {
            Debug.Log(cardSlotsList[i].name);
            cardsInHand[i].transform.SetParent(cardSlotsList[i].transform, false);

            // Optional: Reset position/scale to avoid layout glitches
            RectTransform rt = cardsInHand[i].GetComponent<RectTransform>();
            rt.anchoredPosition = Vector2.zero;
            rt.localScale = Vector3.one;
            rt.localRotation = Quaternion.identity;

            CardSelectionHandler selectionHandler = cardsInHand[i].GetComponent<CardSelectionHandler>();
            selectionHandler.setStartPos(cardsInHand[i].transform.position);
        }
    }

    public void animateCardMovement(Transform card, Vector3 targetPosition, Vector3 targetScale, float duration, Action onComplete = null)
    {
        StartCoroutine(moveCardCoroutine(card, targetPosition, targetScale, duration, onComplete));
    }

    IEnumerator moveCardCoroutine(Transform card, Vector3 targetPos, Vector3 targetScale, float duration, Action onComplete)
    {
        Vector3 startPos = card.position;
        Vector3 startScale = card.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
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

        reorganizeCardsInHand();
    }
    
}