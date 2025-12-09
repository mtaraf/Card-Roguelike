using UnityEditor.ShaderGraph.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;

public class TargetableObject : MonoBehaviour
{
    [SerializeField] private GameObject targetSprites;

    private float height;
    private float width;
    private RectTransform rect;
    private bool isEnemy;

    private Player player;

    private Enemy enemy;
    private bool targetable = false;
    private bool hasUpdated = false;

    void Start()
    {
        player = GetComponent<Player>();
        enemy = GetComponent<Enemy>();

        if (enemy == null && player == null)
        {
            Debug.LogError("No player or enemy component found for this targetable object");
        }

        if (targetSprites == null)
        {
            targetSprites = Helpers.findDescendant(transform, "TargetSprites");
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
        if (!HandManager.instance.hasSelectedCard())
        {
            hideTarget();
            hasUpdated = false;
        }
        else if (!hasUpdated)
        {
            updateTarget();
            hasUpdated = true;
        }
    }

    public bool checkEnemyForSpecialParameters(Card card)
    {
        switch (card.getCardTitle())
        {
            case "Divine Smite":
            case "Divine Smite+":
                if (!enemy.hasAttribute(EffectType.Divinity))
                {
                    return false;
                }
                break;
        }
        return true;
    }

    public bool checkPlayerForSpecialParameters(Card card)
    {
        switch (card.getCardTitle())
        {
        }

        return true;
    }

    public void showTarget()
    {
        targetSprites.SetActive(true);
        targetable = true;
    }

    public void hideTarget()
    {
        if (targetSprites != null)
        {
            targetSprites.SetActive(false);
        }
        targetable = false;
    }

    public bool isTargetable()
    {
        return targetable;
    }

    public void updateTarget()
    {
        // Check if this object can be targeted before showing target
        Card card = HandManager.instance.getSelectedCard();
        if (card == null)
        {
            Debug.LogError("Hand Manager has no selected card when trying to update target."); 
            return;
        }

        bool targetable = false;

        if (card.isSpecial())
        {
            if (!isEnemy && card.getCardTarget() == Target.Player)
            {
                targetable = checkPlayerForSpecialParameters(card);
            }
            else if (isEnemy && card.getCardTarget() != Target.Player)
            {
                targetable = checkEnemyForSpecialParameters(card);
            }

            if (!targetable)
            {
                    hideTarget();
                return;
            }
        }

        if ((card.getCardTarget() == Target.Player && !isEnemy) || (card.getCardTarget() != Target.Player && isEnemy))
        {
            targetable = true;
        }

        if (targetable)
        {
            showTarget();
        }
    }
}
