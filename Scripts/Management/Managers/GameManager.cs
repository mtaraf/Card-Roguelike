using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // Controllers
    private TopBarUIManager topBarUIManager;
    private VictoryManager victoryManager;
    private int currentSaveSlot = -1;


    // Game States
    private int currentLevel = 1;
    private ParentSceneController currentSceneController;
    private List<PathOptionData> currentPathSelectionOptions;

    // Player States
    private GameObject playerObject;
    private GameObject playerDisplayObject;
    private PlayerInformation playerInformation;
    private Player player;

    // UI
    [SerializeField] private List<GameObject> statusPrefabs = new List<GameObject>();


    // Encounter Information
    private EncounterReward encounterReward;
    private int encounterRewardValue;
    private EncounterType previousEncounter;

    // Scene
    private int previousScene;
    private HashSet<int> battleScenes = new HashSet<int>() { 1, 4, 6, 7 };


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
            if (currentSaveSlot == -1 || !loadGame(currentSaveSlot))
            {
                    playerInformation = new PlayerInformation(50, 50, 3, 0, 6, null, null, PlayerClass.Mistborn, null);
                    currentLevel = 1;
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
            topBarUIManager.Initialize(playerInformation.playerGold, currentLevel, playerInformation.playerCardRarity, playerInformation.playerCardChoices);
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

    public List<GameObject> getStatusPrefabs()
    {
        return statusPrefabs;
    }
    
    public List<DeckModelSO> getVictoryCardsPools()
    {
        return playerInformation.victoryCardPools;
    }

    public IEnumerator encounterVictory()
    {
        Debug.Log("Victory! rewards: " + encounterReward);
        AudioManager.instance.playVictory();
        addEncounterRewards();
        yield return new WaitForSeconds(1.0f);
        victoryManager.showVictoryScreen(playerInformation.playerCardChoices, playerInformation.playerCardRarity, playerInformation.playerMythic != null);
    }

    void addEncounterRewards()
    {
        switch (encounterReward)
        {
            case EncounterReward.Gold:
                addPlayerGold(encounterRewardValue);
                break;
            case EncounterReward.CardRarity:
                playerInformation.playerCardChoices += encounterRewardValue;
                break;
            case EncounterReward.CardChoices:
                playerInformation.playerCardChoices += encounterRewardValue;
                break;
            case EncounterReward.MaxHealth:
                playerInformation.playerMaxHealth += encounterRewardValue;
                break;
        }
    }

    public void addCardToPlayerDeck(CardModelSO model)
    {
        CardModelSO clone = model.clone();
        playerInformation.playerDeck.cards.Add(clone);
    }

    public void setPlayerMythicCard(CardModelSO card)
    {
        playerInformation.playerMythic = new MythicCard(card);
        player.setMythic(card);
    }

    public void removeCardFromPlayerDeck(CardModelSO model)
    {
        playerInformation.playerDeck.cards.Remove(model);
    }

    public IEnumerator moveToPathSelection()
    {
         // update player stats
        playerInformation.playerCurrentHealth = player.getCurrentHealth();
        playerInformation.playerMaxHealth = player.getMaxHealth();

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
        return playerInformation.playerGold;
    }

    // TO-DO: add cha-ching sound
    public void addPlayerGold(int value)
    {
        playerInformation.playerGold += value;
        topBarUIManager.updateGoldCount(playerInformation.playerGold);
    }

    // TO-DO: add gold spending sound
    public void subtractPlayerGold(int value)
    {
        playerInformation.playerGold -= value;
        topBarUIManager.updateGoldCount(playerInformation.playerGold);
    }

    public int getPlayerHandSize()
    {
        return playerInformation.playerHandSize;
    }

    public int getPlayerHandEnergy()
    {
        return playerInformation.playerHandEnergy;
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

    public void endTurn()
    {
        TurnManager.instance.endTurnButtonPressed();
    }

    public DeckModelSO getPlayerDeck()
    {
        return playerInformation.playerDeck;
    }

    public int getPlayerMaxHealth()
    {
        return playerInformation.playerMaxHealth;
    }

    public int getPlayerCurrentHealth()
    {
        return playerInformation.playerCurrentHealth;
    }

    public void loadEncounterTypeAndRewards(EncounterType type, EncounterReward reward, int rewardValue)
    {
        encounterReward = reward;
        encounterRewardValue = rewardValue;
        switch (type)
        {
            case EncounterType.Forge:
                SceneLoader.instance.loadScene(Enums.mapBuildIndex(SceneBuildIndex.FORGE));
                break;
            case EncounterType.Regular_Encounter:
                SceneLoader.instance.loadScene(Enums.mapBuildIndex(SceneBuildIndex.BASE_LEVEL));
                break;
            case EncounterType.Mini_Boss_Encounter:
                // TODO: Change this
                SceneLoader.instance.loadScene(Enums.mapBuildIndex(SceneBuildIndex.MINI_BOSS));
                break;
            case EncounterType.Final_Boss:
                // TODO: Change this
                SceneLoader.instance.loadScene(Enums.mapBuildIndex(SceneBuildIndex.MINI_BOSS));
                break;
            case EncounterType.Culver_Encounter:
                SceneLoader.instance.loadScene(Enums.mapBuildIndex(SceneBuildIndex.CULVER));
                break;
            case EncounterType.Hold_The_Line_Encounter:
                SceneLoader.instance.loadScene(Enums.mapBuildIndex(SceneBuildIndex.HOLD_THE_LINE));
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
        saveData.playerCurrentHealth = playerInformation.playerCurrentHealth;
        saveData.playerMaxHealth = playerInformation.playerMaxHealth;
        saveData.playerGold = playerInformation.playerGold;
        saveData.playerHandSize = playerInformation.playerHandSize;
        saveData.playerHandEnergy = playerInformation.playerHandEnergy;
        saveData.playerCards = new List<SerializableCardModel>();
        saveData.playerCardChoices = playerInformation.playerCardChoices;
        saveData.playerCardRarity = playerInformation.playerCardRarity;
        saveData.mythicCard = playerInformation.playerMythic;
        saveData.victoryCards = playerInformation.victoryCardPools;
        saveData.playerPrefab = playerObject;
        saveData.playerDisplay = playerDisplayObject;
        saveData.playerClass = playerInformation.playerClass;
        saveData.pathOptions = currentPathSelectionOptions;
        saveData.musicVolume = currentVolume.Item1;
        saveData.sfxVolume = currentVolume.Item2;


        foreach (CardModelSO model in playerInformation.playerDeck.cards)
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
        playerInformation = new PlayerInformation(data.playerMaxHealth, data.playerCurrentHealth, data.playerHandEnergy, data.playerGold, 
            data.playerHandSize, null, data.mythicCard, data.playerClass, data.victoryCards);
        playerInformation.playerCardChoices = data.playerCardChoices;
        playerInformation.playerCardRarity = data.playerCardRarity;
        playerInformation.playerDeck = ScriptableObject.CreateInstance<DeckModelSO>();
        playerInformation.playerDeck.cards = new List<CardModelSO>();
        currentPathSelectionOptions = data.pathOptions;
        playerObject = data.playerPrefab;
        playerDisplayObject = data.playerDisplay;



        foreach (SerializableCardModel serializableCardModel in data.playerCards)
        {
            playerInformation.playerDeck.cards.Add(SaveSystem.convertToRuntimeCard(serializableCardModel));
        }

        AudioManager.instance.setBackgroundMusicVolume(data.musicVolume);
        AudioManager.instance.setSFXVolume(data.sfxVolume);

        return true;
    }

    public void loadPlayerInformation(Player currentPlayer)
    {
        player = currentPlayer;

        currentPlayer.setDeck(playerInformation.playerDeck);
        currentPlayer.setMaxHealth(playerInformation.playerMaxHealth);
        currentPlayer.setCurrentHealth(playerInformation.playerCurrentHealth);
        currentPlayer.setEnergy(playerInformation.playerHandEnergy);
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
