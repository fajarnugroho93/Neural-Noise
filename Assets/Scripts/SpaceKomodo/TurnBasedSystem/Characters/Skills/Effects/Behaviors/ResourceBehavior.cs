using System.Collections.Generic;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects.Behaviors
{
    public class ResourceBehavior : IEffectBehavior
    {
        private readonly ResourceManager _resourceManager;
        
        public ResourceBehavior(ResourceManager resourceManager)
        {
            _resourceManager = resourceManager;
        }
        
        public void Execute(CharacterModel source, CharacterModel target, IEffectModel effectModel)
        {
            if (!(effectModel is IAmountEffect amountEffect))
                return;
            
            var amount = amountEffect.Amount;
            var resourceType = (int)effectModel.Type;
            
            if (amount > 0)
            {
                _resourceManager.AddResource(target, resourceType, amount);
            }
            else if (amount < 0)
            {
                _resourceManager.ConsumeResource(target, resourceType, -amount);
            }
        }
        
        public Dictionary<string, object> PredictEffect(CharacterModel source, CharacterModel target, IEffectModel effectModel)
        {
            var result = new Dictionary<string, object>();
            
            if (!(effectModel is IAmountEffect amountEffect))
                return result;
            
            result["ResourceType"] = effectModel.Type;
            result["Amount"] = amountEffect.Amount;
            
            return result;
        }
    }
}