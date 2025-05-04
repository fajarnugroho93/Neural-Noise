using System;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects.Editor
{
    public static class EffectEditorUtility
    {
        private static EffectRegistry _effectRegistry;
        
        public static EffectRegistry GetEffectRegistry()
        {
            if (_effectRegistry != null)
                return _effectRegistry;
            
            var damageCalculator = new TurnBasedSystem.Effects.DamageCalculator();
            var resourceManager = new ResourceManager();
            var statusEffectManager = new TurnBasedSystem.Effects.StatusEffectManager();
            
            var effectRegistriesScriptableObject = Resources.Load<TurnBasedSystem.Effects.EffectRegistriesScriptableObject>("Data/Effects");
            if (effectRegistriesScriptableObject == null)
            {
                var guids = AssetDatabase.FindAssets("t:EffectRegistriesScriptableObject");
                if (guids.Length > 0)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    effectRegistriesScriptableObject = AssetDatabase.LoadAssetAtPath<TurnBasedSystem.Effects.EffectRegistriesScriptableObject>(path);
                }
            }
            
            _effectRegistry = new EffectRegistry(damageCalculator, statusEffectManager, resourceManager, effectRegistriesScriptableObject);
            return _effectRegistry;
        }
        
        public static IEffectModel DeserializeEffectModel(EffectType type, string serializedData)
        {
            if (string.IsNullOrEmpty(serializedData))
                return null;
                
            try
            {
                var registry = GetEffectRegistry();
                var model = registry.CreateModel(type);
                
                JsonConvert.PopulateObject(serializedData, model);
                
                return model;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to deserialize effect model: {ex.Message}");
                return null;
            }
        }
        
        public static float DrawEffectModel(Rect position, IEffectModel model, float yOffset, float lineHeight, float spacing)
        {
            var properties = EffectModelPropertyCache.GetPropertiesForModel(model);
            
            foreach (var property in properties)
            {
                var propertyRect = new Rect(position.x, position.y + yOffset, position.width, lineHeight);
                var drawer = EffectPropertyDrawerRegistry.GetDrawerForProperty(property);
                
                var propertyName = NicifyPropertyName(property.Name);
                drawer.DrawProperty(propertyRect, property, model, propertyName);
                
                yOffset += drawer.GetPropertyHeight(property, model) + spacing;
            }
            
            return yOffset;
        }
        
        private static string NicifyPropertyName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;
                
            var result = name;
            
            if (result.StartsWith("_"))
                result = result.Substring(1);
                
            var chars = result.ToCharArray();
            if (chars.Length > 0)
                chars[0] = char.ToUpper(chars[0]);
                
            for (var i = 1; i < chars.Length; i++)
            {
                if (char.IsUpper(chars[i]) && !char.IsUpper(chars[i - 1]))
                    result = result.Insert(i, " ");
            }
            
            return result;
        }
        
        public static float CalculateEffectModelHeight(IEffectModel model, float lineHeight, float spacing)
        {
            var properties = EffectModelPropertyCache.GetPropertiesForModel(model);
            var height = 0f;
            
            foreach (var property in properties)
            {
                var drawer = EffectPropertyDrawerRegistry.GetDrawerForProperty(property);
                height += drawer.GetPropertyHeight(property, model) + spacing;
            }
            
            return height;
        }
    }
}