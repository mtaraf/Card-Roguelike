using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Button close;
    [SerializeField] private Button gameSettings;
    [SerializeField] private Button exitGame;
    [SerializeField] private Button backButton;
    [SerializeField] private Button mainMenu;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private GameObject mainSettingsPage;
    [SerializeField] private GameObject gameSettingsPage;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    void Start()
    {
        close.onClick.RemoveAllListeners();
        close.onClick.AddListener(() => { gameObject.SetActive(false); });

        exitGame.onClick.RemoveAllListeners();
        exitGame.onClick.AddListener(() => GameManager.instance.quitGame());

        mainMenu.onClick.RemoveAllListeners();
        mainMenu.onClick.AddListener(() => GameManager.instance.exitToMainMenu());

        gameSettings.onClick.RemoveAllListeners();
        gameSettings.onClick.AddListener(() => showGameSettingsPage());

        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(() => showMainSettingsPage());

        musicSlider.value = AudioManager.instance.getBackgroundMusicVolume();
        sfxSlider.value = AudioManager.instance.getSFXVolume();

        musicSlider.onValueChanged.RemoveAllListeners();
        musicSlider.onValueChanged.AddListener((value) => AudioManager.instance.setBackgroundMusicVolume(value));

        sfxSlider.onValueChanged.RemoveAllListeners();
        sfxSlider.onValueChanged.AddListener((value) => AudioManager.instance.setSFXVolume(value));

        gameSettingsPage.SetActive(false);
    }

    public void showGameSettingsPage()
    {
        mainSettingsPage.SetActive(false);
        gameSettingsPage.SetActive(true);
        title.text = "Game Settings";
    }

    public void showMainSettingsPage()
    {
        mainSettingsPage.SetActive(true);
        gameSettingsPage.SetActive(false);
        title.text = "Settings";
    }
}