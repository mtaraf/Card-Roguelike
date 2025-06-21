using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Enemy : Character
{
    [SerializeField] private int moveset;

    // Attributes

    public override void Start()
    {
        base.Start();
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

    public override void processCardEffects(CardEffects effects)
    {
        base.processCardEffects(effects);
    }
}
