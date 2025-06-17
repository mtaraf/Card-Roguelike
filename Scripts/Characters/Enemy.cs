using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public enum EnemyEffect {
    POISON,
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

        // Check if selected card can target self


        // Code to get mouse position on click, move to Clickable file
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector3 screenPos = Mouse.current.position.ReadValue(); // in pixels
            screenPos.z = -Camera.main.transform.position.z;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

            Debug.Log("World: " + worldPos + " Screen: " + screenPos);
        }
    }

    // Check if selected card can target self, display visual indicator if so
    private void checkSelectedCard()
    {
        if (HandManager.instance.getSelectedCard() != null)
        {
            GameObject card = HandManager.instance.getSelectedCard();
        }
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
        Debug.Log("Enemy clicked");
        if (HandManager.instance.getSelectedCard() != null)
        {
            // Check if type of card can be used on enemy
            GameObject card = HandManager.instance.getSelectedCard();

            Debug.Log("Hit by: " + card);
        }
    }
}
