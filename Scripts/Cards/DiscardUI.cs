using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardUI : MonoBehaviour
{
    [SerializeField] private int discardNum = 0;
    private bool discardInProgress = false;

    public void setDiscardNum(int num)
    {
        discardNum = num;
    }

    public int getDiscardNum()
    {
        return discardNum;
    }

    public void discardCard(Card card)
    {
        HandManager.instance.addCardToDiscardPile(card);
        discardNum--;
    }

    public void setInactive()
    {
        gameObject.SetActive(false);
    }

    public void startDiscard(int discardNum, GameObject cardWithDiscards)
    {
        
    }
}
