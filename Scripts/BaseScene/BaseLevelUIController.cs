using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BaseLevelUIController : MonoBehaviour
{
    private GameObject endTurnButton;

    public void Initialize()
    {
        endTurnButton = GameObject.FindGameObjectWithTag("EndTurnButton");

        if (endTurnButton == null)
        {
            Debug.LogError("Could not find end turn button");
        }

        endTurnButton.GetComponent<Button>().onClick.AddListener(GameManager.instance.endTurn);
    }
}