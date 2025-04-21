using System.Collections.Generic;
using System.Linq;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects
{
    public abstract class BaseSkillEffect : ISkillEffect
    {
        public abstract EffectType Type { get; }
        
        public abstract void Execute(CharacterModel source, CharacterModel primaryTarget, BaseSkillEffectModel effectModel);
        
        public abstract Dictionary<string, object> PredictEffect(CharacterModel source, CharacterModel primaryTarget, BaseSkillEffectModel effectModel);
        
        public List<CharacterModel> GetTargets(CharacterModel source, CharacterModel primaryTarget, RelativeTarget relativeTarget)
        {
            var targets = new List<CharacterModel>();
            
            switch (relativeTarget)
            {
                case RelativeTarget.Primary:
                    targets.Add(primaryTarget);
                    break;
                    
                case RelativeTarget.Self:
                    targets.Add(source);
                    break;
                    
                case RelativeTarget.AllAllies:
                    targets.AddRange(GetAllAllies(source));
                    break;
                    
                case RelativeTarget.OtherAllies:
                    targets.AddRange(GetAllAllies(source).Where(ally => ally != source));
                    break;
                    
                case RelativeTarget.AllEnemies:
                    targets.AddRange(GetAllEnemies(source));
                    break;
                    
                case RelativeTarget.AdjacentTargets:
                    targets.AddRange(GetAdjacentTargets(primaryTarget));
                    break;
                    
                case RelativeTarget.All:
                    targets.AddRange(GetAllCharacters());
                    break;
            }
            
            return targets;
        }
        
        private List<CharacterModel> GetAllAllies(CharacterModel character)
        {
            return GetAllCharacters().Where(model => model.IsHero() == character.IsHero()).ToList();
        }
        
        private List<CharacterModel> GetAllEnemies(CharacterModel character)
        {
            return GetAllCharacters().Where(model => model.IsHero() != character.IsHero()).ToList();
        }
        
        private List<CharacterModel> GetAdjacentTargets(CharacterModel target)
        {
            var result = new List<CharacterModel>();
            var allCharacters = GetAllCharacters();
            
            foreach (var otherCharacter in allCharacters)
            {
                if (IsAdjacent(target, otherCharacter))
                {
                    result.Add(otherCharacter);
                }
            }
            
            return result;
        }
        
        private bool IsAdjacent(CharacterModel a, CharacterModel b)
        {
            return true;
        }
        
        private List<CharacterModel> GetAllCharacters()
        {
            return new List<CharacterModel>();
        }
    }
}