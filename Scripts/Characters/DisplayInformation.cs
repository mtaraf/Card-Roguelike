using UnityEngine;

public enum CharacterClass
{
    Paladin,
    Flower
}
public class DisplayInformation : MonoBehaviour
{
    [SerializeField] public CharacterClass characterClass;
    [SerializeField] public string[] classDetails;
    [SerializeField] public string characterName;
    [SerializeField] public Sprite characterIcon;
}
