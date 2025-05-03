using System;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects
{
    [Serializable]
    public class SkillEffectContainer : ICloneable
    {
        public EffectType Type;
        
        [SerializeField] private string _serializedData;
        
        private IEffectModel _cachedModel;
        
        public IEffectModel GetEffectModel(EffectRegistry registry)
        {
            if (_cachedModel != null)
                return _cachedModel;
            
            if (string.IsNullOrEmpty(_serializedData))
            {
                _cachedModel = registry.CreateModel(Type);
                return _cachedModel;
            }
            
            try
            {
                _cachedModel = DeserializeModel(registry);
                return _cachedModel;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to deserialize effect model of type {Type}: {ex.Message}. Creating default.");
                _cachedModel = registry.CreateModel(Type);
                return _cachedModel;
            }
        }
        
        public void SetEffectModel(IEffectModel model)
        {
            if (model == null)
                return;
            
            Type = model.Type;
            _cachedModel = model;
            _serializedData = JsonConvert.SerializeObject(model);
        }
        
        private IEffectModel DeserializeModel(EffectRegistry registry)
        {
            try
            {
                var behavior = registry.GetBehavior(Type);
                if (behavior != null)
                {
                    var model = registry.CreateModel(Type);
                    JsonConvert.PopulateObject(_serializedData, model);
                    return model;
                }
                
                return registry.CreateModel(Type);
            }
            catch
            {
                return registry.CreateModel(Type);
            }
        }
        
        public object Clone()
        {
            var container = new SkillEffectContainer
            {
                Type = Type,
                _serializedData = _serializedData
            };
            
            if (_cachedModel != null)
            {
                container._cachedModel = (IEffectModel)_cachedModel.Clone();
            }
            
            return container;
        }
    }
}