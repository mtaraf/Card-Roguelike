using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class FloatingFeedbackUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textComponent;
    public float lifetime = 1.2f;
    public float riseSpeed = 30f;
    private Vector3 startScale;

    private void Awake()
    {
        textComponent = gameObject.GetComponent<TextMeshProUGUI>();
        startScale = transform.localScale;
    }

    public void setText(string text, Color color, int baseFontSize, bool useGradient, Color gradientTop, Color gradientBottom, float bounce, float shake)
    {
        textComponent.text = text;

        // Scale based on damage
        int value = 0;
        int.TryParse(text, out value);
        float scaleBoost = Mathf.Clamp(value / 50f, 0, 2f);

        textComponent.fontSize = baseFontSize + scaleBoost * 10f;

        if (useGradient)
        {
            textComponent.colorGradient = new VertexGradient(gradientTop, gradientTop, gradientBottom, gradientBottom);
        }
        else
        {
            textComponent.color = color;
        }

        StartCoroutine(animateFeedback(bounce, shake));
    }

    private IEnumerator animateFeedback(float bounce, float shake)
    {
        float timer = 0f;

        // Bounce
        transform.localScale = startScale * bounce;
        yield return new WaitForSeconds(0.05f);
        transform.localScale = startScale;

        while (timer < lifetime)
        {
            transform.localPosition += Vector3.up * riseSpeed * Time.deltaTime;

            // // Shake
            if (shake > 0)
            {
                transform.localPosition += new Vector3(UnityEngine.Random.Range(-shake, shake) * Time.deltaTime, 0, 0);
            }

            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
