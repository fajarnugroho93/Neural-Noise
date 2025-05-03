using NaughtyAttributes;
using SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects;
using UnityEditor;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Effects
{
    [CreateAssetMenu(fileName = "New Effect Registry", menuName = "TurnBasedSystem/Effect Registry", order = -9000)]
    public class EffectRegistryScriptableObject : ScriptableObject
    {
        public EffectType EffectType;
        public EffectCategory Category;
        public string ImplementationClassName;
        
        [SerializeField] private string _baseModelTypeName;
        
        [SerializeField] private int _defaultAmount = 10;
        [SerializeField] private int _defaultDuration = 3;
        [SerializeField] private float _defaultCriticalChance = 0.1f;
        [SerializeField] private float _defaultCriticalMultiplier = 1.5f;
        
#if UNITY_EDITOR
        [Button]
        public void DoRenameAsset()
        {
            RenameAsset();
            AssetDatabase.SaveAssets();
        }

        public void RenameAsset()
        {
            var assetPath = AssetDatabase.GetAssetPath(this);
            if (string.IsNullOrEmpty(assetPath))
                return;
                
            var newName = GetAssetName();
            
            AssetDatabase.RenameAsset(assetPath, newName);
            EditorUtility.SetDirty(this);
        }

        public string GetAssetName()
        {
            return $"{(int)EffectType}-{EffectType}";
        }
        
        public void SetBaseModelType()
        {
            _baseModelTypeName = Category switch
            {
                EffectCategory.Basic => "InstantEffectModel",
                EffectCategory.Status => "StatusEffectModel",
                EffectCategory.Resource => "StatusEffectModel",
                _ => "BaseEffectModel"
            };
        }
        
        public IEffectModel CreateDefaultModel()
        {
            var model = EffectTypeRegistry.CreateModel(EffectType);
            
            if (model is IAmountEffect amountEffect)
            {
                amountEffect.Amount = _defaultAmount;
            }
            
            if (model is IDurationEffect durationEffect)
            {
                durationEffect.Duration = _defaultDuration;
            }
            
            if (model is ICriticalEffect criticalEffect)
            {
                criticalEffect.CriticalChance = _defaultCriticalChance;
                criticalEffect.CriticalMultiplier = _defaultCriticalMultiplier;
            }
            
            return model;
        }
        
        public void UpdateDefaultValues(IEffectModel model)
        {
            if (model is IAmountEffect amountEffect)
            {
                _defaultAmount = amountEffect.Amount;
            }
            
            if (model is IDurationEffect durationEffect)
            {
                _defaultDuration = durationEffect.Duration;
            }
            
            if (model is ICriticalEffect criticalEffect)
            {
                _defaultCriticalChance = criticalEffect.CriticalChance;
                _defaultCriticalMultiplier = criticalEffect.CriticalMultiplier;
            }
        }
        
        public string GetModelClassName()
        {
            return $"{EffectType.ToString()}EffectModel";
        }
        
        public string GetBaseModelTypeName()
        {
            return _baseModelTypeName;
        }
        
        public string GetBehaviorClassName()
        {
            return $"{EffectType.ToString()}Behavior";
        }
#endif
    }
}