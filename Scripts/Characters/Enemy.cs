using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public enum EnemyEffect {
    POISON,
    FREEZE,
    THORNS
}

public class Enemy : MonoBehaviour
{
    [SerializeField] private double health;
    [SerializeField] private double attackPower;
    [SerializeField] private EnemyEffect effect;
    [SerializeField] private int moveset;

    void Start()
    {

    }

    void Update()
    {
        // Check GameManager if it is enemy turn 

        // Check if selected card can target self
    }

    // Check if selected card can target self, display visual indicator if so
    private void checkSelectedCard()
    {
        if (HandManager.instance.hasSelectedCard())
        {
            Card card = HandManager.instance.getSelectedCard();
        }
    }

    private void playTurn()
    {

    }

    private void endTurn()
    {
        // Notify GameManager that turn has ended
    }

    public void processCardEffects(CardEffects effects)
    {
        
    }
}
