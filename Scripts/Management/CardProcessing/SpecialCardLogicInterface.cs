using System.Collections.Generic;

public interface SpecialCardLogicInterface
{
    List<CardEffect> process(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies, ParentSceneController parentSceneController, CardProcessor mistbornCardProcessor);
}


public interface MistbornSpecialCardLogicInterface
{
    List<CardEffect> process(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies, ParentSceneController parentSceneController, MistbornCardProcessor mistbornCardProcessor);
}