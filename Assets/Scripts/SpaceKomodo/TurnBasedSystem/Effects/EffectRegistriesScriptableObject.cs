using System.Linq;
using NaughtyAttributes;
using SpaceKomodo.Editor;
using SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Effects
{
    [CreateAssetMenu(fileName = "Effect Registries", menuName = "TurnBasedSystem/Effect Registries", order = -9000)]
    public class EffectRegistriesScriptableObject : ScriptableObject
    {
        public EffectRegistryScriptableObject[] BasicEffects;
        public EffectRegistryScriptableObject[] StatusEffects;
        public EffectRegistryScriptableObject[] ResourceEffects;
        
#if UNITY_EDITOR
        [Button]
        public void DoFetchAssets()
        {
            var effectRegistryScriptableObjects = EditorHelpers.GetAllInstances<EffectRegistryScriptableObject>();
            foreach (var effectRegistry in effectRegistryScriptableObjects)
            {
                effectRegistry.RenameAsset();
            }

            BasicEffects = effectRegistryScriptableObjects
                .Where(effectRegistry => 
                    (int)effectRegistry.EffectType >= (int)EffectCategory.Basic && 
                    (int)effectRegistry.EffectType < (int)EffectCategory.Status)
                .OrderBy(effectRegistry => (int)effectRegistry.EffectType)
                .ToArray();

            StatusEffects = effectRegistryScriptableObjects
                .Where(effectRegistry => 
                    (int)effectRegistry.EffectType >= (int)EffectCategory.Status && 
                    (int)effectRegistry.EffectType < (int)EffectCategory.Resource)
                .OrderBy(effectRegistry => (int)effectRegistry.EffectType)
                .ToArray();
        
            ResourceEffects = effectRegistryScriptableObjects
                .Where(effectRegistry => 
                    (int)effectRegistry.EffectType >= (int)EffectCategory.Resource && 
                    (int)effectRegistry.EffectType < (int)EffectCategory.Resource + 10000)
                .OrderBy(effectRegistry => (int)effectRegistry.EffectType)
                .ToArray();
    
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
        }
#endif
    }
}