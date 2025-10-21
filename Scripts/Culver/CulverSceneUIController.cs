using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CulverSceneUIController : BaseLevelUIController
{
    private TextMeshProUGUI turnCount;

    public override void Initialize()
    {
        base.Initialize();

        turnCount = GameObject.FindGameObjectWithTag("RoundCount").GetComponent<TextMeshProUGUI>();

        if (turnCount == null)
        {
            Debug.LogError("Could not find culver turn counter");
        }
    }

    public void updateTurnCount(int currentTurn, int totalTurns)
    {
        turnCount.text = "Turn " + currentTurn + " / " + totalTurns;
    }
}