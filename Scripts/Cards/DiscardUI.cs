using System.Collections.Generic;
using UnityEngine;

public class DiscardUI : MonoBehaviour
{
    [SerializeField] private List<GameObject> discardSlots = new List<GameObject>();

    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (child.name != "ConfirmationButton")
            {
                discardSlots.Add(child);
            }
        }
    }

    void Update()
    {

    }

    public void addCardToSlot(GameObject card)
    {
        for (int i = 0; i < discardSlots.Count; i++)
        {
            if (discardSlots[i].transform.childCount == 0)
            {
                card.transform.SetParent(discardSlots[i].transform);
                card.transform.localPosition = Vector3.zero;
            }
        }
    }
}
