using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    public static HandManager instance;

    [SerializeField] private Player player;
    [SerializeField] private HandUIController handUI;
    private CardProcessor cardProcessor;
    private ParentSceneController sceneController;

    // Decks
    private ObservableDeck drawPile;
    private ObservableDeck discardPile;
    private DeckModelSO playerDeck;
    private DeckModelSO corruptedCards;
    private DeckModelSO disabledCards;

    // Cards
    private Card selectedCard = null;
    private GameObject selectedCardObject = null;
    private List<GameObject> cardSlotsList;
    private Card lastCardPlayed;

    // Discard
    private bool discardInProgress = false;

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
        sceneController = GameManager.instance.getCurrentSceneController();

        if (sceneController == null)
        {
            Debug.LogError("Could not find scene controller in hand manager");
            return;
        }

        StartCoroutine(createDecksAfterStartHasRun(sceneController));
        cardSlotsList = new List<GameObject>();
    }

    // Any data that is created in Start() functions of other files needs to be accessed after the first frame
    private IEnumerator createDecksAfterStartHasRun(ParentSceneController controller)
    {
        yield return null;

        player = FindFirstObjectByType<Player>();
        if (player == null)
        {
            Debug.LogError("Player not found in scene from hand manager");
        }

        // Get player deck and create an empty discard deck and a full draw pile on load
        playerDeck = ScriptableObject.CreateInstance<DeckModelSO>();
        playerDeck.cards = new List<CardModelSO>();
        foreach (CardModelSO cardModel in GameManager.instance.getPlayerDeck()?.cards)
        {
            playerDeck.cards.Add(cardModel.clone());
        }

        drawPile = new ObservableDeck();
        drawPile.cards.AddRange(playerDeck.cards);
        drawPile.OnDeckSizeChanged += GameManager.instance.updateDrawPile;

        discardPile = new ObservableDeck();
        discardPile.OnDeckSizeChanged += GameManager.instance.updateDiscardPile;

        cardProcessor = player.getPlayerClass() switch
        {
            PlayerClass.Paladin => new PaladinCardProcessor(controller),
            PlayerClass.Mistborn => new MistbornCardProcessor(controller),
            _ => new CardProcessor(controller),
        };
        
        corruptedCards = ScriptableObject.CreateInstance<DeckModelSO>();
        corruptedCards.cards = new List<CardModelSO>();

        disabledCards = ScriptableObject.CreateInstance<DeckModelSO>();
        disabledCards.cards = new List<CardModelSO>();

        handUI.Initialize();
    }

    // General card functions
    public void setSelectedCard(GameObject card)
    {
        if (card.GetComponent<Card>() != null)
        {
            selectedCard = card.GetComponent<Card>();
            selectedCardObject = card;
        }
        else
        {
            Debug.LogError("No card component found in setSelectedCard");
        }
    }

    public void clearSelectedCard()
    {
        selectedCard = null;
        selectedCardObject = null;
    }

    public Card getSelectedCard()
    {
        return selectedCard;
    }

    public GameObject getSelectedCardObject()
    {
        return selectedCardObject;
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

    public void addDisabledCard(CardModelSO model)
    {
        disabledCards.cards.Add(model);
    }

    public List<CardModelSO> getDisabledCards()
    {
        return disabledCards.cards;
    }

    private void shuffleDiscardPileIntoDrawPile()
    {
        List<CardModelSO> discardCopy = new List<CardModelSO>(discardPile.cards);
        foreach (CardModelSO card in discardCopy)
        {
            drawPile.Add(card);
            if (!card.corrupts && !card.oneUse)
            {
                discardPile.Remove(card);
            }
        }
    }

    public void addCardToDiscardPile(Card card)
    {
        discardPile.Add(card.getCardModel());
        handUI.moveCardToDiscardPile(card);
    }

    public void litheCardPlayed(Card card)
    {
        playerDeck.cards.Remove(card.getCardModel());
    }

    public bool addCardToCurrentHand(CardModelSO card)
    {
        handUI.addCardToHand(card);
        return true;
    }

    public void addCardToPlayerDeck(CardModelSO card)
    {
        // TO-DO: add animation for card entering draw pile
        CardModelSO clonedCard = card.clone();
        playerDeck.cards.Add(clonedCard);
        drawPile.Add(clonedCard);
    }

    public void removeCardFromPlayerDeck(CardModelSO card)
    {
        playerDeck.cards.Remove(card);
        drawPile.Remove(card);
        discardPile.Remove(card);
    }

    public void removeAllOfSpecificCardFromPlayerDeck(CardModelSO card)
    {
        playerDeck.cards.RemoveAll((c) => c.title == card.title);
        drawPile.cards.RemoveAll((c) => c.title == card.title);
        discardPile.cards.RemoveAll((c) => c.title == card.title);
    }

    public CardModelSO getCardInDeckDuringEncounter(string title)
    {
        return playerDeck.cards.Find((card) => card.title == title || card.title == title + "+");
    }

    public void updateCardDisplay(Card card)
    {
        handUI.updateCardDisplay(card.getCardModel());
    }

    public List<Card> getCurrentHand()
    {
        return handUI.getCurrentHand();
    }

    // Card Processing
    public List<CardEffect> useSelectedCard(List<Enemy> enemies)
    {
        // Get player attributes
        Dictionary<EffectType, int> playerAttributes = sceneController.getPlayerAttributes();

        // Get card effects
        List<CardEffect> effects = cardProcessor.processCard(selectedCard, playerAttributes,enemies);

        // Add card to discard pile and remove card
        if (selectedCard.isLithe())
        {
            litheCardPlayed(selectedCard);
        }
        else if (selectedCard.isCorrupt())
        {
            addCorruptedCard(selectedCard.getCardModel());
        }
        else if (selectedCard.isOneUse())
        {
            addDisabledCard(selectedCard.getCardModel());
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
        handUI.addCardToHand(cardInformation);
        return true;
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
        if (lastCardPlayed != null)
        {
            cardProcessor.checkOnDeathEffect(lastCardPlayed);
        }
    }

    public bool getDiscardInProgress()
    {
        return discardInProgress;
    }

    public void setDiscardInProgress(bool inProgress)
    {
        discardInProgress = inProgress;
    }

    public void resetCardPositions()
    {
        handUI.reflowHand();
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
    Heal,
    HealOverTime,
    Stun,
    Bleed,
    Agility,
    Corruption
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
            case EffectType.HealOverTime: return "HealOverTimeEffect";
            case EffectType.Bleed: return "BleedEffect";
            case EffectType.Agility: return "AgilityEffect";
            case EffectType.Corruption: return "CorruptionEffect";
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
            case EffectType.Blind: return "+blind";
            case EffectType.Stun: return "+stun";
            case EffectType.Heal:
            case EffectType.HealDamageDone:
            case EffectType.HealOverTime: return "+hp";
            case EffectType.Bleed: return "+bleed";
            case EffectType.Agility: return "+agility";
            default: return "Default";
        }
    }
}