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

    [SerializeField] private Character character;
    [SerializeField] private GameObject deckSizeText;

    private Deck deck;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        deck = character.getDeck();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Deck Clicked");
        if (view == DeckViewType.FullDeck)
        {

        }
    }
}
