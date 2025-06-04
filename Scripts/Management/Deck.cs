using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField] public int maxDeckSize = 20;
    [SerializeField] public int deckSize = 0;
    [SerializeField] public List<CardModelSO> deck;
    [SerializeField] public GameObject cardPrefab;

    [SerializeField] private GameObject deckSizeText;

    [SerializeField] private GameObject cardSlotContainer;

    private List<GameObject> cardSlots = new List<GameObject> { };


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get card slots
        foreach (Transform child in cardSlotContainer.transform)
        {
            cardSlots.Add(child.gameObject);
        }

        deckSizeText.GetComponent<TextMeshProUGUI>().text = deckSize.ToString();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseDown()
    {
        Debug.Log("Deck clicked");
    }

    void OnMouseEnter()
    {
        Debug.Log("Deck Hovered");
    }

    void AddCardToDeck(CardModelSO card)
    {
        if (deckSize != maxDeckSize)
        {
            deck.Add(card);
        }
    }

    void drawCard(CardModelSO card)
    {
        foreach (GameObject slot in cardSlots)
        {
            if (slot.transform.childCount == 0)
            {
                // Add Card to this slot

                break;
            }
        }
    }
}
