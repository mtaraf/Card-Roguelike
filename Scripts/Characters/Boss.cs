using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum BossType
{
    HoldTheLine,
    Culver,
}
public class Boss : Enemy
{
    [SerializeField] private BossType bossType;
    private bool invincible = false;

    public override void Start()
    {
        base.Start();

        StartCoroutine(initializeAfterFrame());
    }

    private IEnumerator initializeAfterFrame()
    {
        yield return null;

        if (bossType == BossType.HoldTheLine)
        {
            invincible = true;
            uIUpdater.setInvincible();
            uIUpdater.setHealth(0, 0);
        }
    }

    public override void processCardEffects(List<CardEffect> effects, Enemy enemy = null)
    {
        base.processCardEffects(effects, enemy);

        if (invincible)
        {
            dead = false;
        }
    }
}
