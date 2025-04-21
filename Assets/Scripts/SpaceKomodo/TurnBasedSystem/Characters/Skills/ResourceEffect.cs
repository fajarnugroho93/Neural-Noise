using System.Collections.Generic;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills
{
    public class ResourceEffect : BaseSkillEffect
    {
        private readonly ResourceManager _resourceManager;
        
        public ResourceEffect(ResourceManager resourceManager)
        {
            _resourceManager = resourceManager;
        }
        
        public override EffectType Type => EffectType.Resource;
        
        public override void Execute(CharacterModel source, CharacterModel primaryTarget, EffectParameters parameters)
        {
            var resourceType = parameters.GetInt("ResourceType", 0);
            var amount = parameters.GetInt("Amount", 0);
            
            if (amount > 0)
            {
                _resourceManager.AddResource(primaryTarget, resourceType, amount);
            }
            else if (amount < 0)
            {
                _resourceManager.ConsumeResource(primaryTarget, resourceType, -amount);
            }
        }
        
        public override Dictionary<string, object> PredictEffect(CharacterModel source, CharacterModel primaryTarget, EffectParameters parameters)
        {
            var result = new Dictionary<string, object>();
            
            var resourceType = parameters.GetInt("ResourceType", 0);
            var amount = parameters.GetInt("Amount", 0);
            
            result["ResourceType"] = resourceType;
            result["Amount"] = amount;
            
            return result;
        }
    }
}