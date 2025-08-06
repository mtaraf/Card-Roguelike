using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForgeSceneController : MonoBehaviour
{
    public static ForgeSceneController instance;
    private GameObject forgeCardContainer;
    private GameObject forgeScrollViewContent;
    private GameObject overlayCanvas;
    private GameObject forgeUpgradeCardDisplay;
    private GameObject forgeRemovalConfirmation;
    private DeckModelSO deck;
    private List<GameObject> displayedCards = new();

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    void Start()
    {
        InitializeScene();
    }

    private void InitializeScene()
    {
        deck = GameManager.instance.getPlayerDeck();
        forgeCardContainer = Resources.Load<GameObject>("UI/ForgeScene/ForgeCardContainer");
        forgeUpgradeCardDisplay = Resources.Load<GameObject>("UI/ForgeScene/UpgradeCardConfirmationScreen");
        forgeRemovalConfirmation = Resources.Load<GameObject>("UI/ForgeScene/RemovalConfirmation");
        forgeScrollViewContent = GameObject.Find("ForgeScrollViewContent");
        overlayCanvas = GameObject.FindGameObjectWithTag("OverlayCanvas");

        if (forgeScrollViewContent == null)
        {
            Debug.LogError("Could not find scroll view content in forge scene");
        }

        StartCoroutine(instantiateCards());
    }

    private IEnumerator instantiateCards()
    {
        yield return null;

        int deckSize = deck.cards.Count;
        int rows = deckSize / 6;

        // Adjust the scroll view height
        RectTransform scrollViewContentRect = forgeScrollViewContent.GetComponent<RectTransform>();
        scrollViewContentRect.sizeDelta = new Vector2(scrollViewContentRect.sizeDelta.x, rows * 800);

        foreach (CardModelSO cardModel in deck.cards)
        {
            GameObject newCardContainer = Instantiate(forgeCardContainer, forgeScrollViewContent.transform);
            displayedCards.Add(newCardContainer);
            newCardContainer.GetComponent<CardInteractions>().fillCardInformation(cardModel);
        }
    }

    public void updateUpgradedCardDisplay(CardModelSO current, CardModelSO upgraded)
    {
        GameObject upgradedCard = displayedCards.Find((obj) => obj.GetComponent<CardInteractions>().getCurrentCard() == current);
        CardInteractions cardInteractions = upgradedCard.GetComponent<CardInteractions>();
        StartCoroutine(cardInteractions.cardUpgradeAnimation(() => cardInteractions.fillCardInformation(upgraded)));
    }

    public void instatiateForgeUpgradeCardDisplay(CardModelSO currentCard, CardModelSO upgradedCard)
    {
        if (GameManager.instance.getPlayerGold() >= 30)
        {
            GameObject upgradeDisplay = Instantiate(forgeUpgradeCardDisplay, overlayCanvas.transform);
            UpgradeCardConfirmation upgradeCardConfirmation = upgradeDisplay.GetComponent<UpgradeCardConfirmation>();
            upgradeCardConfirmation.setCardInformation(currentCard, upgradedCard);
        }
        else
        {
            // TO-DO: Add not enough gold message
        }
    }

    public void instatiateForgeRemovalCardConfirmation(CardModelSO card)
    {
        if (GameManager.instance.getPlayerGold() >= 25)
        {
            GameObject removalConfirmation = Instantiate(forgeRemovalConfirmation, overlayCanvas.transform);
            CardRemovalConfirmation cardRemovalConfirmation = removalConfirmation.GetComponent<CardRemovalConfirmation>();
            cardRemovalConfirmation.setCardInformation(card);
        }
        else
        {
            // Add insufficient gold message
        }
    }

    public void removeCardDisplay(CardModelSO card)
    {
        GameObject removedCard = displayedCards.Find((obj) => obj.GetComponent<CardInteractions>().getCurrentCard() == card);

        if (removedCard != null)
        {
            displayedCards.Remove(removedCard);
            StartCoroutine(removedCard.GetComponent<CardInteractions>()
            .cardRemovalAnimation(() => Destroy(removedCard)));
        }
    }
}
