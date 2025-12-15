using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionSceneController : MonoBehaviour
{
    private GameObject characterSelector;
    private GameObject characterDisplay;
    private GameObject characterDetails;
    private GameObject characterName;
    private GameObject characterIconPrefab;
    private GameObject[] availableCharacters;
    private Transform canvasTransform;
    private GameObject cardPrefab;
    private Button startGameButton;

    void Start()
    {
        cardPrefab = Resources.Load<GameObject>("UI/Cards/CardNoAnimation");
        characterIconPrefab = Resources.Load<GameObject>("UI/CharacterSelection/CharacterIcon");
        availableCharacters = Resources.LoadAll<GameObject>("CharacterPrefabs/CharacterDisplays");

        characterDetails = GameObject.Find("CharacterDetails");
        characterSelector = GameObject.Find("CharacterSelector");
        characterDisplay = GameObject.Find("CharacterDisplay");
        characterName = GameObject.Find("CharacterName");

        startGameButton = GameObject.Find("StartGameButton").GetComponent<Button>();
        startGameButton.onClick.AddListener(() => startGame());

        if (characterDetails == null || characterSelector == null || characterDisplay == null || characterName == null)
        {
            Debug.LogError("Could not find object before initializing CharacterSelectionScene");
        }

        instatiateScene();
    }

    void instatiateScene()
    {
        Vector2 firstCharacterContianerPos = new Vector2(550, -800);
        for (int i=0; i<availableCharacters.Length;  i++)
        {
            GameObject character = availableCharacters[i];
            DisplayInformation displayInformation = character.GetComponent<DisplayInformation>();
            GameObject icon = Instantiate(characterIconPrefab, characterSelector.transform);

            // Set sprite
            icon.transform.GetChild(0).GetComponent<Image>().sprite = displayInformation.characterIcon;

            // Set OnClick
            icon.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => setSelectedCharacter(character.name));
        }

        // Set default character to first one
        setSelectedCharacter(availableCharacters[0].name);
    }

    void setSelectedCharacter(string name)
    {
        GameObject character = Array.Find(availableCharacters, character => character.name == name);
        DisplayInformation displayInformation = character.GetComponent<DisplayInformation>();
        Debug.Log("Selected: " + character.name);
        if (characterDisplay.transform.childCount > 0)
        {
            // Destroy the current character display
            Destroy(characterDisplay.transform.GetChild(0).gameObject);
        }

        characterName.GetComponent<TextMeshProUGUI>().text = displayInformation.characterName;

        Instantiate(character, characterDisplay.transform);

        setDisplayDetails(displayInformation);
    }

    void setDisplayDetails(DisplayInformation displayInformation)
    {
        string specialties = "";
        for (int i = 0; i < displayInformation.classDetails.Length; i++)
        {
            specialties += displayInformation.classDetails[i] + "\n";
        }

        characterDetails.transform.Find("CharacterClass").GetComponent<TextMeshProUGUI>().text = displayInformation.characterClass.ToString();
        characterDetails.transform.Find("CharacterSpecialties").GetComponent<TextMeshProUGUI>().text = specialties;
        Transform cardSlot1 = characterDetails.transform.Find("CardSlot1");
        Transform cardSlot2 = characterDetails.transform.Find("CardSlot2");

        // Destroy present cards
        if (cardSlot1.childCount > 0)
            Destroy(cardSlot1.GetChild(0).gameObject);
        if (cardSlot2.childCount > 0)
            Destroy(cardSlot2.GetChild(0).gameObject);

        // Instatiate new cards
        GameObject card1 = Instantiate(cardPrefab, cardSlot1);
        card1.GetComponent<Card>().setCardDisplayInformation(displayInformation.displaycards[0]);

        GameObject card2 = Instantiate(cardPrefab, cardSlot2);
        card2.GetComponent<Card>().setCardDisplayInformation(displayInformation.displaycards[1]);
    }
    
    public void startGame()
    {
        GameObject selectedCharacter = characterDisplay.transform.GetChild(0).gameObject;
        DisplayInformation information = selectedCharacter.GetComponent<DisplayInformation>();
        GameObject playerCharacter;
        GameObject playerDisplay;

        switch (information.characterClass)
        {
            case PlayerClass.Paladin:
                playerCharacter = Resources.Load<GameObject>("CharacterPrefabs/PlayableCharacters/Dwarf/DwarfCharacter");
                playerDisplay = Resources.Load<GameObject>("CharacterPrefabs/CharacterDisplays/DwarfDisplay");
                break;
            case PlayerClass.Mistborn:
                playerCharacter = Resources.Load<GameObject>("CharacterPrefabs/PlayableCharacters/Witch/WitchCharacter");
                playerDisplay = Resources.Load<GameObject>("CharacterPrefabs/CharacterDisplays/WitchDisplay");
                break;
            default:
                playerCharacter = Resources.Load<GameObject>("CharacterPrefabs/PlayableCharacters/Mistborn/Mistborn");
                playerDisplay = Resources.Load<GameObject>("CharacterPrefabs/CharacterDisplays/MistbornDisplay");
                break;
        }

        GameManager.instance.setPlayerCharacter(playerCharacter);
        GameManager.instance.setPlayerDisplayObject(playerDisplay);

        GameManager.instance.loadScene((int)SceneBuildIndex.PATH_SELECTION);
    }
}
