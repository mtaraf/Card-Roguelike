using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

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
    [SerializeField] private GameObject deckViewUI; // Turn on and off when viewing deck
    [SerializeField] private GameObject cardPrefab;

    // For Testing purposes, remove later
    [SerializeField] public DeckModelSO testDeck;


    private Deck deck;
    private GameObject deckViewScrollBarContent; // Fill this with title and cards within the deck
    private Transform canvasTransform;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Find the scroll bar content
        deckViewScrollBarContent = findDescendant(deckViewUI.gameObject.transform, "Content");
        canvasTransform = transform.parent.transform.parent;

        if (player)
        {
            deck = player.getDeck();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Deck Clicked");
        Instantiate(deckViewUI, canvasTransform);
        if (view == DeckViewType.FullDeck)
        {
            fillDeckView(deck, "Full Deck");
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
                if (deckViewScrollBarContent.transform.GetChild(index).transform.childCount == 1)
                {
                    GameObject cardObj = deckViewScrollBarContent.transform.GetChild(index).transform.Find("CardNoAnimationPrefab").gameObject;
                    cardObj.transform.Find("Title").GetComponent<TextMeshProUGUI>().SetText(card.title);
                    cardObj.transform.Find("Description").GetComponent<TextMeshProUGUI>().SetText(card.details);
                    cardObj.transform.Find("EnergyTextContainer").transform.Find("EnergyCost").GetComponent<TextMeshProUGUI>().SetText(card.energy.ToString());

                    cardObj.SetActive(true);

                    index++;
                    break;
                }
                index++;
            }
        }
    }

    // Move this to a helper script eventually
    private GameObject findDescendant(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == "Content")
                return child.gameObject;

            GameObject found = findDescendant(child, "Content");
            if (found != null)
                return found;
        }
        return null;
    }
}
