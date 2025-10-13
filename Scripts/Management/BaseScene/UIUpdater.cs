using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIUpdater : MonoBehaviour
{
    [Header("Health UI")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Turn Effect UI")]
    [SerializeField] private Transform turnEffectUIParent;
    [SerializeField] private List<GameObject> turnEffectPrefabs;

    [Header("Value Effect UI")]
    [SerializeField] private Transform valueEffectUIParent;
    [SerializeField] private List<GameObject> valueEffectPrefabs;


    [SerializeField] private Dictionary<EffectType, GameObject> activeEffects = new();
    private List<GameObject> turnEffectUISlots = new();
    private List<GameObject> valueEffectUISlots = new();
    private HashSet<EffectType> turnEffects = new HashSet<EffectType>();
    private bool invincible = false;

    void Awake()
    {
        turnEffects.Add(EffectType.Weaken);
        turnEffects.Add(EffectType.Divinity);

        foreach (Transform child in turnEffectUIParent)
        {
            turnEffectUISlots.Add(child.gameObject);
        }

        turnEffectPrefabs = GameManager.instance.getTurnBasedStatusObjects();

        foreach (Transform child in valueEffectUIParent)
        {
            valueEffectUISlots.Add(child.gameObject);
        }

        valueEffectPrefabs = GameManager.instance.getValueBasedStatusObject();
    }

    public void setHealth(float current, float max)
    {
        if (invincible)
        {
            healthText.text = $"\u221E/\u221E";
            healthSlider.value = 1;
        }
        else if (current < 1)
        {
            healthText.text = $"0/{max}";
            healthSlider.value = 0;
        }
        else
        {
            healthText.text = $"{current}/{max}";
            healthSlider.value = current / max;
        }
    }

    public void updateEffect(EffectType type, int value)
    {
        // Remove effect if value < 1
        if (value < 1)
        {
            if (activeEffects.ContainsKey(type))
            {
                Destroy(activeEffects[type]);
                activeEffects.Remove(type);
                StartCoroutine(reorderSlots(type));
            }
            return;
        }

        // Update value if effect exists
        if (activeEffects.ContainsKey(type))
        {
            activeEffects[type].transform.Find("ValueContainer").GetChild(0).GetComponent<TextMeshProUGUI>().text = value.ToString();
            return;
        }

        // Add new effect
        GameObject prefab;
        GameObject slot;

        // Check if turn or status based
        if (turnEffects.Contains(type))
        {
            // Turn Based
            prefab = turnEffectPrefabs.Find(obj => obj.name == type.ToDisplayString());
            if (prefab == null)
            {
                Debug.LogWarning($"Effect prefab not found for {type}");
                return;
            }

            slot = turnEffectUISlots.Find(s => s.transform.childCount == 0);
            if (slot == null)
            {
                Debug.LogWarning("No empty effect UI slots available");
                return;
            }
        }
        else
        {
            // Value Based
            prefab = valueEffectPrefabs.Find(obj => obj.name == type.ToDisplayString());
            if (prefab == null)
            {
                Debug.LogWarning($"Effect prefab not found for {type}");
                return;
            }

            slot = valueEffectUISlots.Find(s => s.transform.childCount == 0);
            if (slot == null)
            {
                Debug.LogWarning("No empty effect UI slots available");
                return;
            }
        }

        GameObject newEffect = Instantiate(prefab, slot.transform);
        newEffect.transform.Find("ValueContainer").GetChild(0).GetComponent<TextMeshProUGUI>().text = value.ToString();
        activeEffects[type] = newEffect;

        StartCoroutine(reorderSlots(type));
    }

    private IEnumerator reorderSlots(EffectType type)
    {
        yield return new WaitForEndOfFrame();

        if (turnEffects.Contains(type))
        {
            reorderTurnSlots();
        }
        else
        {
            reorderValueSlots();
        }
        // List<Transform> active = new List<Transform>();

        // foreach (GameObject slot in turnEffectUISlots)
        // {
        //     if (slot.transform.childCount > 0)
        //     {
        //         Transform effect = slot.transform.GetChild(0);
        //         active.Add(effect);
        //         effect.SetParent(null); // Detach to preserve it
        //     }
        // }

        // foreach (GameObject slot in turnEffectUISlots)
        // {
        //     foreach (Transform child in slot.transform)
        //     {
        //         Destroy(child.gameObject);
        //     }
        // }

        // for (int i = 0; i < active.Count && i < turnEffectUISlots.Count; i++)
        // {
        //     Transform effect = active[i];
        //     effect.SetParent(turnEffectUISlots[i].transform, false);
        //     effect.localPosition = Vector3.zero;
        //     effect.localRotation = Quaternion.identity;
        //     effect.localScale = Vector3.one;
        // }
    }

    void reorderValueSlots()
    {
        List<Transform> active = new List<Transform>();

        foreach (GameObject slot in valueEffectUISlots)
        {
            if (slot.transform.childCount > 0)
            {
                Transform effect = slot.transform.GetChild(0);
                active.Add(effect);
                effect.SetParent(null); // Detach to preserve it
            }
        }

        foreach (GameObject slot in valueEffectUISlots)
        {
            foreach (Transform child in slot.transform)
            {
                Destroy(child.gameObject);
            }
        }

        for (int i = 0; i < active.Count && i < valueEffectUISlots.Count; i++)
        {
            Transform effect = active[i];
            effect.SetParent(valueEffectUISlots[i].transform, false);
            effect.localPosition = Vector3.zero;
            effect.localRotation = Quaternion.identity;
            effect.localScale = Vector3.one;
        }
    }

    void reorderTurnSlots()
    {
        List<Transform> active = new List<Transform>();

        foreach (GameObject slot in turnEffectUISlots)
        {
            if (slot.transform.childCount > 0)
            {
                Transform effect = slot.transform.GetChild(0);
                active.Add(effect);
                effect.SetParent(null); // Detach to preserve it
            }
        }

        foreach (GameObject slot in turnEffectUISlots)
        {
            foreach (Transform child in slot.transform)
            {
                Destroy(child.gameObject);
            }
        }

        for (int i = 0; i < active.Count && i < turnEffectUISlots.Count; i++)
        {
            Transform effect = active[i];
            effect.SetParent(turnEffectUISlots[i].transform, false);
            effect.localPosition = Vector3.zero;
            effect.localRotation = Quaternion.identity;
            effect.localScale = Vector3.one;
        }
    }

    public void clearAllEffects()
    {
        foreach (GameObject effect in activeEffects.Values)
        {
            Destroy(effect);
        }
        activeEffects.Clear();
    }
    
    public void setInvincible()
    {
        invincible = true;
    }
}