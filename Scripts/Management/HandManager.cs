using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HandManager : MonoBehaviour
{
    public static HandManager instance;

    [SerializeField] private Player player;
    [SerializeField] public GameObject cardPrefab;
    [SerializeField] public GameObject noAnimationCardPrefab;
    [SerializeField] private GameObject centerOfUI;
    [SerializeField] private GameObject feedbackMessage;
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

        centerOfUI = GameObject.FindGameObjectWithTag("CenterOfUI");

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

    // Instantiate and play animation for enemy cards
    public IEnumerator enemyCardAnimation(CardModelSO model)
    {
        // Instantiate at the center of the screen
        GameObject card = Instantiate(noAnimationCardPrefab, centerOfUI.transform);
        Card cardInfo = card.GetComponent<Card>();
        cardInfo.setCardDisplayInformation(model);

        CanvasGroup canvasGroup = card.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = card.AddComponent<CanvasGroup>();
        }

        //processEnemyCard(model);

        yield return new WaitForSeconds(1.0f);

        // Step 1: Fade out
        float fadeDuration = 2f;
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            float t = elapsedTime / fadeDuration;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        // Destroy the card after it fades out
        Destroy(card);
    }

    public IEnumerator displayFeedbackMessage(string message)
    {
        Debug.Log("Display Message");

        GameObject textObj = Instantiate(feedbackMessage, centerOfUI.transform);
        textObj.GetComponent<TextMeshProUGUI>().text = message;

        yield return new WaitForSeconds(1.0f);

        Destroy(textObj);
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