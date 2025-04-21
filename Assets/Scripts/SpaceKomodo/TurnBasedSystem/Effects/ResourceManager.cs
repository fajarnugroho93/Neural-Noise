using System.Collections.Generic;
using System.Linq;

namespace SpaceKomodo.TurnBasedSystem.Characters
{
    public class ResourceManager
    {
        private readonly Dictionary<CharacterModel, Dictionary<int, int>> _characterResources = new Dictionary<CharacterModel, Dictionary<int, int>>();
        
        public int GetResource(CharacterModel character, int resourceType)
        {
            if (!_characterResources.TryGetValue(character, out var resources))
            {
                return 0;
            }
            
            if (!resources.TryGetValue(resourceType, out var amount))
            {
                return 0;
            }
            
            return amount;
        }
        
        public void AddResource(CharacterModel character, int resourceType, int amount)
        {
            if (amount <= 0) return;
            
            if (!_characterResources.TryGetValue(character, out var resources))
            {
                resources = new Dictionary<int, int>();
                _characterResources[character] = resources;
            }
            
            if (!resources.TryGetValue(resourceType, out var currentAmount))
            {
                currentAmount = 0;
            }
            
            resources[resourceType] = currentAmount + amount;
        }
        
        public bool ConsumeResource(CharacterModel character, int resourceType, int amount)
        {
            if (amount <= 0) return true;
            
            if (!_characterResources.TryGetValue(character, out var resources))
            {
                return false;
            }
            
            if (!resources.TryGetValue(resourceType, out var currentAmount))
            {
                return false;
            }
            
            if (currentAmount < amount)
            {
                return false;
            }
            
            resources[resourceType] = currentAmount - amount;
            return true;
        }
        
        public Dictionary<int, int> GetAllResources(CharacterModel character)
        {
            if (!_characterResources.TryGetValue(character, out var resources))
            {
                return new Dictionary<int, int>();
            }
            
            return resources.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
        
        public void ClearResources(CharacterModel character)
        {
            if (_characterResources.TryGetValue(character, out var resources))
            {
                resources.Clear();
            }
        }
    }
}