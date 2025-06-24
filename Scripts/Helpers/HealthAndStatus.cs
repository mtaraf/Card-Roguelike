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
    private Dictionary<Attributes, int> currentAttributes;
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
            currentAttributes = enemy.getAttributes();
        }
        else
        {
            currentAttributes = player.getAttributes();
        }

        if (player == null && enemy == null)
        {
            Debug.LogError("Could not find Player or Enemy component for HealthAndStatus component");
        }

        updateAttributes(true, currentAttributes);
    }

    public void updateAttributes(bool onLoad, Dictionary<Attributes, int> att)
    {
        // TO-DO: Is this needed?
        if (attributesAreEqual(currentAttributes, att) && !onLoad)
        {
            return;
        }

        // update attributes on UI
        foreach (KeyValuePair<Attributes, int> pair in att)
        {
            if (pair.Value != 0)
            {
                addStatus(pair.Key, pair.Value);
            }
        }

        // update current attributes
        currentAttributes = att;
    }



    bool attributesAreEqual(Dictionary<Attributes, int> currentAtt, Dictionary<Attributes, int> updatedAtt)
    {
        if (currentAtt.Count != updatedAtt.Count)
        {
            return false;
        }

        foreach (var pair in currentAtt)
        {
            if (!updatedAtt.TryGetValue(pair.Key, out int valueB) || pair.Value != valueB)
            {
                return false;
            }
        }

        return true;
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

    // adds or updates status to ui
    public void addStatus(Attributes status, int value)
    {
        foreach (GameObject slot in effectUISlots)
        {
            if (slot.transform.childCount > 0)
            {
                if (slot.transform.GetChild(0).name == status.ToDisplayString())
                {
                    // Check value of effect to change or remove effect if value is < 1
                    int currentEffectValue = Int32.Parse(slot.transform.GetChild(0).transform.Find("ValueContainer").transform.GetChild(0).GetComponent<TextMeshProUGUI>().text);
                    currentEffectValue += value;

                    // remove effect if value is below 1
                    if (currentEffectValue < 1)
                    {
                        Destroy(slot.transform.GetChild(0));
                        return;
                    }

                    slot.transform.GetChild(0).transform.Find("ValueContainer").transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = currentEffectValue.ToString();
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
