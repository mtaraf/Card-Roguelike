using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDeck", menuName = "Cards/Deck")]
public class DeckModelSO : ScriptableObject
{
    public List<CardModelSO> cards;
}