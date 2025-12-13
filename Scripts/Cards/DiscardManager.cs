using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiscardManager : MonoBehaviour
{
    public static DiscardManager instance;
    public int requiredCount;
    public List<Card> selectedCards = new List<Card>();
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private GameObject background;
    [SerializeField] private TextMeshProUGUI cardCounter;
    private GameObject discardCard;
    private List<Enemy> cardEnemies;
    private Player cardPlayer;



    // private bool discardInProgress = false;
    // private List<GameObject> cardsToBeDiscarded = new List<GameObject>();
    // private GameObject currentCardPlayed;

    public void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        confirmButton.onClick.AddListener(() => confirmDiscard());
        cancelButton.onClick.AddListener(() => cancelDiscard());

        setComponents(false);
    }

    public void startDiscard(int required, GameObject cardUsed, List<Enemy> enemies, Player player)
    {
        discardCard = cardUsed;
        cardEnemies = enemies;
        cardPlayer = player;
        cardUsed.transform.localPosition = new Vector3(cardCounter.transform.localPosition.x, cardCounter.transform.localPosition.y + 250, cardCounter.transform.localPosition.z);
        cardUsed.GetComponent<CardSelectionHandler>().enabled = false;

        HandManager.instance.clearSelectedCard();
        HandManager.instance.setDiscardInProgress(true);
        requiredCount = required;
        selectedCards.Clear();

        setComponents(true);
        updateCounter();
    }

    public void cancelDiscard()
    {
        // Reset and toggled cards for discard to original position
        resetToggledCardPositions();

        // Reset discard card
        discardCard.GetComponent<CardSelectionHandler>().enabled = true;
        discardCard.GetComponent<CardSelectionHandler>().returnToStartPos();

        // Clean up
        HandManager.instance.setDiscardInProgress(false);
        selectedCards.Clear();
        setComponents(false);
    }

    public void resetToggledCardPositions()
    {
        foreach (Card card in selectedCards)
        {
            CardSelectionHandler cardSelectionHandler = card.gameObject.GetComponent<CardSelectionHandler>();
            cardSelectionHandler.returnToStartPos();
            cardSelectionHandler.setToggledForDiscard(false);
        }
    }

    public bool maxCardsSelected()
    {
        Debug.Log(selectedCards.Count);
        return requiredCount == selectedCards.Count;
    }

    public void toggleCard(Card card)
    {
        Debug.Log($"{card.getCardTitle()} toggled for discard");

        if (selectedCards.Contains(card))
            selectedCards.Remove(card);
        else
            selectedCards.Add(card);

        updateCounter();
    }

    public void confirmDiscard()
    {
        if (selectedCards.Count == requiredCount)
        {
            // TO-DO: Add feedback error message
            foreach (Card card in selectedCards)
            {
                HandManager.instance.addCardToDiscardPile(card);
            }
        }

        // Use card
        Card cardComponent = discardCard.gameObject.GetComponent<Card>();
        discardCard.gameObject.GetComponent<CardSelectionHandler>().continueCardUse(cardComponent,cardEnemies, cardPlayer, cardComponent.getEnergy(), true);

        setComponents(false);
        HandManager.instance.setDiscardInProgress(false);
    }

    public void setComponents(bool active)
    {
        background.SetActive(active);
        confirmButton.gameObject.SetActive(active);
        cancelButton.gameObject.SetActive(active);
        cardCounter.gameObject.SetActive(active);
    }

    private void updateCounter()
    {
        cardCounter.text = $"Selected: {selectedCards.Count} \nRequired: {requiredCount}";

        // Check if required count is met, activate confirm button
        if (requiredCount == selectedCards.Count)
            confirmButton.interactable = true;
        else
            confirmButton.interactable = false;
    }
}
