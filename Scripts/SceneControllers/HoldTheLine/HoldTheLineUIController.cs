using TMPro;
using UnityEngine;

public class HoldTheLineUIController : BaseLevelUIController
{
    private TextMeshProUGUI roundCount;

    public override void Initialize()
    {
        base.Initialize();

        roundCount = GameObject.FindGameObjectWithTag("RoundCount").GetComponent<TextMeshProUGUI>();

        if (roundCount == null)
        {
            Debug.LogError("Could not find round counter ui");
        }
    }

    public void updateRoundCount(int currentTurn, int totalTurns)
    {
        roundCount.text = "Round " + currentTurn + " / " + totalTurns;
    }
}
