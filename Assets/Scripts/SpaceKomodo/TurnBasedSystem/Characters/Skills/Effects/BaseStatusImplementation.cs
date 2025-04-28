namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects
{
    public abstract class BaseStatusImplementation : IStatusEffectImplementation
    {
        public virtual void OnApplied(CharacterModel target, int intensity) { }
        public virtual void OnRemoved(CharacterModel target, int intensity) { }
        public virtual void OnRoundStart(CharacterModel target, int intensity) { }
    }
}