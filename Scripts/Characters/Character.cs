using System.Collections;
using UnityEngine;

public enum Effects
{
    POISON,
    FREEZE,
    THORNS
}

public class Character : MonoBehaviour
{
    // Attributes
    [SerializeField] protected int maxHealth;
    protected int currentHealth; // Set current health to max health only when a run starts not in start for player
    protected int strength;
    protected int weakness;
    protected int armor;
    protected int ward;

    // UI
    protected HealthAndStatus healthAndStatus;

    public virtual void Start()
    {

        currentHealth = maxHealth;

        // Set up after first frame
        StartCoroutine(findComponentsAfterFrame());
    }

    private IEnumerator findComponentsAfterFrame()
    {
        yield return null;

        // UI set up
        healthAndStatus = GetComponent<HealthAndStatus>();
        if (healthAndStatus == null)
        {
            Debug.LogError("Could not find Health and Status for this obj: " + name);
        }
        else
        {
            healthAndStatus.setHealth(currentHealth, maxHealth);
        }
    }
}
