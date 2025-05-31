using UnityEngine;

public enum EnemyEffect {
    POiSON,
    FREEZE,
    THORNS
}

public class Enemy : MonoBehaviour
{


    [SerializeField] private double health;
    [SerializeField] private double attackPower;
    [SerializeField] private EnemyEffect effect;
    [SerializeField] private int moveset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
