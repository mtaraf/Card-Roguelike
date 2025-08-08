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
    private ObservableDeck drawPile;
    private ObservableDeck discardPile;
    private DeckModelSO playerDeck;
    private DeckModelSO corruptedCards;

    // Cards
    private Card selectedCard = null;
    private GameObject cardSlots;
    private List<GameObject> cardSlotsList;
    private Card lastCardPlayed;

    public void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void Initialize()
    {
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

        drawPile = new ObservableDeck();
        drawPile.cards.AddRange(playerDeck.cards);
        drawPile.OnDeckSizeChanged += GameManager.instance.updateDrawPile;

        discardPile = new ObservableDeck();
        discardPile.OnDeckSizeChanged += GameManager.instance.updateDiscardPile;

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
    public ObservableDeck getDiscardPile()
    {
        return discardPile;
    }

    public ObservableDeck getDrawPile()
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
        if (drawPile.Count < numCards)
        {
            shuffleDiscardPileIntoDrawPile();
        }

        for (int i = 0; i < numCards; i++)
        {
            // Don't draw cards if out of cards
            if (drawPile.Count == 0)
            {
                return;
            }

            randomCardIndex = Random.Range(0, drawPile.Count);
            bool spaceInHandRemaining = addCardToCardSlot(drawPile.cards[randomCardIndex]);
            if (spaceInHandRemaining)
            {
                drawPile.RemoveAt(randomCardIndex);
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
            drawPile.Add(card);
            if (!card.corrupts)
            {
                discardPile.Remove(card);
            }
        }
    }

    private void addCardToDiscardPile(Card card)
    {
        discardPile.Add(card.getCardModel());
    }

    // Card Processing
    public List<CardEffect> useSelectedCard()
    {
        // Get player attributes
        Dictionary<EffectType, int> playerAttributes = BaseLevelSceneController.instance.getPlayerAttributes();

        // Get card effects
        List<CardEffect> effects = cardProcessor.processCard(selectedCard, playerAttributes);

        // Add card to discard pile and remove card
        if (selectedCard.isCorrupt())
        {
            addCorruptedCard(selectedCard.getCardModel());
        }
        else
        {
            addCardToDiscardPile(selectedCard);
        }

        lastCardPlayed = selectedCard;

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

    public void checkOnDeathEffect()
    {
        cardProcessor.checkOnDeathEffect(lastCardPlayed);
    }
}

public enum EffectType
{
    Damage,
    Armor,
    Strength,
    Weaken,
    Blind,
    Divinity,
    Poison,
    Frostbite,
    HealDamageDone,
    Heal
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
            case EffectType.Frostbite: return "FrostbiteEffect";
            case EffectType.Blind: return "BlindEffect";
            default: return "Default";
        }
    }

    public static string ToFeedbackString(this EffectType eff)
    {
        switch (eff)
        {
            case EffectType.Strength: return "+strength";
            case EffectType.Armor: return "+armor";
            case EffectType.Poison: return "+poison";
            case EffectType.Divinity: return "+divinity";
            case EffectType.Weaken: return "+weaken";
            case EffectType.Frostbite: return "+frostbite";
            default: return "Default";
        }
    }
}