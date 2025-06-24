using UnityEditor.ShaderGraph.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;

public class TargetableObject : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    [SerializeField] private GameObject targetSprites;

    private float height;
    private float width;
    private RectTransform rect;
    private bool isEnemy;

    private Player player;

    private Enemy enemy;
    private bool targetable = false;

    void Start()
    {
        player = GetComponent<Player>();
        enemy = GetComponent<Enemy>();

        if (enemy == null && player == null)
        {
            Debug.LogError("No player or enemy component found for this targetable object");
        }


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
        if (player != null)
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

    // Listener for the user to play cards on this object
    public void OnPointerClick(PointerEventData eventData)
    {
        // Check if card is selected and type of card can be used on this entity
        // if (HandManager.instance.hasSelectedCard() && targetSprites.activeSelf)
        // {
        //     // Check if type of card can be used on enemy
        //     Card card = HandManager.instance.getSelectedCard();
        //     CardEffects effects = HandManager.instance.processCard(card);
        //     Debug.Log(effects.getTotalDamage());

        //     // Call enemy/player processCard function
        //     if (isEnemy)
        //     {
        //         enemy.processCardEffects(effects);
        //     }
        //     else
        //     {
        //         player.processCardEffects(effects);
        //     }
        // }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // For hover on target, probably stop animations when they are in
    }

    public void showTarget()
    {
        targetSprites.SetActive(true);
        targetable = true;
    }

    public void hideTarget()
    {
        targetSprites.SetActive(false);
        targetable = false;
    }

    public bool isTargetable()
    {
        return targetable;
    }
}
