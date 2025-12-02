using System.Collections.Generic;


public interface SpecialEnemyCardLogicInterface
{
    List<CardEffect> process(CardModelSO card, Dictionary<EffectType, int> attributes, ParentSceneController parentSceneController, Enemy enemy);
}