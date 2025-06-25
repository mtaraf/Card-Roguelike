using System;
using System.Collections.Generic;
using System.Linq;
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
    private Player player;
    private Enemy enemy;
    private bool isEnemy = false;
    private Dictionary<Attributes, int> startingAttributes;
    private List<GameObject> effectUISlots = new List<GameObject>();

    void Start()
    {
        healthStatusUI = getHeathAndStatus();
        GameObject effectUI = Helpers.findDescendant(transform, "EffectUI");
        if (healthStatusUI == null || effectUI == null)
        {
            Debug.LogError("healthStatusUI or effectUI not found for " + name);
        }
        else
        {
            healthSlider = healthStatusUI.GetComponentInChildren<Slider>();
            healthText = healthStatusUI.Find("HealthTextValue").GetComponent<TextMeshProUGUI>();

            for (int i = 0; i < effectUI.transform.childCount; i++)
            {
                effectUISlots.Add(effectUI.transform.GetChild(i).gameObject);
            }
        }
        player = GetComponent<Player>();

        if (player == null)
        {
            enemy = GetComponent<Enemy>();
            isEnemy = true;
            startingAttributes = enemy.getAttributes();
        }
        else
        {
            startingAttributes = player.getAttributes();
        }

        if (player == null && enemy == null)
        {
            Debug.LogError("Could not find Player or Enemy component for HealthAndStatus component");
        }

        updateAttributes(true, startingAttributes);
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

    public void updateAttributes(bool onLoad, Dictionary<Attributes, int> att)
    {
        // update attributes on UI
        foreach (KeyValuePair<Attributes, int> pair in att)
        {
            if (pair.Value != 0)
            {
                updateStatus(pair.Key, pair.Value);
            }
        }
    }

    // adds or updates status to ui
    public void updateStatus(Attributes status, int value)
    {
        foreach (GameObject slot in effectUISlots)
        {
            if (slot.transform.childCount > 0)
            {
                // TO-DO: Find a better way to see what status is in the effect slot
                if (slot.transform.GetChild(0).name.Substring(0,slot.transform.GetChild(0).name.Length-7) == status.ToDisplayString())
                {
                    // remove effect if value is below 1
                    if (value < 1)
                    {
                        Destroy(slot.transform.GetChild(0));
                        return;
                    }

                    slot.transform.GetChild(0).transform.Find("ValueContainer").transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = value.ToString();
                    return;
                }
            }
            else
            {
                // Get Effect UI to instantiate
                GameObject statusEffectUI = statuses.Find((obj) => obj.name == status.ToDisplayString());

                // Assign value to effect UI
                statusEffectUI.transform.Find("ValueContainer").transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = value.ToString();

                if (statusEffectUI == null)
                {
                    Debug.LogError("Could not find status ui object in addStatus");
                    return;
                }

                Instantiate(statusEffectUI, slot.transform);
                return;
            }
        }
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

    public void setHealth(float current, float max)
    {
        healthText.text = current.ToString() + '/' + max.ToString();
        healthSlider.value = current / max;
    }
}
