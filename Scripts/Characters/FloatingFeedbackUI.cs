using System.Collections;
using TMPro;
using UnityEngine;

public class FloatingFeedbackUI : MonoBehaviour
{
    [SerializeField] private float floatSpeed = 1.0f;
    [SerializeField] private float fadeSpeed = 1.0f;
    [SerializeField] private Vector3 floatDirection;

    private TextMeshProUGUI textComponent;
    private Color color;
    private float lifetime;


    void Start()
    {
        floatDirection = new Vector3(Random.Range(-1, 1.01f), 1, 0);
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
        int num;
        bool isNumeric = int.TryParse(text, out num);
        if (isNumeric)
        {
            textComponent.fontSize = 70;
            textComponent.fontStyle = FontStyles.Bold;
        }
        else
        {
            textComponent.fontSize = 50;
        }
        this.color = color;
    }
}
