namespace SpaceKomodo.TurnBasedSystem.Characters.Skills
{
    public interface IStatusEffectImplementation
    {
        void OnApplied(CharacterModel target, int intensity);
        void OnRemoved(CharacterModel target, int intensity);
        void OnRoundStart(CharacterModel target, int intensity);
    }
}