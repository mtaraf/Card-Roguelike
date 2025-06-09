using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HandManager : MonoBehaviour
{
    public static HandManager instance;

    [SerializeField] private Player player;
    private DeckModelSO drawPile;
    private DeckModelSO discardPile;
    private DeckModelSO playerDeck;
    private GameObject selectedCard = null;

    private GameObject cardSlots;

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
    }

    // Card Functions
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

    public void drawCards(int numCards)
    {
        
    }

    public void drawNewHand()
    {
        
    }

    public void endTurn()
    {
        
    }

    public void useSelectedCard()
    {

    }
}
