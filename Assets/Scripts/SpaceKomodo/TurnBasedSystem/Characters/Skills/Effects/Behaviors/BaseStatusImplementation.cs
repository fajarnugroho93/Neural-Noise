namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects.Behaviors
{
    public abstract class BaseStatusImplementation : IStatusEffectImplementation
    {
        public EffectType EffectType { get; private set; }

        protected BaseStatusImplementation(EffectType effectType)
        {
            EffectType = effectType;
        }
        
        public virtual void OnApplied(CharacterModel target, int intensity) { }
        public virtual void OnRemoved(CharacterModel target, int intensity) { }
        public virtual void OnRoundStart(CharacterModel target, int intensity) { }
    }
}