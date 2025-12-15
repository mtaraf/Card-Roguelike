using UnityEngine;

public class DisplayInformation : MonoBehaviour
{
    [SerializeField] public PlayerClass characterClass;
    [SerializeField] public string [] classDetails;
    [SerializeField] public string characterName;
    [SerializeField] public Sprite characterIcon;
    [SerializeField] public CardModelSO[] displaycards = new CardModelSO[2];

    [SerializeField] public int pathSelectionYOffset;
    [SerializeField] public float pathSelectionScale;

    public void pathSelectionAlignment()
    {
        Vector3 pos = transform.localPosition;
        pos.y += pathSelectionYOffset;
        transform.localPosition = pos;

        transform.localScale = Vector2.one * pathSelectionScale;
    }
}
