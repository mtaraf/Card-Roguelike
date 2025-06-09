using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HandManager : MonoBehaviour
{
    public static HandManager instance;

    [SerializeField] private Player player;
    PlayerEffects effects = new PlayerEffects();
    [SerializeField] public GameObject cardPrefab;

    // Decks
    private DeckModelSO drawPile;
    private DeckModelSO discardPile;
    private DeckModelSO playerDeck;

    // Cards
    private GameObject selectedCard = null;
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

        if (player == null)
        {
            Debug.Log("Player not found, trying to locate player by object type");
            player = FindFirstObjectByType<Player>(); // fallback if not assigned
        }

        StartCoroutine(createDecksAfterStartHasRun());
    }

    // Any data that is created in Start() functions of other files needs to be accessed after the first frame
    private IEnumerator createDecksAfterStartHasRun()
    {
        yield return null;
        // Get player deck and create an empty discard deck and a full draw pile on load
        Debug.Log("Setting player deck with: " + player.getCards());
        playerDeck = ScriptableObject.CreateInstance<DeckModelSO>();
        playerDeck.cards = new List<CardModelSO>(player?.getCards().cards);

        drawPile = ScriptableObject.CreateInstance<DeckModelSO>();
        drawPile.cards = new List<CardModelSO>(playerDeck != null ? playerDeck.cards : new List<CardModelSO>());

        discardPile = ScriptableObject.CreateInstance<DeckModelSO>();
        discardPile.cards = new List<CardModelSO>();

        cardSlots = GameObject.FindGameObjectWithTag("CardSlots");

        string cardSlotName = "";
        for (int i = 0; i < cardSlots.transform.childCount; i++)
        {
            cardSlotName = "CardSlot" + (i + 1).ToString();
            cardSlotsList.Add(cardSlots.transform.Find(cardSlotName).gameObject);
        }
    }

    // General card functions
    public void setSelectedCard(GameObject card)
    {
        selectedCard = card;
    }

    public void clearSelectedCard()
    {
        selectedCard = null;
    }

    public GameObject getSelectedCard()
    {
        return selectedCard;
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

    }

    // Start of turn functions
    public void startTurn()
    {
        // Check for any player effects add them 
        effects = player.getPlayerEffects();

        // Draw New Hand
        drawCards(handSize);

    }


    // End of turn functions
    public void endTurn()
    {
        // Remove cards from card slots and move them to the discard pile

        // Relay to GameManager
    }

    public void useSelectedCard(CardModelSO model)
    {
        // Check effects of player 


        

        // Add card to discard pile
    }
}
