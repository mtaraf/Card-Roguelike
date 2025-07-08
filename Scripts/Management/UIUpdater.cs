using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIUpdater : MonoBehaviour
{
    [Header("Health UI")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Effect UI")]
    [SerializeField] private Transform effectUIParent;
    [SerializeField] private List<GameObject> effectPrefabs;


    private Dictionary<EffectType, GameObject> activeEffects = new();
    private List<GameObject> effectUISlots = new();

    void Awake()
    {
        foreach (Transform child in effectUIParent)
        {
            effectUISlots.Add(child.gameObject);
        }

        effectPrefabs = GameManager.instance.getStatusObjects();
    }

    public void setHealth(float current, float max)
    {
        healthText.text = $"{current}/{max}";
        healthSlider.value = current / max;
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
                reorderSlots();
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
        GameObject prefab = effectPrefabs.Find(obj => obj.name == type.ToDisplayString());
        if (prefab == null)
        {
            Debug.LogWarning($"Effect prefab not found for {type}");
            return;
        }

        GameObject slot = effectUISlots.Find(s => s.transform.childCount == 0);
        if (slot == null)
        {
            Debug.LogWarning("No empty effect UI slots available");
            return;
        }

        GameObject newEffect = Instantiate(prefab, slot.transform);
        newEffect.transform.Find("ValueContainer").GetChild(0).GetComponent<TextMeshProUGUI>().text = value.ToString();
        activeEffects[type] = newEffect;
        reorderSlots();
    }

    private void reorderSlots()
    {
        List<Transform> active = new List<Transform>();

        foreach (GameObject slot in effectUISlots)
        {
            if (slot.transform.childCount > 0)
            {
                Transform effect = slot.transform.GetChild(0);
                active.Add(effect);
                effect.SetParent(null); // Detach to preserve it
            }
        }

        foreach (GameObject slot in effectUISlots)
        {
            foreach (Transform child in slot.transform)
            {
                Destroy(child.gameObject);
            }
        }

        for (int i = 0; i < active.Count && i < effectUISlots.Count; i++)
        {
            Transform effect = active[i];
            effect.SetParent(effectUISlots[i].transform, false);
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
}