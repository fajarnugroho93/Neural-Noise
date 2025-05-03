using SpaceKomodo.TurnBasedSystem.Core;
using SpaceKomodo.TurnBasedSystem.Effects;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects
{
    public class EffectRegistry
    {
        private readonly DamageCalculator _damageCalculator;
        private readonly StatusEffectManager _statusEffectManager;
        private readonly ResourceManager _resourceManager;
        private readonly EffectRegistriesScriptableObject _effectRegistries;

        public EffectRegistry(
            DamageCalculator damageCalculator,
            StatusEffectManager statusEffectManager,
            ResourceManager resourceManager,
            EffectRegistriesScriptableObject effectRegistries)
        {
            _damageCalculator = damageCalculator;
            _statusEffectManager = statusEffectManager;
            _resourceManager = resourceManager;
            _effectRegistries = effectRegistries;
            
            RegisterAllEffects();
        }

        private void RegisterAllEffects()
        {
            EffectRegistryInitialization.InitializeFromScriptableObjects(
                _effectRegistries,
                _damageCalculator,
                _statusEffectManager,
                _resourceManager);
        }

        public IEffectModel CreateModel(EffectType type)
        {
            return EffectTypeRegistry.CreateModel(type);
        }

        public IEffectBehavior GetBehavior(EffectType type)
        {
            return EffectTypeRegistry.GetBehavior(type);
        }
    }
}