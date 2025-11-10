using UnityEngine;
using UnityEngine.UI;

public class MainMenuSceneController : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject loadGameMenu;
    private SaveSlot[] saveSlots = new SaveSlot[3];
    private Slider musicSlider;
    private Slider sfxSlider;

    void Start()
    {
        
        mainMenu ??= GameObject.FindGameObjectWithTag("MainMenuUI");
        optionsMenu ??= GameObject.FindGameObjectWithTag("MainOptionsMenuUI");
        loadGameMenu ??= GameObject.FindGameObjectWithTag("LoadGameMenuUI");


        // Get sliders in options menu
        musicSlider = GameObject.Find("MusicVolumeSlider").GetComponent<Slider>();
        sfxSlider = GameObject.Find("SFXVolumeSlider").GetComponent<Slider>();

        if (musicSlider == null || sfxSlider == null)
        {
            Debug.LogError("Could not find options menu sliders");
        }

        // TO-DO: add this to global save data to retain volume settings
        changeMusicVolume();
        changeSFXVolume();

        // Fill save slot information if it exists
        for (int i = 0; i < loadGameMenu.transform.childCount; i++)
        {
            SaveSlot saveSlot = loadGameMenu.transform.GetChild(i).GetComponent<SaveSlot>();
            if (saveSlot != null)
            {
                int slotNumber = saveSlot.getSlotNumber();
                if (GameManager.instance.checkForSavedGames(slotNumber))
                {
                    string details = GameManager.instance.getSaveSlotSummary(slotNumber);
                    string title = GameManager.instance.getSaveSlotTitle(slotNumber);
                    saveSlot.setSaveSlotInformation(title, details, () => enterGame(slotNumber));
                }
                else
                {
                    saveSlot.setSaveSlotInformation("Save Slot " + slotNumber, "", () => enterGame(slotNumber));
                }
                saveSlots[i] = saveSlot;
            }
        }

        mainMenu.gameObject.SetActive(true);
        optionsMenu.gameObject.SetActive(false);
        loadGameMenu.gameObject.SetActive(false);
    }

    public void openLoadGameScreen()
    {
        mainMenu.gameObject.SetActive(false);
        loadGameMenu.gameObject.SetActive(true);
        AudioManager.instance.playClick();
    }

    public void enterGame(int saveSlot)
    {
        GameManager.instance.setCurrentSaveSlot(saveSlot);

        // Check for SaveSlot
        if (!GameManager.instance.checkForSavedGames(saveSlot))
        {
            // Navigate to Character Selection
            GameManager.instance.loadScene((int)SceneBuildIndex.CHARACTER_SELECTION);
        }
        else
        {
            // TO-DO: Navigate to save slot level
            
        }
        AudioManager.instance.playClick();
    }

    public void returnToMainMenu()
    {
        loadGameMenu.gameObject.SetActive(false);
        optionsMenu.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(true);
        AudioManager.instance.playClick();
    }

    public void openSettings()
    {
        mainMenu.gameObject.SetActive(false);
        optionsMenu.gameObject.SetActive(true);
        AudioManager.instance.playClick();
    }

    public void quitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
    }

    public void changeMusicVolume()
    {
        AudioManager.instance.setBackgroundMusicVolume(musicSlider.value);
    }
    
    public void changeSFXVolume()
    {
        AudioManager.instance.setSFXVolume(sfxSlider.value);
    }
}
