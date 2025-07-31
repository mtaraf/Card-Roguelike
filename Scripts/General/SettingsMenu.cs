using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Button close;
    [SerializeField] private Button gameSettings;
    [SerializeField] private Button exitGame;
    [SerializeField] private Button mainMenu;

    private TopBarUIManager topBarUIManager;

    void Start()
    {
        topBarUIManager = FindFirstObjectByType<TopBarUIManager>();

        if (topBarUIManager == null)
        {
            Debug.LogError("Could not find topbarmanager for settings menu");
        }

        close.onClick.RemoveAllListeners();
        close.onClick.AddListener(() => { gameObject.SetActive(false); });

        exitGame.onClick.RemoveAllListeners();
        exitGame.onClick.AddListener(() => GameManager.instance.quitGame());

        mainMenu.onClick.RemoveAllListeners();
        mainMenu.onClick.AddListener(() => GameManager.instance.exitToMainMenu());

        gameSettings.onClick.RemoveAllListeners();
        gameSettings.onClick.AddListener(() => showGameSettingsPage());
    }

    public void showGameSettingsPage()
    {

    }
    
}