using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CulverSceneController : ParentSceneController
{
    public static CulverSceneController instance;
    [SerializeField] private GameObject culverGameObject;

    // UI
    private CulverSceneUIController culverSceneUIController;

    // Game States
    private int turns = 5;

    public override void Start()
    {
        culverSceneUIController = gameObject.AddComponent<CulverSceneUIController>();
        InitializeScene();
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public override void InitializeScene()
    {
        GameManager.instance.setCurrentSceneController(instance);
        culverSceneUIController.Initialize();
        updateTurnCount(1);

        base.InitializeScene();

        // Spawn Culver
        GameObject culver = Instantiate(culverGameObject, enemySpawnLocation.transform);
        Enemy culverComponent = culver.GetComponent<Enemy>();
        enemies.Add(culverComponent);

        // Start turn sequence
        TurnManager.instance.Initialize(player, enemies);
        TurnManager.instance.startCulver(turns);
    }

    public void updateTurnCount(int currentTurn)
    {
        culverSceneUIController.updateTurnCount(currentTurn, turns);
    }

    public override void updateDrawPile(int count)
    {
        culverSceneUIController.updateDrawPile(count);
    }

    public override void updateDiscardPile(int count)
    {
        culverSceneUIController.updateDiscardPile(count);
    }
}