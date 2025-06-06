using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public enum DeckViewType
{
    FullDeck,
    DiscardPile,
    DrawPile
}

public class DeckView : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private DeckViewType view;

    [SerializeField] private Character player;
    [SerializeField] private GameObject deckSizeText;
    [SerializeField] private GameObject deckViewScrollBarContent; // Fill this with title and cards within the deck
    [SerializeField] private GameObject deckView; // Turn on and off when viewing deck
    [SerializeField] private GameObject cardPrefab;

    // For Testing purposes, remove later
    [SerializeField] public DeckModelSO testDeck;
    private Deck deck;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (player)
        {
            deck = player.getDeck();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Deck Clicked");
        fillDeckView(deck, "Full Deck");
        if (view == DeckViewType.FullDeck)
        {

        }
    }

    // Fills the deck view, used to view full deck, discard pile, and draw pile
    private void fillDeckView(Deck deck, string title)
    {
        // Fill the title of the deck view
        TextMeshProUGUI titleObject = deckViewScrollBarContent.gameObject.GetComponentInChildren<TextMeshProUGUI>();
        titleObject.SetText(title);

        // Fill the cards of the deck view
        int index = 0;
        foreach (CardModelSO card in testDeck.cards)
        {
            while (index <= deckViewScrollBarContent.transform.childCount - 1)
            {
                if (deckViewScrollBarContent.transform.GetChild(index).transform.childCount == 0)
                {
                    // Create Card Prefab and fill with data from card
                    GameObject cardDisplay = Instantiate(cardPrefab);
                    cardDisplay.transform.Find("Title").GetComponent<TextMeshProUGUI>().SetText(card.title);
                    cardDisplay.transform.Find("Description").GetComponent<TextMeshProUGUI>().SetText(card.details);
                    cardDisplay.transform.Find("EnergyTextContainer").transform.Find("EnergyCost").GetComponent<TextMeshProUGUI>().SetText(card.energy.ToString());

                    // Add object as a child to the open card slot
                    cardDisplay.transform.SetParent(deckViewScrollBarContent.transform.GetChild(index).transform);
                    cardDisplay.transform.localPosition = Vector3.zero;

                    index++;
                    break;
                }
                index++;
            }
        }

        // Set the deck view to active
        deckView.gameObject.SetActive(true);
    }
}
