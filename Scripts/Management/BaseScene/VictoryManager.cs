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

    private CardModelSO[] chosenCards = new CardModelSO[2];
    private GameObject victoryScreenInstance;
    private Dictionary<CardRarity, Color> rarityColors = new()
    {
        {CardRarity.COMMON, new Color(204f / 255f, 204f / 255f, 204f / 255f, 1f)},
        {CardRarity.RARE,   new Color(90f / 255f, 207f / 255f, 255f / 255f, 1f)},
        {CardRarity.EPIC,   new Color(190f / 255f, 123f / 255f, 248f / 255f, 1f)},
        {CardRarity.MYTHIC, new Color(255f / 255f, 142f / 255f, 6f / 255f, 1f)},
    };

    public void instantiate()
    {
        victoryCardSelectionScreenPrefab = Resources.Load<GameObject>("UI/General/VictoryCardSelection");
        cardSelectionDisplayPrefab = Resources.Load<GameObject>("UI/Cards/CardNoAnimation");
        canvasTransform = GameObject.FindGameObjectWithTag("OverlayCanvas").transform;
        victoryCardPools = GameManager.instance.getVictoryCardsPools();
    }

    // TO-DO: implement card choices / card rarity
    public void showVictoryScreen(int cardChoices, int cardRarity)
    {
        victoryScreenInstance = Instantiate(victoryCardSelectionScreenPrefab, canvasTransform);
        generateCardRewards();

        var slotsParent = victoryScreenInstance.transform.Find("Slots");
        for (int i = 0; i < cardChoices; i++)
        {
            var slot = slotsParent.GetChild(i).gameObject;
            int index = i;
            slot.GetComponent<Button>().onClick.AddListener(() => chooseCard(index));

            if (i < 2)
            {
                GameObject display = Instantiate(cardSelectionDisplayPrefab, slot.transform);
                display.transform.GetChild(0).GetComponent<Image>().color = rarityColors[chosenCards[i].rarity];
                display.GetComponent<Card>().setCardDisplayInformation(chosenCards[i]);
            }
        }
    }

    void generateCardRewards()
    {
        for (int i = 0; i < 2; i++)
        {
            int rand = Random.Range(0, 100);
            CardModelSO card = rand switch
            {
                < 65 => getRandomCard(victoryCardPools[0]),
                < 90 => getRandomCard(victoryCardPools[1]),
                < 98 => getRandomCard(victoryCardPools[2]),
                _ => getRandomCard(victoryCardPools[3])
            };
            chosenCards[i] = card;
        }
    }

    CardModelSO getRandomCard(DeckModelSO deck) {
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

    void chooseCard(int index)
    {
        if (index < 2)
        {
            Debug.Log("Selected card: " + chosenCards[index].title);
            GameManager.instance.addCardToPlayerDeck(chosenCards[index]);
        }
        else
        {
            Debug.Log("Enhance card path chosen");
        }

        Destroy(victoryScreenInstance);
        StartCoroutine(GameManager.instance.moveToNextEncounter());
    }
}