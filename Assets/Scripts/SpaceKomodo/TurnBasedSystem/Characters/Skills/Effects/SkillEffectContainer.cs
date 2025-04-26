using System;
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
                _cachedModel = DeserializeModel();
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
            _serializedData = JsonUtility.ToJson(model);
        }
        
        private IEffectModel DeserializeModel()
        {
            var typeInfo = EffectTypeRegistry.GetEffectTypeInfo(Type);
            
            try
            {
                return (IEffectModel)JsonUtility.FromJson(_serializedData, typeInfo.ModelType);
            }
            catch
            {
                return EffectTypeRegistry.CreateModel(Type);
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