using System;
using System.Collections.Generic;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects
{
    public class EffectTypeInfo
    {
        public EffectType Type { get; }
        public EffectCategory Category { get; }
        public Type ModelType { get; }
        
        public EffectTypeInfo(EffectType type, EffectCategory category, Type modelType)
        {
            Type = type;
            Category = category;
            ModelType = modelType;
        }
    }
    
    public static class EffectTypeRegistry
    {
        private static readonly Dictionary<EffectType, EffectTypeInfo> _effectTypeInfos = new();
        private static readonly Dictionary<EffectType, Func<IEffectModel>> _modelFactories = new();
        private static readonly Dictionary<EffectType, IEffectBehavior> _effectBehaviors = new();
        
        public static void RegisterEffectType(
            EffectType type,
            EffectCategory category,
            Type modelType,
            Func<IEffectModel> modelFactory,
            IEffectBehavior behavior)
        {
            _effectTypeInfos[type] = new EffectTypeInfo(type, category, modelType);
            _modelFactories[type] = modelFactory;
            _effectBehaviors[type] = behavior;
        }
        
        public static EffectTypeInfo GetEffectTypeInfo(EffectType type)
        {
            if (_effectTypeInfos.TryGetValue(type, out var info))
            {
                return info;
            }
            
            throw new ArgumentException($"Effect type {type} not registered");
        }
        
        public static IEnumerable<EffectTypeInfo> GetAllEffectTypeInfos()
        {
            return _effectTypeInfos.Values;
        }
        
        public static IEnumerable<EffectTypeInfo> GetEffectTypeInfosByCategory(EffectCategory category)
        {
            foreach (var info in _effectTypeInfos.Values)
            {
                if (info.Category == category)
                {
                    yield return info;
                }
            }
        }
        
        public static IEffectModel CreateModel(EffectType type)
        {
            if (_modelFactories.TryGetValue(type, out var factory))
            {
                return factory();
            }
            
            throw new ArgumentException($"No model factory registered for effect type {type}");
        }
        
        public static IEffectBehavior GetBehavior(EffectType type)
        {
            if (_effectBehaviors.TryGetValue(type, out var behavior))
            {
                return behavior;
            }
            
            throw new ArgumentException($"No behavior registered for effect type {type}");
        }
    }
}