using System;
using System.Collections.Generic;

[Serializable]
public class ObservableDeck
{
    public List<CardModelSO> cards = new();
    public event Action<int> OnDeckSizeChanged;

    public void Add(CardModelSO card)
    {
        cards.Add(card);
        OnDeckSizeChanged?.Invoke(cards.Count);
    }

    public void Remove(CardModelSO card)
    {
        cards.Remove(card);
        OnDeckSizeChanged?.Invoke(cards.Count);
    }

    public void RemoveAt(int index)
    {
        cards.RemoveAt(index);
        OnDeckSizeChanged?.Invoke(cards.Count);
    }

    public void Clear()
    {
        cards.Clear();
        OnDeckSizeChanged?.Invoke(cards.Count);
    }

    public int Count => cards.Count;

    public CardModelSO this[int index] => cards[index];
}