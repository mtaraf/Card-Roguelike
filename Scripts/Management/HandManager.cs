using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HandManager : MonoBehaviour
{
    public static HandManager instance;

    [SerializeField] private Player player;
    [SerializeField] public GameObject cardPrefab;

    // Decks
    private DeckModelSO drawPile;
    private DeckModelSO discardPile;
    private DeckModelSO playerDeck;

    // Cards
    private Card selectedCard = null;
    private GameObject cardSlots;
    private int handSize = 6; // Default Hand Size

    private List<GameObject> cardSlotsList = new List<GameObject>();

    public void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Detect when a new scene is loaded
        SceneManager.sceneLoaded += onSceneLoaded;
    }

    public void onSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Hand Manager detected scene loaded: {scene.name}");
        StartCoroutine(createDecksAfterStartHasRun());
    }

    // Any data that is created in Start() functions of other files needs to be accessed after the first frame
    private IEnumerator createDecksAfterStartHasRun()
    {
        yield return null;

        player = FindFirstObjectByType<Player>();
        if (player == null)
        {
            Debug.Log("Player not found in scene from hand manager");
        }

        // Get player deck and create an empty discard deck and a full draw pile on load
        playerDeck = ScriptableObject.CreateInstance<DeckModelSO>();
        playerDeck.cards = new List<CardModelSO>(player?.getCards().cards);

        drawPile = ScriptableObject.CreateInstance<DeckModelSO>();
        drawPile.cards = new List<CardModelSO>(playerDeck != null ? playerDeck.cards : new List<CardModelSO>());

        discardPile = ScriptableObject.CreateInstance<DeckModelSO>();
        discardPile.cards = new List<CardModelSO>();

        cardSlots = GameObject.FindGameObjectWithTag("CardSlots");

        string cardSlotName;
        for (int i = 0; i < cardSlots.transform.childCount; i++)
        {
            cardSlotName = "CardSlot" + (i + 1).ToString();
            cardSlotsList.Add(cardSlots.transform.Find(cardSlotName).gameObject);
        }
    }

    // General card functions
    public void setSelectedCard(GameObject card)
    {
        Debug.Log("Card Selected: " + card.GetComponent<Card>().getCardModel());
        if (card.GetComponent<Card>() != null)
        {
            selectedCard = card.GetComponent<Card>();
        }
        else
        {
            Debug.LogError("No card component found in setSelectedCard");
        }
    }

    public void clearSelectedCard()
    {
        selectedCard = null;
    }

    public Card getSelectedCard()
    {
        return selectedCard;
    }

    public bool hasSelectedCard()
    {
        return selectedCard != null;
    }

    // Deck Functions
    public DeckModelSO getDiscardPile()
    {
        return discardPile;
    }

    public DeckModelSO getDrawPile()
    {
        return drawPile;
    }

    public DeckModelSO getPlayerDeck()
    {
        return playerDeck;
    }

    public void setPlayerDeck(DeckModelSO deck)
    {
        playerDeck = deck;
    }

    // Card movement functions
    public void drawCards(int numCards)
    {
        Debug.Log("Draw cards called");
        int randomCardIndex = -1;

        // Reshuffle discard and draw if drawPile does not have enough cards
        if (drawPile.cards.Count < numCards)
        {
            shuffleDiscardPileIntoDrawPile();
        }

        for (int i = 0; i < numCards; i++)
        {
            randomCardIndex = Random.Range(0, drawPile.cards.Count);
            bool spaceInHandRemaining = addCardToCardSlot(drawPile.cards[randomCardIndex]);
            if (spaceInHandRemaining)
            {
                drawPile.cards.RemoveAt(randomCardIndex);
            }
        }

    }

    public bool addCardToCardSlot(CardModelSO cardInformation)
    {
        for (int i = 0; i < cardSlotsList.Count; i++)
        {
            if (cardSlotsList[i].transform.childCount == 0)
            {
                GameObject card = Instantiate(cardPrefab, cardSlotsList[i].transform);
                Card cardInfo = card.GetComponent<Card>();
                cardInfo.setCardDisplayInformation(cardInformation);
                return true;
            }
        }
        return false;
    }

    private void shuffleDiscardPileIntoDrawPile()
    {
        List<CardModelSO> discardCopy = new List<CardModelSO>(discardPile.cards);
        foreach (CardModelSO card in discardCopy)
        {
            drawPile.cards.Add(card);
            discardPile.cards.Remove(card);
        }
    }

    // Card Processing
    public CardEffects processCard(Card card)
    {
        int armor, ward, damage;
        int[] damageOverTime;
        CardEffects effects = new CardEffects();

        if (card.hasCondition())
        {

        }
        else
        {
            return new CardEffects(card.getDamage(), card.getArmor(), card.getWard());
        }

        return new CardEffects();
    }

    public CardEffects useSelectedCard()
    {
        // Get card effects
        CardEffects effects = processCard(selectedCard);


        // Add card to discard pile and remove card
        addCardToDiscardPile(selectedCard);


        clearSelectedCard();

        return effects;
    }

    private void addCardToDiscardPile(Card card)
    {
        discardPile.cards.Add(card.getCardModel());
    }

    public void shuffleCurrentHandIntoDiscardPile()
    {
        for (int i = 0; i < cardSlotsList.Count; i++)
        {
            if (cardSlotsList[i].transform.childCount != 0)
            {
                GameObject card = cardSlotsList[i].transform.GetChild(0).gameObject;
                Card cardInfo = card.GetComponent<Card>();
                addCardToDiscardPile(cardInfo);
                Destroy(card);
            }
        }
    }
}

public class CardEffects
{
    private int totalDamage = 0;
    private int totalArmor = 0;
    private int totalWard = 0;
    private int[] damageOverTime;

    public CardEffects(int damage, int armor, int ward)
    {
        totalDamage = damage;
        totalArmor = armor;
        totalWard = ward;
    }

    public CardEffects()
    {

    }

    public void setTotalDamage(int damage)
    {
        totalDamage = damage;
    }

    public int getTotalDamage()
    {
        return totalDamage;
    }

    public void setTotalArmor(int armor)
    {
        totalArmor = armor;
    }

    public int getTotalArmor()
    {
        return totalArmor;
    }

    public void setTotalWard(int ward)
    {
        totalWard = ward;
    }

    public int getTotalWard()
    {
        return totalWard;
    }

    public void setTotalDamageOverTime(int[] dot)
    {
        damageOverTime = dot;
    }
    
    public int[] getTotalDamageOverTime()
    {
        return damageOverTime;
    }
}