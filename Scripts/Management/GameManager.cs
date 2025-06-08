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
    private Player player;

    // Cards
    [SerializeField] private DeckModelSO starterDeck;


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

    // Card/Deck Functions
    public DeckModelSO getStarterDeck()
    {
        return starterDeck;
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
