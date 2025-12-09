using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class FloatingFeedbackUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textComponent;
    public float lifetime = 0.7f;
    public float riseSpeed = 40f;
    private Vector3 startScale;

    private void Awake()
    {
        textComponent = gameObject.GetComponent<TextMeshProUGUI>();
        startScale = transform.localScale;
    }

    public void setText(string text, Color color, int baseFontSize, bool useGradient, Color gradientTop, Color gradientBottom, float bounce, float shake, int rotationSpeed)
    {
        textComponent.text = text;

        // Scale based on damage
        int value = 0;
        int.TryParse(text, out value);
        float scaleBoost = Mathf.Clamp(value / 50f, 0, 2f);

        textComponent.fontSize = baseFontSize + scaleBoost * 10f;

        if (useGradient)
        {
            textComponent.enableVertexGradient = true;
            textComponent.colorGradient = new VertexGradient(gradientTop, gradientTop, gradientBottom, gradientBottom);
        }
        else
        {
            textComponent.color = color;
        }

        StartCoroutine(animateFeedback(bounce, shake, rotationSpeed));
    }

    private IEnumerator animateFeedback(float bounce, float shake, int rotationSpeed)
    {
        float timer = 0f;

        // Bounce
        transform.localScale = startScale * bounce;
        yield return new WaitForSeconds(0.05f);
        transform.localScale = startScale;

        Vector3 randomUpwardDirection = new Vector3(
        UnityEngine.Random.Range(-0.5f, 0.5f),
        UnityEngine.Random.Range(0.7f, 1f),
        0f
        ).normalized;

        while (timer < lifetime)
        {
            transform.localPosition += randomUpwardDirection * riseSpeed * Time.deltaTime;
            transform.Rotate(0, 10 * rotationSpeed * Time.deltaTime, 0);

            // // Shake
            // if (shake > 0)
            // {
            //     transform.localPosition += new Vector3(UnityEngine.Random.Range(-shake, shake) * Time.deltaTime, 0, 0);
            // }

            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
