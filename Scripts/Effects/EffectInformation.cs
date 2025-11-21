using UnityEngine;

public enum EffectExecutionType
{
    Turn_Based,
    Value_Based,
}
public class EffectInformation : MonoBehaviour
{
    [SerializeField] public EffectExecutionType effectExecutionType;
    [SerializeField] public EffectType effectType;
}
