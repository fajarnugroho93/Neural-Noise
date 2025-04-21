using System.Collections.Generic;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects
{
    public class ResourceEffect : BaseSkillEffect
    {
        private readonly ResourceManager _resourceManager;
        
        public ResourceEffect(ResourceManager resourceManager)
        {
            _resourceManager = resourceManager;
        }
        
        public override EffectType Type => EffectType.Resource;
        
        public override void Execute(CharacterModel source, CharacterModel primaryTarget, BaseSkillEffectModel effectModel)
        {
            var resourceModel = (ResourceEffectModel)effectModel;
            var amount = resourceModel.Amount;
            
            if (amount > 0)
            {
                _resourceManager.AddResource(primaryTarget, (int)resourceModel.ResourceType, amount);
            }
            else if (amount < 0)
            {
                _resourceManager.ConsumeResource(primaryTarget, (int)resourceModel.ResourceType, -amount);
            }
        }
        
        public override Dictionary<string, object> PredictEffect(CharacterModel source, CharacterModel primaryTarget, BaseSkillEffectModel effectModel)
        {
            var result = new Dictionary<string, object>();
            var resourceModel = (ResourceEffectModel)effectModel;
            
            result["ResourceType"] = resourceModel.ResourceType;
            result["Amount"] = resourceModel.Amount;
            
            return result;
        }
    }
}