using System.Collections.Generic;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects
{
    public enum EffectType
    {
        Damage,
        Heal,
        Shield,
        Poison,
        Burn,
        Stun,
        Blind,
        Silence,
        Root,
        Taunt,
        Energy,
        Rage,
        Mana,
        Focus,
        Charge
    }

    public interface IEffectModel
    {
        EffectType Type { get; }
        RelativeTarget Target { get; set; }
        object Clone();
    }

    public interface IAmountEffect
    {
        int Amount { get; set; }
    }

    public interface IDurationEffect
    {
        int Duration { get; set; }
    }
    
    public interface ICriticalEffect
    {
        float CriticalChance { get; set; }
        float CriticalMultiplier { get; set; }
    }

    public interface IInstantEffect : IEffectModel, IAmountEffect, ICriticalEffect
    {
        
    }

    public interface IStatusEffect : IEffectModel, IAmountEffect, IDurationEffect, ICriticalEffect
    {
        
    }

    public interface IEffectBehavior
    {
        void Execute(CharacterModel source, CharacterModel target, IEffectModel effectModel);
        Dictionary<string, object> PredictEffect(CharacterModel source, CharacterModel target, IEffectModel effectModel);
    }

    public interface IEffectTargetResolver
    {
        List<CharacterModel> ResolveTargets(CharacterModel source, CharacterModel primaryTarget, RelativeTarget targeting);
    }
}