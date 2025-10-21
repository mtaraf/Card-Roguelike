using TMPro;
using UnityEngine;

public class CharacterContainer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private TextMeshProUGUI characterClassName;
    [SerializeField] private TextMeshProUGUI characterDetails;
    [SerializeField] private GameObject characterDisplayContainer;

    public void setCharacterInformation(string name, CharacterClass characterClass, string [] details, GameObject displayCharacter)
    {
        characterName.text = name;
        string setDetails = "Specialties: \n";
        foreach (string detail in details)
        {
            setDetails += "- " + detail + "\n";
        }
        characterDetails.text = setDetails;
        characterClassName.text = characterClass.ToString();
        GameObject characterDisplay = Instantiate(displayCharacter, characterDisplayContainer.transform);
        characterDisplay.transform.localPosition = Vector3.zero;
    }
}
