using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // Menu
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject loadGameMenu;


    // Game States
    private Character player;
    private List<CardModelSO> drawPile;
    private List<CardModelSO> discardPile;
    private List<CardModelSO> characterDeck;

    // Card States
    private GameObject selectedCard = null;


    // Private data
    private string pathToSaveFiles = "../../Saves/";


    public void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Detect when a new scene is loaded
        SceneManager.sceneLoaded += onSceneLoaded;
    }


    public void onSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Game Manager detected scene loaded: {scene.name}");

        // Add scene specific logic here
    }

    // Game Functions
    public void setSelectedCard(GameObject card)
    {
        selectedCard = card;
    }

    public void clearSelectedCard()
    {
        selectedCard = null;
    }

    public GameObject getSelectedCard()
    {
        return selectedCard;
    }



    // Save Functions

    // Menu functions
    public void playButtonClicked()
    {
        mainMenu.gameObject.SetActive(false);
        loadGameMenu.gameObject.SetActive(true);
        Debug.Log("Play clicked!");
    }

    private void checkForSavedGames()
    {

    }

    private void saveGame()
    {
        
    }

    public void enterGame(int saveSlot)
    {
        // Add checks for save slots

        SceneManager.LoadScene(1);
    }

    public void openSettings()
    {
        Debug.Log("Open Settings");
        mainMenu.gameObject.SetActive(false);
        optionsMenu.gameObject.SetActive(true);
    }

    public void returnToMainMenu()
    {
        loadGameMenu.gameObject.SetActive(false);
        optionsMenu.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(true);
    }

    public void quitGame()
    {
        Debug.Log("Quit Game");

        // Save everything here

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
