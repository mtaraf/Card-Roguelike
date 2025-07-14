using System.Collections;
using TMPro;
using UnityEngine;

public class FloatingFeedbackUI : MonoBehaviour
{
    [SerializeField] private float floatSpeed = 1.0f;
    [SerializeField] private float fadeSpeed = 1.0f;
    [SerializeField] private Vector3 floatDirection = new Vector3(0, 1, 0);

    private TextMeshProUGUI textComponent;
    private Color color;
    private float lifetime;


    void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        if (textComponent == null)
        {
            Debug.LogError("FeedbackUI requires a TextMeshProUGUI component.");
            return;
        }

        color = textComponent.color;
    }

    public IEnumerator moveAndDestroy()
    {
        lifetime = fadeSpeed;
        Debug.Log(lifetime);
        RectTransform rect = GetComponent<RectTransform>();
        while (lifetime > 0f)
        {
            // Move upward
            rect.position += floatDirection * floatSpeed * Time.deltaTime;

            // Fade out
            lifetime -= Time.deltaTime;
            float alpha = Mathf.Clamp01(lifetime / fadeSpeed);
            textComponent.color = new Color(color.r, color.g, color.b, alpha);

            yield return null;
        }

        Destroy(gameObject);
    }
    
    public void SetText(string text, Color color)
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.color = color;
        this.color = color;
    }
}
