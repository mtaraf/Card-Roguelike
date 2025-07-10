using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HandManager : MonoBehaviour
{
    public static HandManager instance;

    [SerializeField] private Player player;
    [SerializeField] private HandUIController handUI;
    private CardProcessor cardProcessor;

    // Decks
    private DeckModelSO drawPile;
    private DeckModelSO discardPile;
    private DeckModelSO playerDeck;
    private DeckModelSO corruptedCards;

    // Cards
    private Card selectedCard = null;
    private GameObject cardSlots;
    private List<GameObject> cardSlotsList;

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

        cardSlotsList = new List<GameObject>();
    }

    // Any data that is created in Start() functions of other files needs to be accessed after the first frame
    private IEnumerator createDecksAfterStartHasRun()
    {
        yield return null;

        player = FindFirstObjectByType<Player>();
        if (player == null)
        {
            Debug.LogError("Player not found in scene from hand manager");
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

        cardProcessor = new CardProcessor(BaseLevelSceneController.instance);

        corruptedCards = ScriptableObject.CreateInstance<DeckModelSO>();
        corruptedCards.cards = new List<CardModelSO>();

        handUI.Initialize();
    }

    // General card functions
    public void setSelectedCard(GameObject card)
    {
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
        int randomCardIndex;

        // Reshuffle discard and draw if drawPile does not have enough cards
        if (drawPile.cards.Count < numCards)
        {
            shuffleDiscardPileIntoDrawPile();
        }

        for (int i = 0; i < numCards; i++)
        {
            // Don't draw cards if out of cards
            if (drawPile.cards.Count == 0)
            {
                return;
            }

            randomCardIndex = Random.Range(0, drawPile.cards.Count);
            bool spaceInHandRemaining = addCardToCardSlot(drawPile.cards[randomCardIndex]);
            if (spaceInHandRemaining)
            {
                drawPile.cards.RemoveAt(randomCardIndex);
            }
        }
    }

    public void addCorruptedCard(CardModelSO model)
    {
        corruptedCards.cards.Add(model);
    }

    public List<CardModelSO> getCorruptedCards()
    {
        return corruptedCards.cards;
    }

    private void shuffleDiscardPileIntoDrawPile()
    {
        List<CardModelSO> discardCopy = new List<CardModelSO>(discardPile.cards);
        foreach (CardModelSO card in discardCopy)
        {
            drawPile.cards.Add(card);
            if (!card.corrupts)
            {
                discardPile.cards.Remove(card);
            }
        }
    }

    private void addCardToDiscardPile(Card card)
    {
        discardPile.cards.Add(card.getCardModel());
    }

    // Card Processing
    public List<CardEffect> useSelectedCard()
    {
        // Get player attributes
        Dictionary<EffectType, int> playerAttributes = BaseLevelSceneController.instance.getPlayerAttributes();

        // Get card effects
        List<CardEffect> effects =  cardProcessor.processCard(selectedCard, playerAttributes);

        // Add card to discard pile and remove card
        if (selectedCard.isCorrupt())
        {
            addCorruptedCard(selectedCard.getCardModel());
        }
        else
        {
            addCardToDiscardPile(selectedCard);
        }

        clearSelectedCard();

        return effects;
    }

    // UI Changes
    public HandUIController getHandUIContoller()
    {
        return handUI;
    }
    public IEnumerator enemyCardAnimation(CardModelSO model)
    {
        return handUI.enemyCardAnimation(model);
    }

    public bool addCardToCardSlot(CardModelSO cardInformation)
    {
        return handUI.addCardToSlot(cardInformation);
    }

    public void reorganizeHand()
    {
        handUI.reorganizeCardsInHand();
    }

    public int getNumCardsInHand()
    {
        return handUI.getNumCardsInHand();
    }

    public void shuffleCurrentHandIntoDiscardPile()
    {
        handUI.shuffleHandIntoDiscard(discardPile);
    }

    public IEnumerator displayFeedbackMessage(string message)
    {
        return handUI.displayFeedbackMessage(message);
    }
}

public enum EffectType
{
    Damage,
    Armor,
    Strength,
    Weaken,
    Divinity,
    Poison,
}

public static class EffectTypeExtensions
{
    public static string ToDisplayString(this EffectType eff)
    {
        switch (eff)
        {
            case EffectType.Strength: return "StrengthEffect";
            case EffectType.Armor: return "ArmorEffect";
            case EffectType.Poison: return "PoisonEffect";
            case EffectType.Divinity: return "DivinityEffect";
            case EffectType.Weaken: return "WeakenEffect";
            default: return "Default";
        }
    }
}