using System.Collections.Generic;

public class Mistborn : Player
{
    public override void processEndOfRoundEffects()
    {
        if (mythic != null && mythic.getMythicName() == "Crimson Curse")
        {
            int totalBleed = 0;
            // Check all enemies for Bleed stacks
            List<Enemy> enemies = TurnManager.instance.getCurrentEnemies();
            foreach(Enemy enemy in enemies)
            {
                totalBleed += enemy.getAttributeValue(EffectType.Bleed);
            }

            healCharacter(totalBleed / 5);
        }

        base.processEndOfRoundEffects();
    }
}
