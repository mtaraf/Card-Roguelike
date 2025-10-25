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

    void Start()
    {
        characterIconPrefab = Resources.Load<GameObject>("UI/CharacterSelection/CharacterIcon");
        availableCharacters = Resources.LoadAll<GameObject>("CharacterPrefabs/CharacterDisplays");

        characterDetails = GameObject.Find("CharacterDetails");
        characterSelector = GameObject.Find("CharacterSelector");
        characterDisplay = GameObject.Find("CharacterDisplay");
        characterName = GameObject.Find("CharacterName");

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
            icon.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => setSelectedCharacter(i));
        }

        // Set default character to first one
        setSelectedCharacter(0);
    }
    
    void setSelectedCharacter(int index)
    {
        GameObject character = availableCharacters[index];

        if (characterDisplay.transform.childCount > 0)
        {
            // Destroy the current character display
            Destroy(characterDisplay.transform.GetChild(0).gameObject);
        }

        characterName.GetComponent<TextMeshProUGUI>().text = character.GetComponent<DisplayInformation>().characterName;

        Instantiate(character, characterDisplay.transform);
    }
}
