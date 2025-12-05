using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BaseLevelUIController : MonoBehaviour
{
    private GameObject endTurnButton;
    private DeckView drawPile;
    private DeckView discardPile;

    public virtual void Initialize()
    {
        endTurnButton = GameObject.FindGameObjectWithTag("EndTurnButton");
        GameObject drawPileObj = GameObject.FindGameObjectWithTag("DrawPile");
        GameObject discardPileObj = GameObject.FindGameObjectWithTag("DiscardPile");

        if (endTurnButton == null || drawPileObj == null || discardPileObj == null)
        {
            Debug.LogError("Could not find end turn button");
        }

        endTurnButton.GetComponent<Button>().onClick.AddListener(GameManager.instance.endTurn);
        drawPile = drawPileObj.GetComponent<DeckView>();
        discardPile = discardPileObj.GetComponent<DeckView>();
        discardPile.setDeckCount(0);
    }

    public void updateDrawPile(int count)
    {
        drawPile.setDeckCount(count);
    }

    public void updateDiscardPile(int count)
    {
        discardPile.setDeckCount(count);
    }
}