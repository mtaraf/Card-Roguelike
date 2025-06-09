using UnityEngine;
using UnityEngine.EventSystems;

public enum EnemyEffect {
    POiSON,
    FREEZE,
    THORNS
}

public class Enemy : MonoBehaviour, IPointerClickHandler
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
    }

    private void playTurn()
    {
        
    }

    private void endTurn()
    {
        // Notify GameManager that turn has ended
    }



    // Listener for the user to play cards on this enemy
    public void OnPointerClick(PointerEventData eventData)
    {
        if (HandManager.instance.getSelectedCard() != null)
        {
            // Check if type of card can be used on enemy
            GameObject card = HandManager.instance.getSelectedCard();

            Debug.Log("Used card on enemy");
        }
    }
}
