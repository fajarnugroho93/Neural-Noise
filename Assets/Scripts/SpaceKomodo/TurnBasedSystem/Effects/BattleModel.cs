using System.Collections.Generic;
using System.Linq;
using SpaceKomodo.TurnBasedSystem.Characters;

namespace SpaceKomodo.TurnBasedSystem.Core
{
    public class BattleModel
    {
        private readonly List<CharacterModel> _allCharacters = new();
        
        public void RegisterCharacter(CharacterModel character)
        {
            if (!_allCharacters.Contains(character))
            {
                _allCharacters.Add(character);
            }
        }
        
        public void UnregisterCharacter(CharacterModel character)
        {
            _allCharacters.Remove(character);
        }
        
        public List<CharacterModel> GetAllCharacters()
        {
            return new List<CharacterModel>(_allCharacters);
        }
        
        public List<CharacterModel> GetAllAllies(CharacterModel character)
        {
            return _allCharacters.Where(model => model.IsHero() == character.IsHero()).ToList();
        }
        
        public List<CharacterModel> GetAllEnemies(CharacterModel character)
        {
            return _allCharacters.Where(model => model.IsHero() != character.IsHero()).ToList();
        }
        
        public List<CharacterModel> GetAdjacentCharacters(CharacterModel character)
        {
            var result = new List<CharacterModel>();
            
            foreach (var otherCharacter in _allCharacters)
            {
                if (character != otherCharacter && IsAdjacent(character, otherCharacter))
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
    }
}