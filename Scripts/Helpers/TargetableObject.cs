using UnityEngine;
using UnityEngine.EventSystems;

public class TargetableObject : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    [SerializeField] private GameObject targetSprites;

    private float height;
    private float width;
    private RectTransform rect;
    private bool isEnemy;

    void Start()
    {
        rect = GetComponent<RectTransform>();

        // Match height/width of targetSprites to gameObject
        RectTransform targetRect = targetSprites.GetComponent<RectTransform>();
        if (targetRect != null)
        {
            height = rect.rect.height;
            width = rect.rect.width;

            targetRect.sizeDelta = new Vector2(width, height);
        }

        // Check if player or enemy
        if (GetComponent<Player>() != null)
        {
            isEnemy = false;
        }
        else
        {
            isEnemy = true;
        }
    }

    void Update()
    {
        if (HandManager.instance.hasSelectedCard())
        {
            // Check if this object can be targeted before showing target
            Card card = HandManager.instance.getSelectedCard();
            if ((card.getCardTarget() == Target.Player && !isEnemy) || (card.getCardTarget() != Target.Player && isEnemy))
            {
                showTarget();
            }
        }
        else
        {
            hideTarget();
        }
    }

    // Listener for the user to play cards on this enemy
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Target clicked");
        if (HandManager.instance.hasSelectedCard())
        {
            // Check if type of card can be used on enemy


            Card card = HandManager.instance.getSelectedCard();

            // Call enemy/player processCard function
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // For hover on target, probably stop animations when they are in
    }

    public void showTarget()
    {
        targetSprites.SetActive(true);
    }

    public void hideTarget()
    {
        targetSprites.SetActive(false);
    }
}
