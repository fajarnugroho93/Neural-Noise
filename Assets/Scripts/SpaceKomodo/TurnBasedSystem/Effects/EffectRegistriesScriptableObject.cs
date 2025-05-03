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
                .Where(effectRegistry => effectRegistry.Category == EffectCategory.Basic)
                .OrderBy(effectRegistry => effectRegistry.EffectType)
                .ToArray();

            StatusEffects = effectRegistryScriptableObjects
                .Where(effectRegistry => effectRegistry.Category == EffectCategory.Status)
                .OrderBy(effectRegistry => effectRegistry.EffectType)
                .ToArray();
                
            ResourceEffects = effectRegistryScriptableObjects
                .Where(effectRegistry => effectRegistry.Category == EffectCategory.Resource)
                .OrderBy(effectRegistry => effectRegistry.EffectType)
                .ToArray();
            
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
        }
#endif
    }
}