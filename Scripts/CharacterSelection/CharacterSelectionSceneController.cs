using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectionSceneController : MonoBehaviour
{
    private GameObject characterContainerPrefab;
    private GameObject[] availableCharacters;
    private Transform canvasTransform;

    void Start()
    {
        characterContainerPrefab = Resources.Load<GameObject>("UI/CharacterSelection/CharacterContainer");
        availableCharacters = Resources.LoadAll<GameObject>("CharacterPrefabs/PlayerDisplays");
        canvasTransform = GameObject.FindGameObjectWithTag("CharacterDisplayContent").transform;

        instatiateContainers();
    }

    void instatiateContainers()
    {
        Vector2 firstCharacterContianerPos = new Vector2(550, -800);
        foreach(GameObject character in availableCharacters)
        {
            DisplayInformation displayInformation = character.GetComponent<DisplayInformation>();
            GameObject container = Instantiate(characterContainerPrefab, canvasTransform);
            container.transform.localPosition = firstCharacterContianerPos;
            CharacterContainer characterContainer = container.GetComponent<CharacterContainer>();
            characterContainer.setCharacterInformation(displayInformation.characterName, displayInformation.characterClass, displayInformation.classDetails, character);
        }
    }
}
