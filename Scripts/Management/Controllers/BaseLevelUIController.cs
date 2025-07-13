using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BaseLevelUIController : MonoBehaviour
{
    [SerializeField] private GameObject goldCount;
    [SerializeField] private GameObject levelCount;
    private GameObject endTurnButton;

    public void Initialize()
    {
        goldCount = GameObject.FindGameObjectWithTag("GoldCount");
        levelCount = GameObject.FindGameObjectWithTag("LevelCount");
        endTurnButton = GameObject.FindGameObjectWithTag("EndTurnButton");

        if (endTurnButton == null)
        {
            Debug.LogError("Could not find end turn button");
        }

        endTurnButton.GetComponent<Button>().onClick.AddListener(GameManager.instance.endTurn);
    }

    public void updateGoldCount(int gold)
    {
        goldCount.GetComponent<TextMeshProUGUI>().text = gold.ToString();
    }

    public void updateLevelCount(int level)
    {
        levelCount.GetComponent<TextMeshProUGUI>().text = "Level " + level.ToString();
    }
}