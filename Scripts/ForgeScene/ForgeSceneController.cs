using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForgeSceneController : MonoBehaviour
{
    public static ForgeSceneController instance;
    private GameObject forgeCardContainer;
    private GameObject forgeScrollViewContent;
    private GameObject forgeUpgradeCardDisplay;
    private DeckModelSO deck;
    private int containerXPosition = -1000;
    private int containerYPosition = -300;
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
        forgeScrollViewContent = GameObject.Find("ForgeScrollViewContent");

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

        // Adjust the scroll view width
        RectTransform scrollViewContentRect = forgeScrollViewContent.GetComponent<RectTransform>();
        scrollViewContentRect.sizeDelta = new Vector2(scrollViewContentRect.sizeDelta.x, rows * 800);

        containerXPosition = 250;

        GameObject newCardContainer;
        foreach (CardModelSO cardModel in deck.cards)
        {
            newCardContainer = Instantiate(forgeCardContainer, forgeScrollViewContent.transform);
            displayedCards.Add(newCardContainer);
            newCardContainer.transform.localPosition = new Vector2(containerXPosition, containerYPosition);

            // Fill Card Information
            newCardContainer.GetComponent<CardInteractions>().fillCardInformation(cardModel);

            // adjust x/y position of next card
            if (containerXPosition > 2000)
            {
                containerXPosition = 250;
                containerYPosition -= 500;
            }
            else
            {
                containerXPosition += 400;
            }
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
            GameObject upgradeDisplay = Instantiate(forgeUpgradeCardDisplay, forgeScrollViewContent.transform);
            UpgradeCardConfirmation upgradeCardConfirmation = upgradeDisplay.GetComponent<UpgradeCardConfirmation>();
            upgradeCardConfirmation.setCardInformation(currentCard, upgradedCard);
        }
        else
        {
            // TO-DO: Add not enough gold message
        }
    }
}
