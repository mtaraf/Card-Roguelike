using UnityEngine;

public class MainMenuSceneController : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject loadGameMenu;
    private string[] saveGameDetails = new string[3];
    private SaveSlot[] saveSlots = new SaveSlot[3];

    void Start()
    {
        
        mainMenu ??= GameObject.FindGameObjectWithTag("MainMenuUI");
        optionsMenu ??= GameObject.FindGameObjectWithTag("MainOptionsMenuUI");
        loadGameMenu ??= GameObject.FindGameObjectWithTag("LoadGameMenuUI");

        mainMenu.gameObject.SetActive(true);
        optionsMenu.gameObject.SetActive(false);
        loadGameMenu.gameObject.SetActive(false);

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
                saveSlots[i] = saveSlot;
            }
        }
    }

    public void enterGame(int saveSlot)
    {
        GameManager.instance.setCurrentSaveSlot(saveSlot);
        GameManager.instance.loadScene(1);
    }
    
    public void returnToMainMenu()
    {
        loadGameMenu.gameObject.SetActive(false);
        optionsMenu.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(true);
    }
    
    public void openSettings()
    {
        mainMenu.gameObject.SetActive(false);
        optionsMenu.gameObject.SetActive(true);
    }
    
    public void quitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
    }
}
