using TMPro;
using UnityEngine;

public class TopBarUIManager : MonoBehaviour
{
    [SerializeField] private GameObject goldCount;
    [SerializeField] private GameObject levelCount;

    public void Initialize(int gold, int level)
    {
        goldCount = GameObject.FindGameObjectWithTag("GoldCount");
        levelCount = GameObject.FindGameObjectWithTag("LevelCount");

        if (goldCount == null || levelCount == null)
        {
            Debug.LogError("Could not find end turn button");
        }

        goldCount.GetComponent<TextMeshProUGUI>().text = gold.ToString();
        levelCount.GetComponent<TextMeshProUGUI>().text = "Level " + level.ToString();
    }

    public void updateGoldCount(int gold)
    {
        goldCount.GetComponent<TextMeshProUGUI>().text = gold.ToString();
    }
}