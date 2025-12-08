
using UnityEngine;


public enum DamageType
{
    Critical,
    Bleed,
    Corruption,
    Heal,
    General
}


public class FeedbackUI: MonoBehaviour
{
    private DamageTypeSettings damageSettings;
    [SerializeField] private RectTransform characterRect;
    private GameObject floatingFeedbackUI;
    private GameObject mainCanvas;

    public void Start()
    {
        floatingFeedbackUI = Resources.Load<GameObject>("UI/General/Feedback/FloatingFeedbackUIPrefab");
        damageSettings = Resources.Load<DamageTypeSettings>("UI/General/Feedback/DamageTypeSettings");

        characterRect = gameObject.GetComponent<RectTransform>();
        mainCanvas = GameObject.FindGameObjectWithTag("BaseLevelCanvas");

        if (characterRect == null || damageSettings == null || floatingFeedbackUI == null || mainCanvas == null)
        {
            Debug.LogError("Error initializing feedback UI for " + gameObject.name);
        }
    }

    public void showFloatingFeedback(DamageType type, string message)
    {
        DamageTypeSettings.DamageTypeInfo info = damageSettings.GetDamageTypeInfo(type);

        if (info == null)
            return;
        
        GameObject feedback = Instantiate(floatingFeedbackUI, gameObject.transform);

        feedback.transform.localPosition = 
            new Vector3(0, (characterRect.sizeDelta.y / 2) + 20, 0);

        FloatingFeedbackUI floatingFeedback = feedback.GetComponent<FloatingFeedbackUI>();

        floatingFeedback.setText(message, info.color, info.baseFontSize, info.useGradient, info.gradientTop, info.gradientBottom, info.bounceScale, info.shakeIntensity,
            info.rotationSpeed);
    }
}