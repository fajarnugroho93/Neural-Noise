using System.IO;
using UnityEditor;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects
{
    public static class EffectTemplate
    {
        private const string BasePath = "Assets/Scripts/SpaceKomodo/TurnBasedSystem/Characters/Skills/Effects";
        private const string ModelsPath = BasePath + "/Models";
        private const string BehaviorsPath = BasePath + "/Behaviors";
        
        static EffectTemplate()
        {
            Directory.CreateDirectory(ModelsPath);
            Directory.CreateDirectory(BehaviorsPath);
        }
        
        public static void GenerateEffectFiles(string effectName, EffectCategory category)
        {
            GenerateModelClass(effectName, category);
            GenerateBehaviorClass(effectName, category);
            GenerateRegistrationCode(effectName, category);
            
            AssetDatabase.Refresh();
        }
        
        private static void GenerateModelClass(string effectName, EffectCategory category)
        {
            string baseClass = GetBaseClassForCategory(category);
            string filePath = $"{ModelsPath}/{effectName}EffectModel.cs";
            
            string template = $@"using System;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects
{{
    [Serializable]
    public class {effectName}EffectModel : {baseClass}
    {{
        public override EffectType Type => EffectType.{effectName};
    }}
}}";
            
            File.WriteAllText(filePath, template);
        }
        
        private static void GenerateBehaviorClass(string effectName, EffectCategory category)
        {
            string interfaceName = GetBehaviorInterfaceForCategory(category);
            string behaviorTemplate = GetBehaviorForCategory(category);
            string filePath = $"{BehaviorsPath}/{effectName}Behavior.cs";
            
            string template = $@"using System.Collections.Generic;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects
{{
    public class {effectName}Behavior : {interfaceName}
    {{
        {behaviorTemplate}
    }}
}}";
            
            File.WriteAllText(filePath, template);
        }
        
        private static void GenerateRegistrationCode(string effectName, EffectCategory category)
        {
            Debug.Log($"Registration code for {effectName} would be generated here.");
        }
        
        private static string GetBaseClassForCategory(EffectCategory category)
        {
            return category switch
            {
                EffectCategory.Basic => "InstantEffectModel",
                EffectCategory.Status => "StatusEffectModel",
                EffectCategory.Resource => "StatusEffectModel",
                _ => "StatusEffectModel"
            };
        }
        
        private static string GetBehaviorInterfaceForCategory(EffectCategory category)
        {
            return "IEffectBehavior";
        }
        
        private static string GetBehaviorForCategory(EffectCategory category)
        {
            switch (category)
            {
                case EffectCategory.Basic:
                    return @"private readonly DamageCalculator _damageCalculator;
        
        public DamageBehavior(DamageCalculator damageCalculator)
        {
            _damageCalculator = damageCalculator;
        }

        public void Execute(CharacterModel source, CharacterModel target, IEffectModel effectModel)
        {
            if (!(effectModel is InstantEffectModel model))
                return;

            var isCritical = Random.value < model.CriticalChance;
            var finalAmount = model.Amount;
            
            if (isCritical)
            {
                finalAmount = Mathf.RoundToInt(finalAmount * model.CriticalMultiplier);
            }
            
            // TODO: Apply effect
        }

        public Dictionary<string, object> PredictEffect(CharacterModel source, CharacterModel target, IEffectModel effectModel)
        {
            var result = new Dictionary<string, object>();
            
            if (!(effectModel is InstantEffectModel model))
                return result;

            var normalAmount = model.Amount;
            var criticalAmount = Mathf.RoundToInt(model.Amount * model.CriticalMultiplier);

            result[""MinAmount""] = normalAmount;
            result[""MaxAmount""] = criticalAmount;
            result[""CriticalChance""] = model.CriticalChance;

            return result;
        }";
                
                case EffectCategory.Status:
                    return @"private readonly StatusEffectManager _statusEffectManager;
        
        public StatusBehavior(StatusEffectManager statusEffectManager)
        {
            _statusEffectManager = statusEffectManager;
        }
        
        public void Execute(CharacterModel source, CharacterModel target, IEffectModel effectModel)
        {
            if (!(effectModel is StatusEffectModel model))
                return;
                
            _statusEffectManager.ApplyStatus(
                target, 
                (int)effectModel.Type, 
                model.Duration, 
                model.Amount);
        }
        
        public Dictionary<string, object> PredictEffect(CharacterModel source, CharacterModel target, IEffectModel effectModel)
        {
            var result = new Dictionary<string, object>();
            
            if (!(effectModel is StatusEffectModel model))
                return result;
                
            result[""EffectType""] = effectModel.Type;
            result[""Duration""] = model.Duration;
            result[""Intensity""] = model.Amount;
            
            return result;
        }";
                
                case EffectCategory.Resource:
                    return @"private readonly ResourceManager _resourceManager;
        
        public ResourceBehavior(ResourceManager resourceManager)
        {
            _resourceManager = resourceManager;
        }
        
        public void Execute(CharacterModel source, CharacterModel target, IEffectModel effectModel)
        {
            if (!(effectModel is StatusEffectModel model))
                return;
            
            var amount = model.Amount;
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
            
            if (!(effectModel is StatusEffectModel model))
                return result;
            
            result[""ResourceType""] = effectModel.Type;
            result[""Amount""] = model.Amount;
            
            return result;
        }";
                
                default:
                    return @"public void Execute(CharacterModel source, CharacterModel target, IEffectModel effectModel)
        {
            // TODO: Implement effect execution
        }
        
        public Dictionary<string, object> PredictEffect(CharacterModel source, CharacterModel target, IEffectModel effectModel)
        {
            return new Dictionary<string, object>();
        }";
            }
        }
    }
}