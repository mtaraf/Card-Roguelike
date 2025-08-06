using System.Collections.Generic;
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
    [SerializeField] private GameObject deckViewUI;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private TextMeshProUGUI currentDeckCount;


    private DeckModelSO deck;
    private List<CardModelSO> corruptedCards = new List<CardModelSO>();
    private Transform canvasTransform;

    void Start()
    {
        GameObject overlayCanvas = GameObject.FindGameObjectWithTag("OverlayCanvas");

        if (overlayCanvas == null)
        {
            Debug.LogError("Could not find overlay canvas for deck view");
        }

        canvasTransform = overlayCanvas.transform;
        deck = ScriptableObject.CreateInstance<DeckModelSO>();
        deck.cards = new List<CardModelSO>();

        if (currentDeckCount == null && view != DeckViewType.FullDeck)
        {
            Debug.LogError("Could not find UI for deck view");
        }

        if (HandManager.instance == null)
        {
            gameObject.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject newDeckUI = Instantiate(deckViewUI, canvasTransform);
        GameObject deckViewContent = Helpers.findDescendant(newDeckUI.transform, "Content");

        if (view == DeckViewType.FullDeck)
        {
            deck = HandManager.instance.getPlayerDeck();
            fillDeckView(deck, "Full Deck", deckViewContent, newDeckUI);
        }
        else if (view == DeckViewType.DrawPile)
        {
            deck.cards = HandManager.instance.getDrawPile().cards;
            fillDeckView(deck, "Draw Pile", deckViewContent, newDeckUI);
        }
        else
        {
            deck.cards = HandManager.instance.getDiscardPile().cards;
            fillDeckView(deck, "Discard Pile", deckViewContent, newDeckUI);
        }
    }

    // Fills the deck view, used to view full deck, discard pile, and draw pile
    private void fillDeckView(DeckModelSO deck, string title, GameObject content, GameObject deckViewObj)
    {
        // Fill the title of the deck view
        TextMeshProUGUI titleObject = deckViewObj.transform.Find("Title").GetComponent<TextMeshProUGUI>();
        titleObject.SetText(title);

        int deckSize = deck.cards.Count;
        int rows = deckSize / 5;

        // Adjust the scroll view height
        RectTransform scrollViewContentRect = content.GetComponent<RectTransform>();
        scrollViewContentRect.sizeDelta = new Vector2(scrollViewContentRect.sizeDelta.x, rows * 800);

        // Add corrupt cards if discard pile
        if (title == "Discard Pile")
        {
            corruptedCards = HandManager.instance.getCorruptedCards();
            if (corruptedCards.Count > 0)
            {
                scrollViewContentRect.sizeDelta = new Vector2(scrollViewContentRect.sizeDelta.x, scrollViewContentRect.sizeDelta.y + corruptedCards.Count / 5 * 800);
                foreach (CardModelSO model in corruptedCards)
                {
                    GameObject cardObj = Instantiate(cardPrefab, content.transform);
                    Card cardComponent = cardObj.GetComponent<Card>();
                    cardComponent.setCardDisplayInformation(model);
                    cardObj.SetActive(true);
                    break;
                }
            }
        }

        if (deck.cards.Count == 0)
        {
            // No cards for this deck view, display empty
            // TO-DO: Add empty tag where cards are usually
        }
        else
        {
            // Fill the cards of the deck view
            foreach (CardModelSO card in deck.cards)
            {
                GameObject cardObj = Instantiate(cardPrefab, content.transform);
                Card cardComponent = cardObj.GetComponent<Card>();
                cardComponent.setCardDisplayInformation(card);
                cardObj.SetActive(true);
            }
        }
    }

    public void setDeckCount(int count)
    {
        currentDeckCount.text = count.ToSafeString();
    }
}
