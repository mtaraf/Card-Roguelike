using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthAndStatus : MonoBehaviour
{
    [SerializeField] private List<GameObject> statuses = new List<GameObject>();
    private List<GameObject> currentStatuses = new List<GameObject>();

    private Transform healthStatusUI;
    private Slider healthSlider;
    private TextMeshProUGUI healthText;

    void Start()
    {
        healthStatusUI = getHeathAndStatus();

        if (healthStatusUI == null)
        {
            Debug.LogError("Health and Status UI not found for " + name);
        }
        else
        {
            healthSlider = healthStatusUI.GetComponentInChildren<Slider>();
            healthText = healthStatusUI.Find("HealthTextValue").GetComponent<TextMeshProUGUI>();
        }
    }

    Transform getHeathAndStatus()
    {
        foreach (Transform child in this.transform)
        {
            if (child.CompareTag("HealthUI"))
            {
                return child;
            }
        }

        return null;
    }

    public void addStatus(string status, int value)
    {

    }

    public void removeStatus(string status)
    {

    }

    public string[] getCurrentStatuses()
    {
        string[] statuses = new string[currentStatuses.Count];

        for (int i = 0; i < currentStatuses.Count; i++)
        {
            statuses[i] = currentStatuses[i].name;
        }

        return statuses;
    }

    public void setHealth(int current, int max)
    {
        healthText.text = current.ToString() + '/' + max.ToString();
        healthSlider.value = current / max;
    }
}
