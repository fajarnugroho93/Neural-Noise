using System;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects
{
    [Serializable]
    public class SkillEffectModelContainer : ICloneable
    {
        public EffectType EffectType;
        
        [SerializeField] private string _serializedData;
        
        private BaseSkillEffectModel _cachedEffectModel;
        
        public BaseSkillEffectModel GetEffectModel()
        {
            if (_cachedEffectModel != null)
                return _cachedEffectModel;
            
            if (string.IsNullOrEmpty(_serializedData))
            {
                _cachedEffectModel = CreateDefaultModel();
                return _cachedEffectModel;
            }
            
            try
            {
                _cachedEffectModel = DeserializeModel();
                return _cachedEffectModel;
            }
            catch
            {
                Debug.LogError($"Failed to deserialize effect model of type {EffectType}. Creating default.");
                _cachedEffectModel = CreateDefaultModel();
                return _cachedEffectModel;
            }
        }
        
        public void SetEffectModel(BaseSkillEffectModel model)
        {
            if (model == null)
                return;
            
            EffectType = model.EffectType;
            _cachedEffectModel = model;
            _serializedData = JsonUtility.ToJson(model);
        }
        
        private BaseSkillEffectModel DeserializeModel()
        {
            switch (EffectType)
            {
                case EffectType.Damage:
                    return JsonUtility.FromJson<DamageEffectModel>(_serializedData);
                case EffectType.Heal:
                    return JsonUtility.FromJson<HealEffectModel>(_serializedData);
                case EffectType.Shield:
                    return JsonUtility.FromJson<ShieldEffectModel>(_serializedData);
                case EffectType.Status:
                    return JsonUtility.FromJson<StatusEffectModel>(_serializedData);
                case EffectType.Resource:
                    return JsonUtility.FromJson<ResourceEffectModel>(_serializedData);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private BaseSkillEffectModel CreateDefaultModel()
        {
            switch (EffectType)
            {
                case EffectType.Damage:
                    return new DamageEffectModel { Amount = 10 };
                case EffectType.Heal:
                    return new HealEffectModel { Amount = 10 };
                case EffectType.Shield:
                    return new ShieldEffectModel { Amount = 10, Duration = 3 };
                case EffectType.Status:
                    return new StatusEffectModel { StatusType = StatusType.Poison, Duration = 3, Intensity = 5 };
                case EffectType.Resource:
                    return new ResourceEffectModel { ResourceType = ResourceType.Energy, Amount = 10 };
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public object Clone()
        {
            var container = new SkillEffectModelContainer
            {
                EffectType = EffectType,
                _serializedData = _serializedData
            };
            
            if (_cachedEffectModel != null)
            {
                container._cachedEffectModel = (BaseSkillEffectModel)_cachedEffectModel.Clone();
            }
            
            return container;
        }
    }
}