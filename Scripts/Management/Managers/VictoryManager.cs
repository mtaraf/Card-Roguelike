using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class VictoryManager : MonoBehaviour
{
    [SerializeField] private GameObject victoryCardSelectionScreenPrefab;
    [SerializeField] private GameObject cardSelectionDisplayPrefab;
    [SerializeField] private List<DeckModelSO> victoryCardPools; // 0: common, 1: rare, etc.
    [SerializeField] private Transform canvasTransform;

    private CardModelSO[] chosenCards = new CardModelSO[3];
    private GameObject victoryScreenInstance;
    private Dictionary<CardRarity, Color> rarityColors = new()
    {
        {CardRarity.COMMON, new Color(204f / 255f, 204f / 255f, 204f / 255f, 1f)},
        {CardRarity.RARE,   new Color(90f / 255f, 207f / 255f, 255f / 255f, 1f)},
        {CardRarity.EPIC,   new Color(190f / 255f, 123f / 255f, 248f / 255f, 1f)},
        {CardRarity.MYTHIC, new Color(255f / 255f, 142f / 255f, 6f / 255f, 1f)},
    };

    private Card selectedCard;
    private List<VictoryCardChoice> cards = new List<VictoryCardChoice>();

    public void instantiate()
    {
        victoryCardSelectionScreenPrefab = Resources.Load<GameObject>("UI/General/VictoryCardSelection");
        cardSelectionDisplayPrefab = Resources.Load<GameObject>("UI/Cards/VictoryCardChoicePrefab");
        canvasTransform = GameObject.FindGameObjectWithTag("OverlayCanvas").transform;
        victoryCardPools = GameManager.instance.getVictoryCardsPools();
    }

    public void showVictoryScreen(int cardChoices, int cardRarity, bool hasMythic)
    {
        victoryScreenInstance = Instantiate(victoryCardSelectionScreenPrefab, canvasTransform);
        generateCardRewards(cardRarity, hasMythic);

        Transform rewards = victoryScreenInstance.transform.Find("Rewards");
        if (rewards == null)
        {
            Debug.LogError("Rewards transform not found in VictoryCardSelection prefab.");
            return;
        }

        for (int i = 0; i < cardChoices; i++)
        {
            GameObject display = Instantiate(cardSelectionDisplayPrefab, rewards);
            cards.Add(display.GetComponent<VictoryCardChoice>());
            display.GetComponent<Card>().setCardDisplayInformation(chosenCards[i]);
        }
    }

    void generateCardRewards(int cardRarity, bool hasMythic)
    {
        int randomUpperLimit = 100;
        if (hasMythic)
            randomUpperLimit = 95;

        for (int i = 0; i < 3; i++)
        {
            int rand = Random.Range(cardRarity, randomUpperLimit);
            CardModelSO card = rand switch
            {
                < 55 => getRandomCard(victoryCardPools[0]),
                < 75 => getRandomCard(victoryCardPools[1]),
                < 90 => getRandomCard(victoryCardPools[2]),
                _ => getRandomCard(victoryCardPools[3])
            };
            chosenCards[i] = card;
        }
    }

    CardModelSO getRandomCard(DeckModelSO deck)
    {
        int random = Random.Range(0, deck.cards.Count);
        CardModelSO card = deck.cards[random];
        if (chosenCards.Contains(card))
        {
            if (random + 1 < deck.cards.Count)
            {
                card = deck.cards[random + 1];
            }
            else
            {
                card = deck.cards[random - 1];
            }
        }
        return card;
    }

    void confirmReward()
    {
        Debug.Log($"Selected Card: {selectedCard.getCardTitle()}");
        if (selectedCard == null)
        {
            // TO-DO: Maybe show popup warning mesage
        }
        else if (selectedCard.getRarity() == CardRarity.MYTHIC)
            GameManager.instance.setPlayerMythicCard(selectedCard.getCardModel());
        else
            GameManager.instance.addCardToPlayerDeck(selectedCard.getCardModel());

        Destroy(victoryScreenInstance);
        StartCoroutine(GameManager.instance.moveToPathSelection());
    }

    public void setSelectedCard(Card card)
    {
        selectedCard = card;

        // foreach (VictoryCardChoice cardChoice in cards)
        // {
        //     if (card != cardChoice.get)
        // }
    }
}