using System.Collections;
using TMPro;
using UnityEngine;

public class BaseLevelUIController : MonoBehaviour
{
    [SerializeField] private GameObject goldCount;
    [SerializeField] private GameObject levelCount;

    public void Initialize()
    {
        goldCount = GameObject.FindGameObjectWithTag("GoldCount");
        levelCount = GameObject.FindGameObjectWithTag("LevelCount");
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