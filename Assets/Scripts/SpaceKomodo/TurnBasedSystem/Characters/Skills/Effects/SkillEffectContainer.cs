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
            return Type switch
            {
                EffectType.Damage => JsonUtility.FromJson<DamageEffectModel>(_serializedData),
                EffectType.Heal => JsonUtility.FromJson<HealEffectModel>(_serializedData),
                EffectType.Shield => JsonUtility.FromJson<ShieldEffectModel>(_serializedData),
                EffectType.Poison => JsonUtility.FromJson<PoisonEffectModel>(_serializedData),
                EffectType.Burn => JsonUtility.FromJson<BurnEffectModel>(_serializedData),
                EffectType.Stun => JsonUtility.FromJson<StunEffectModel>(_serializedData),
                EffectType.Energy => JsonUtility.FromJson<EnergyEffectModel>(_serializedData),
                EffectType.Rage => JsonUtility.FromJson<RageEffectModel>(_serializedData),
                _ => throw new ArgumentOutOfRangeException()
            };
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