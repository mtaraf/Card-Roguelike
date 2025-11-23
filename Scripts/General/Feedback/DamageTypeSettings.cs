using UnityEngine;

[CreateAssetMenu(fileName = "DamageTypeSettings", menuName = "Settings/DamageTypeSettings")]
public class DamageTypeSettings: ScriptableObject
{
    [System.Serializable]
    public class DamageTypeInfo
    {
        public DamageType type;

        [Header("Text Style")]
        public Color color = Color.white;
        public int baseFontSize = 35;
        public FontStyle fontStyle = FontStyle.Bold;

        [Header("Gradient (Optional)")]
        public bool useGradient = false;
        public Color gradientTop = Color.white;
        public Color gradientBottom = Color.white;

        [Header("Animation Settings")]
        public float bounceScale = 1.2f; 
        public float shakeIntensity = 5f;
    }

    public DamageTypeInfo [] damageTypes;
    
    public DamageTypeInfo GetDamageTypeInfo(DamageType type)
    {
        foreach (DamageTypeInfo entry in damageTypes)
        {
            if (entry.type == type)
                return entry;
        }

        Debug.Log("Could not find damage type entry for " + type);
        return null;
    }
}