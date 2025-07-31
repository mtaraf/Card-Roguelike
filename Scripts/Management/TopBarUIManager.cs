using TMPro;
using UnityEngine;

public class TopBarUIManager : MonoBehaviour
{
    [SerializeField] private GameObject goldCount;
    [SerializeField] private GameObject levelCount;
    private GameObject settingsMenuPrefab;
    private GameObject settingsMenuObject;
    private SettingsMenu settingsMenu;

    public void Initialize(int gold, int level)
    {
        goldCount = GameObject.FindGameObjectWithTag("GoldCount");
        levelCount = GameObject.FindGameObjectWithTag("LevelCount");
        settingsMenuPrefab = Resources.Load<GameObject>("UI/General/SettingsMenu");

        if (goldCount == null || levelCount == null)
        {
            Debug.LogError("Could not find UI for top bar");
        }

        goldCount.GetComponent<TextMeshProUGUI>().text = gold.ToString();
        levelCount.GetComponent<TextMeshProUGUI>().text = "Level " + level.ToString();

        settingsMenuObject = Instantiate(settingsMenuPrefab, transform.parent.transform);

        if (settingsMenuObject == null)
        {
            Debug.LogError("Could not create settings menu");
        }
        settingsMenu = settingsMenuObject.GetComponent<SettingsMenu>();
        settingsMenuObject.SetActive(false);
    }

    public void updateGoldCount(int gold)
    {
        goldCount.GetComponent<TextMeshProUGUI>().text = gold.ToString();
    }

    public void openSettings()
    {
        settingsMenuObject.SetActive(true);
    }

    public void closeSettings()
    {
        settingsMenuObject.SetActive(false);
    }

    public void openGameSettings()
    {
        settingsMenu.showGameSettingsPage();
    }
}