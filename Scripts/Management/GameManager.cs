using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // Menu
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject loadGameMenu;


    // Game States
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private List<GameObject> enemyPrefabs;
    // [SerializeField] private List<GameObject> bossPrefabs;
    private int currentLevel = 0;
    private DeckModelSO playerDeck;
    private int playerMaxHealth;
    private int playerCurrentHealth;
    private int playerHandSize;
    private int playerHandEnergy;
    private int currentPlayerEnergy;

    // UI
    [SerializeField] private List<GameObject> statuses = new List<GameObject>();

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

        // Initial player values
        playerDeck = starterDeck;
        playerMaxHealth = 50;
        playerHandSize = 6;
        playerHandEnergy = 3;
        playerCurrentHealth = 50;
    }


    private void onSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Game Manager detected scene loaded: {scene.name}");
    }

    public List<GameObject> getStatusObjects()
    {
        return statuses;
    }

    public void encounterVictory()
    {
        VictoryManager victoryManager = FindFirstObjectByType<VictoryManager>();
        victoryManager.showVictoryScreen();
    }

    public void addCardToPlayerDeck(CardModelSO model)
    {
        playerDeck.cards.Add(model);
    }


    public void moveToNextEncounter()
    {
        SceneLoader.instance.loadScene(1, () =>
        {
            Debug.Log("Scene loaded!");
        });
    }

    public int getPlayerHandSize()
    {
        return playerHandSize;
    }

    public int getPlayerHandEnergy()
    {
        return playerHandEnergy;
    }

    public int getCurrentLevel()
    {
        return currentLevel;
    }

    public void levelCompleted()
    {
        currentLevel++;
    }

    public void endTurn()
    {
        TurnManager.instance.endTurnButtonPressed();
    }

    public int getCurrentPlayerEnergy()
    {
        return currentPlayerEnergy;
    }

    public DeckModelSO getPlayerDeck()
    {
        return playerDeck;
    }

    public int getPlayerMaxHealth()
    {
        return playerMaxHealth;
    }

    public int getPlayerCurrentHealth()
    {
        return playerCurrentHealth;
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

        // Save everything here

        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                    Application.Quit();
        #endif
    }
}
