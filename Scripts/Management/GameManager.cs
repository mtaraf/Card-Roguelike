using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneBuildIndex
{
    MAIN_MENU = 0,
    BASE_LEVEL = 1,
    PATH_SELECTION = 2,
    FORGE = 3,
    HOLD_THE_LINE = 4,
    CHARACTER_SELECTION = 5
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // Controllers
    private TopBarUIManager topBarUIManager;
    private VictoryManager victoryManager;
    private int currentSaveSlot = -1;


    // Game States
    private int currentLevel;
    [SerializeField] private List<DeckModelSO> victoryCardPools; // 0: common, 1: rare, etc.
    private ParentSceneController currentSceneController;
    private List<PathOptionData> currentPathSelectionOptions;

    // Player States
    private GameObject playerObject;
    private GameObject playerDisplayObject;
    private int playerMaxHealth;
    private DeckModelSO playerDeck;
    private int playerCurrentHealth;
    private int playerHandSize;
    private int playerHandEnergy;
    private int playerGold;
    private Player player;
    private int cardRarity = 0;
    private int cardChoices = 3;

    // UI
    [SerializeField] private List<GameObject> statusPrefabs = new List<GameObject>();
    // [SerializeField] private List<GameObject> valueBasedStatuses = new List<GameObject>();

    // Cards
    [SerializeField] private DeckModelSO starterDeck;


    // Encounter Information
    private EncounterReward encounterReward;
    private EncounterType previousEncounter;

    // Scene
    private int previousScene;
    private HashSet<int> battleScenes = new HashSet<int>() { 1, 4, 6 };


    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        initializePrefabs();

        // Detect when a new scene is loaded
        SceneManager.sceneLoaded += onSceneLoaded;
    }


    private void onSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Game Manager detected scene loaded: {scene.name} + {scene.buildIndex}");
        if (scene.buildIndex != 0)
        {
            if (!loadGame(currentSaveSlot) || currentSaveSlot == -1)
            {
                    createPlayerDeckCopy();
                    playerMaxHealth = 50;
                    playerHandSize = 6;
                    playerHandEnergy = 3;
                    playerCurrentHealth = 50;
                    playerGold = 0;
                    playerHandEnergy = 3;
                    currentLevel = 0;
            }

            StartCoroutine(updateUI());
        }

        if (battleScenes.Contains(scene.buildIndex))
        {
            StartCoroutine(waitForUI());
        }
    }

    private IEnumerator updateUI()
    {
        yield return null;
        topBarUIManager = FindFirstObjectByType<TopBarUIManager>();
        if (topBarUIManager != null)
        {
            topBarUIManager.Initialize(playerGold, currentLevel, cardRarity);
        }
    }

    public void setCurrentSceneController(ParentSceneController controller)
    {
        currentSceneController = controller;
    }

    public ParentSceneController getCurrentSceneController()
    {
        return currentSceneController;
    }

    private void initializePrefabs()
    {
        statusPrefabs = new List<GameObject>(Resources.LoadAll<GameObject>("UI/Effects/EffectPrefabs"));
        //valueBasedStatuses = new List<GameObject>(Resources.LoadAll<GameObject>("UI/Effects/ValueBasedEffects"));
        starterDeck = Resources.Load<DeckModelSO>("ScriptableObjects/Cards/Mistborn/MistbornCurrentTestDeck");
        
        // Get Victory Card Pools
        victoryCardPools.Insert(0, cloneDeck(Resources.Load<DeckModelSO>("ScriptableObjects/Decks/CommonVictoryCards")));
        victoryCardPools.Insert(1, cloneDeck(Resources.Load<DeckModelSO>("ScriptableObjects/Decks/RareVictoryCards")));
        victoryCardPools.Insert(2, cloneDeck(Resources.Load<DeckModelSO>("ScriptableObjects/Decks/EpicVictoryCards")));
        victoryCardPools.Insert(3, cloneDeck(Resources.Load<DeckModelSO>("ScriptableObjects/Decks/MythicVictoryCards")));
    }

    private IEnumerator waitForUI()
    {
        yield return null;
        victoryManager = transform.AddComponent<VictoryManager>();
        victoryManager.instantiate();

        if (victoryManager == null)
        {
            Debug.LogError("Could not instatiate victory manager");
        }

        getPlayer();

        player.setDeck(playerDeck);
    }

    private void getPlayer()
    {
        currentSceneController.getPlayer();
    }

    public void updateDrawPile(int count)
    {
        currentSceneController.updateDrawPile(count);
    }

    public void updateDiscardPile(int count)
    {
        currentSceneController.updateDiscardPile(count);
    }

    private DeckModelSO cloneDeck(DeckModelSO deck)
    {
        DeckModelSO clonedDeck = ScriptableObject.CreateInstance<DeckModelSO>();
        clonedDeck.cards = new List<CardModelSO>();

        foreach (CardModelSO card in deck.cards)
        {
            clonedDeck.cards.Add(card.clone());
        }

        return clonedDeck;
    }

    public void createPlayerDeckCopy()
    {
        playerDeck = ScriptableObject.CreateInstance<DeckModelSO>();
        playerDeck.cards = new List<CardModelSO>();

        foreach (CardModelSO card in starterDeck.cards)
        {
            playerDeck.cards.Add(card.clone());
        }
    }

    public List<GameObject> getStatusPrefabs()
    {
        return statusPrefabs;
    }

    // public List<GameObject> getValueBasedStatusObject()
    // {
    //     return valueBasedStatuses;
    // }

    public List<DeckModelSO> getVictoryCardsPools()
    {
        return victoryCardPools;
    }

    public IEnumerator encounterVictory()
    {
        Debug.Log("Victory! rewards: " + encounterReward);
        AudioManager.instance.playVictory();
        addEncounterRewards();
        yield return new WaitForSeconds(1.0f);
        victoryManager.showVictoryScreen(cardChoices, cardRarity);
    }

    void addEncounterRewards()
    {
        switch (encounterReward)
        {
            case EncounterReward.Gold:
                addPlayerGold(25 +  (currentLevel * 2));
                break;
            case EncounterReward.CardRarity:
                cardRarity += 1;
                break;
            case EncounterReward.CardChoices:
                cardChoices += 1;
                break;
        }
    }

    public void addCardToPlayerDeck(CardModelSO model)
    {
        CardModelSO clone = model.clone();
        playerDeck.cards.Add(clone);
    }

    public void removeCardFromPlayerDeck(CardModelSO model)
    {
        playerDeck.cards.Remove(model);
    }

    public IEnumerator moveToPathSelection()
    {
         // update player stats
        playerCurrentHealth = player.getCurrentHealth();
        playerMaxHealth = player.getMaxHealth();

        // Save game
        yield return new WaitUntil(() => saveGame(currentSaveSlot));

        // Go to path selection scene
        SceneLoader.instance.loadScene("PathSelectionScene", () =>
        {
            Debug.Log("Scene loaded!");
        });
    }

    public int getPlayerGold()
    {
        return playerGold;
    }

    // TO-DO: add cha-ching sound
    public void addPlayerGold(int value)
    {
        playerGold += value;
        topBarUIManager.updateGoldCount(playerGold);
    }

    // TO-DO: add gold spending sound
    public void subtractPlayerGold(int value)
    {
        playerGold -= value;
        topBarUIManager.updateGoldCount(playerGold);
    }

    public int getPlayerHandSize()
    {
        return playerHandSize;
    }

    public int getPlayerHandEnergy()
    {
        return playerHandEnergy;
    }

    public void setPlayerCharacter(GameObject obj)
    {
        playerObject = obj;
    }

    public GameObject getPlayerCharacter()
    {
        return playerObject;
    }

    public void setPlayerDisplayObject(GameObject display)
    {
        playerDisplayObject = display;
    }
    
    public GameObject getPlayerDisplayObject()
    {
        return playerDisplayObject;
    }

    public int getCurrentLevel()
    {
        return currentLevel;
    }

    public void incrementCurrentLevel()
    {
        currentLevel++;
    }    

    public void setPlayer(Player currentPlayer)
    {
        player = currentPlayer;
    }

    public void endTurn()
    {
        TurnManager.instance.endTurnButtonPressed();
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

    public DeckModelSO getStarterDeck()
    {
        return starterDeck;
    }

    public void loadEncounterTypeAndRewards(EncounterType type, EncounterReward reward)
    {
        encounterReward = reward;
        switch (type)
        {
            case EncounterType.Forge:
                //SceneLoader.instance.loadScene(3);
                break;
            case EncounterType.Regular_Encounter:
                SceneLoader.instance.loadScene("BaseLevelScene");
                break;
            case EncounterType.Mini_Boss_Encounter:
                // TODO: Change this
                SceneLoader.instance.loadScene("BaseLevelScene");
                break;
            case EncounterType.Final_Boss:
                // TODO: Change this
                SceneLoader.instance.loadScene("BaseLevelScene");
                break;
            case EncounterType.Culver_Encounter:
                SceneLoader.instance.loadScene("CulverScene");
                break;
            case EncounterType.Hold_The_Line_Encounter:
                SceneLoader.instance.loadScene("HoldTheLine");
                break;
        }
        StartCoroutine(updateUI());
    }

    public EncounterType getPreviousEncounter()
    {
        return previousEncounter;
    }

    public void setPathOptions(List<PathOptionData> options)
    {
        currentPathSelectionOptions = options;
    }

    public List<PathOptionData> getPathOptions()
    {
        return currentPathSelectionOptions;
    }

    // Save Functions
    public bool checkForSavedGames(int saveSlot)
    {
        return SaveSystem.saveFileExists(saveSlot);
    }

    public string getSaveSlotSummary(int saveSlot)
    {
        return SaveSystem.getSlotSummary(saveSlot);
    }

    public string getSaveSlotTitle(int saveSlot)
    {
        return SaveSystem.getSlotTitle(saveSlot);
    }

    public bool deleteSave(int saveSlot)
    {
        return SaveSystem.deleteSave(saveSlot);
    }

    public bool saveGame(int saveSlot)
    {
        Tuple<float,float> currentVolume = AudioManager.instance.getAudioVolumes();

        SaveData saveData = new SaveData();
        saveData.currentLevel = currentLevel;
        saveData.playerCurrentHealth = playerCurrentHealth;
        saveData.playerMaxHealth = playerMaxHealth;
        saveData.playerGold = playerGold;
        saveData.playerHandSize = playerHandSize;
        saveData.playerHandEnergy = playerHandEnergy;
        saveData.playerCards = new List<SerializableCardModel>();
        saveData.playerPrefab = playerObject;
        saveData.playerDisplay = playerDisplayObject;
        saveData.pathOptions = currentPathSelectionOptions;
        saveData.musicVolume = currentVolume.Item1;
        saveData.sfxVolume = currentVolume.Item2;


        foreach (CardModelSO model in playerDeck.cards)
        {
            saveData.playerCards.Add(SaveSystem.convertToSerializableModel(model));
        }

        SaveSystem.saveGame(saveData, saveSlot);

        return true;
    }

    private bool loadGame(int saveSlot)
    {
        SaveData data = SaveSystem.loadGame(saveSlot);
        if (data == null)
        {
            return false;
        }

        currentLevel = data.currentLevel;
        playerCurrentHealth = data.playerCurrentHealth;
        playerMaxHealth = data.playerMaxHealth;
        playerGold = data.playerGold;
        playerHandSize = data.playerHandSize;
        playerHandEnergy = data.playerHandEnergy;
        playerDeck = ScriptableObject.CreateInstance<DeckModelSO>();
        playerDeck.cards = new List<CardModelSO>();
        currentPathSelectionOptions = data.pathOptions;
        AudioManager.instance.setBackgroundMusicVolume(data.musicVolume);
        AudioManager.instance.setSFXVolume(data.sfxVolume);

        playerObject = data.playerPrefab;
        playerDisplayObject = data.playerDisplay;

        foreach (SerializableCardModel serializableCardModel in data.playerCards)
        {
            playerDeck.cards.Add(SaveSystem.convertToRuntimeCard(serializableCardModel));
        }

        return true;
    }

    public void setCurrentSaveSlot(int slot)
    {
        currentSaveSlot = slot;
    }

    public void loadScene(int sceneNumber)
    {
        previousScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(sceneNumber);
    }

    public int getPreviousSceneNumber()
    {
        return previousScene;
    }

    public void quitGame()
    {
        saveGame(currentSaveSlot);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
    }

    public void exitToMainMenu()
    {
        saveGame(currentSaveSlot);

        loadScene(0);
    }
}
