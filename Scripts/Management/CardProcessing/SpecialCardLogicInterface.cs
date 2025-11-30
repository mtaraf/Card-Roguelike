using System.Collections.Generic;


public interface SpecialCardLogicInterface
{
    List<CardEffect> process(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies, ParentSceneController parentSceneController);
}