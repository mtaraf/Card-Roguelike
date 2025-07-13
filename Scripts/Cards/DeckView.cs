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
    [SerializeField] private GameObject corruptCardPrefab;


    private DeckModelSO deck;
    private List<CardModelSO> corruptedCards = new List<CardModelSO>();
    private Transform canvasTransform;

    void Start()
    {
        canvasTransform = transform.parent.transform.parent;
        deck = ScriptableObject.CreateInstance<DeckModelSO>();
        deck.cards = new List<CardModelSO>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject newDeckUI = Instantiate(deckViewUI, canvasTransform);
        GameObject deckViewScrollBarContent = Helpers.findDescendant(newDeckUI.transform, "Content");
        if (view == DeckViewType.FullDeck)
        {
            deck = HandManager.instance.getPlayerDeck();
            fillDeckView(deck, "Full Deck", deckViewScrollBarContent);
        }
        else if (view == DeckViewType.DrawPile)
        {
            deck = HandManager.instance.getDrawPile();
            Debug.Log("Draw Pile:" + deck.cards.Count);
            fillDeckView(deck, "Draw Pile", deckViewScrollBarContent);
        }
        else
        {
            deck = HandManager.instance.getDiscardPile();
            Debug.Log("Discard Pile:" + deck.cards.Count);
            fillDeckView(deck, "Discard Pile", deckViewScrollBarContent);
        }
    }

    // Fills the deck view, used to view full deck, discard pile, and draw pile
    private void fillDeckView(DeckModelSO deck, string title, GameObject content)
    {
        // Fill the title of the deck view
        TextMeshProUGUI titleObject = content.transform.Find("Title").GetComponent<TextMeshProUGUI>();
        titleObject.SetText(title);

        int index = 0;

        // Add corrupt cards if discard pile
        if (title == "Discard Pile")
        {
            corruptedCards = HandManager.instance.getCorruptedCards();
            if (corruptedCards.Count > 0)
            {
                foreach (CardModelSO model in corruptedCards)
                {
                    while (index <= content.transform.childCount - 1)
                    {
                        if (content.transform.GetChild(index).transform.childCount == 0)
                        {
                            GameObject cardObj = Instantiate(corruptCardPrefab, content.transform.GetChild(index).transform);
                            Card cardComponent = cardObj.GetComponent<Card>();
                            cardComponent.setCardDisplayInformation(model);
                            cardObj.SetActive(true);
                            index++;
                            break;
                        }
                        index++;
                    }
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
                while (index <= content.transform.childCount - 1)
                {
                    if (content.transform.GetChild(index).transform.childCount == 0)
                    {
                        GameObject cardObj = Instantiate(cardPrefab, content.transform.GetChild(index).transform);
                        Card cardComponent = cardObj.GetComponent<Card>();
                        cardComponent.setCardDisplayInformation(card);
                        cardObj.SetActive(true);
                        index++;
                        break;
                    }
                    index++;
                }
            }
        }
    }
}
