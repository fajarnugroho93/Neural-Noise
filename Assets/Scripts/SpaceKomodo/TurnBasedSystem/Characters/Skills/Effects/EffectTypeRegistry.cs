using System;
using System.Collections.Generic;
using System.Linq;
using SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects.Behaviors;
using SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects.Models;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects
{
    public static class EffectTypeRegistry
    {
        private static readonly Dictionary<EffectType, IEffectBehavior> _effectBehaviors = new();
        private static readonly Dictionary<EffectType, Type> _modelTypes = new();
        private static readonly Dictionary<EffectType, Func<IEffectModel>> _modelFactories = new();
        private static readonly Dictionary<EffectType, EffectCategory> _effectCategories = new();

        public static void RegisterEffectType(
            EffectType type,
            EffectCategory category,
            Type modelType,
            Func<IEffectModel> modelFactory,
            IEffectBehavior behavior)
        {
            _modelTypes[type] = modelType;
            _modelFactories[type] = modelFactory;
            _effectBehaviors[type] = behavior;
            _effectCategories[type] = category;
        }

        public static IEffectModel CreateModel(EffectType type)
        {
            if (_modelFactories.TryGetValue(type, out var factory))
            {
                return factory();
            }
            
            Debug.LogWarning($"No model factory found for effect type {type}. Creating a default model.");
            return new BasicDamageModel();
        }

        public static IEffectBehavior GetBehavior(EffectType type)
        {
            if (_effectBehaviors.TryGetValue(type, out var behavior))
            {
                return behavior;
            }
            
            Debug.LogWarning($"No behavior found for effect type {type}. Using NoEffect behavior.");
            return new NoneNoneBehavior();
        }

        public static Type GetModelType(EffectType type)
        {
            if (_modelTypes.TryGetValue(type, out var modelType))
            {
                return modelType;
            }
            
            Debug.LogWarning($"No model type found for effect type {type}.");
            return typeof(BasicDamageModel);
        }
        
        public static EffectCategory GetEffectCategory(EffectType type)
        {
            if (_effectCategories.TryGetValue(type, out var category))
            {
                return category;
            }
            
            return EffectCategory.None;
        }
        
        public static EffectType[] GetAllRegisteredEffectTypes()
        {
            return _effectBehaviors.Keys.ToArray();
        }
        
        public static EffectType[] GetEffectTypesByCategory(EffectCategory category)
        {
            return _effectCategories
                .Where(kvp => kvp.Value == category)
                .Select(kvp => kvp.Key)
                .ToArray();
        }

        public static void Clear()
        {
            _modelTypes.Clear();
            _modelFactories.Clear();
            _effectBehaviors.Clear();
            _effectCategories.Clear();
        }
    }
}