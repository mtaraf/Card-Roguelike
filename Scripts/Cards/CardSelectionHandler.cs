using System.Collections;
using UnityEngine;
using UnityEditor.EventSystems;
using UnityEngine.EventSystems;

public class CardSelectionHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private float verticalMoveAmount = 300f;
    [SerializeField] private float moveTime = 0.1f;
    [Range(0f, 2f), SerializeField] private float scaleAmount = 1.1f;

    private Vector3 startPos;
    private Vector3 startScale;

    private int originalIndex = -1;

    private void Start()
    {
        startPos = transform.position;
        startScale = transform.localScale;
        originalIndex = transform.parent.gameObject.transform.GetSiblingIndex();
    }

    private IEnumerator moveCard(bool startingAnimation)
    {
        Vector3 endPos;
        Vector3 endScale;

        float elapsedTime = 0f;
        while (elapsedTime < moveTime)
        {
            elapsedTime += Time.deltaTime;

            if (startingAnimation)
            {
                endPos = startPos + new Vector3(0f, verticalMoveAmount, 0f);
                endScale = startScale * scaleAmount;
            }
            else
            {
                endPos = startPos;
                endScale = startScale;
            }

            // Lerppppp
            Vector3 lerpPos = Vector3.Lerp(transform.position, endPos, (elapsedTime / moveTime));
            Vector3 lerpScale = Vector3.Lerp(transform.localScale, endScale, (elapsedTime / moveTime));

            transform.position = lerpPos;
            transform.localScale = lerpScale;

            yield return null;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        eventData.selectedObject = gameObject;

        // Layer card infront of all others
        transform.parent.gameObject.transform.SetAsLastSibling();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        eventData.selectedObject = null;

        // Set layering back to original
        transform.parent.gameObject.transform.SetSiblingIndex(originalIndex);
    }

    public void OnSelect(BaseEventData eventData)
    {
        StartCoroutine(moveCard(true));
    }

    public void OnDeselect(BaseEventData eventData)
    {
        StartCoroutine(moveCard(false));
    }
}
