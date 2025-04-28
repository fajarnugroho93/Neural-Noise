using SpaceKomodo.TurnBasedSystem.Effects;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects
{
    public class EffectRegistry
    {
        private readonly DamageCalculator _damageCalculator;
        private readonly StatusEffectManager _statusEffectManager;
        private readonly ResourceManager _resourceManager;

        public EffectRegistry(
            DamageCalculator damageCalculator,
            StatusEffectManager statusEffectManager,
            ResourceManager resourceManager)
        {
            _damageCalculator = damageCalculator;
            _statusEffectManager = statusEffectManager;
            _resourceManager = resourceManager;
            
            RegisterAllEffects();
        }

        private void RegisterAllEffects()
        {
            RegisterBasicEffects();
            RegisterStatusEffects();
            RegisterResourceEffects();
        }

        private void RegisterBasicEffects()
        {
            var damageBehavior = new DamageBehavior(_damageCalculator);
            EffectTypeRegistry.RegisterEffectType(
                EffectType.Damage,
                EffectCategory.Basic,
                typeof(DamageEffectModel),
                () => new DamageEffectModel { Amount = 10, CriticalChance = 0.1f, CriticalMultiplier = 1.5f },
                damageBehavior
            );
            
            var healBehavior = new HealBehavior();
            EffectTypeRegistry.RegisterEffectType(
                EffectType.Heal,
                EffectCategory.Basic,
                typeof(HealEffectModel),
                () => new HealEffectModel { Amount = 15, CriticalChance = 0.1f, CriticalMultiplier = 1.5f },
                healBehavior
            );
            
            var shieldBehavior = new ShieldBehavior();
            EffectTypeRegistry.RegisterEffectType(
                EffectType.Shield,
                EffectCategory.Basic,
                typeof(ShieldEffectModel),
                () => new ShieldEffectModel { Amount = 10, Duration = 3 },
                shieldBehavior
            );
        }

        private void RegisterStatusEffects()
        {
            var statusBehavior = new StatusBehavior(_statusEffectManager);
            
            EffectTypeRegistry.RegisterEffectType(
                EffectType.Poison,
                EffectCategory.Status,
                typeof(PoisonEffectModel),
                () => new PoisonEffectModel { Amount = 5, Duration = 3 },
                statusBehavior
            );
            
            EffectTypeRegistry.RegisterEffectType(
                EffectType.Burn,
                EffectCategory.Status,
                typeof(BurnEffectModel),
                () => new BurnEffectModel { Amount = 5, Duration = 3 },
                statusBehavior
            );
            
            EffectTypeRegistry.RegisterEffectType(
                EffectType.Stun,
                EffectCategory.Status,
                typeof(StunEffectModel),
                () => new StunEffectModel { Amount = 1, Duration = 1 },
                statusBehavior
            );
            
            EffectTypeRegistry.RegisterEffectType(
                EffectType.Blind,
                EffectCategory.Status,
                typeof(BlindEffectModel),
                () => new BlindEffectModel { Amount = 1, Duration = 2 },
                statusBehavior
            );
            
            EffectTypeRegistry.RegisterEffectType(
                EffectType.Silence,
                EffectCategory.Status,
                typeof(SilenceEffectModel),
                () => new SilenceEffectModel { Amount = 1, Duration = 2 },
                statusBehavior
            );
            
            EffectTypeRegistry.RegisterEffectType(
                EffectType.Root,
                EffectCategory.Status,
                typeof(RootEffectModel),
                () => new RootEffectModel { Amount = 1, Duration = 2 },
                statusBehavior
            );
            
            EffectTypeRegistry.RegisterEffectType(
                EffectType.Taunt,
                EffectCategory.Status,
                typeof(TauntEffectModel),
                () => new TauntEffectModel { Amount = 1, Duration = 2 },
                statusBehavior
            );
        }

        private void RegisterResourceEffects()
        {
            var resourceBehavior = new ResourceBehavior(_resourceManager);
            
            EffectTypeRegistry.RegisterEffectType(
                EffectType.Energy,
                EffectCategory.Resource,
                typeof(EnergyEffectModel),
                () => new EnergyEffectModel { Amount = 10 },
                resourceBehavior
            );
            
            EffectTypeRegistry.RegisterEffectType(
                EffectType.Rage,
                EffectCategory.Resource,
                typeof(RageEffectModel),
                () => new RageEffectModel { Amount = 5 },
                resourceBehavior
            );
            
            EffectTypeRegistry.RegisterEffectType(
                EffectType.Mana,
                EffectCategory.Resource,
                typeof(ManaEffectModel),
                () => new ManaEffectModel { Amount = 10 },
                resourceBehavior
            );
            
            EffectTypeRegistry.RegisterEffectType(
                EffectType.Focus,
                EffectCategory.Resource,
                typeof(FocusEffectModel),
                () => new FocusEffectModel { Amount = 10 },
                resourceBehavior
            );
            
            EffectTypeRegistry.RegisterEffectType(
                EffectType.Charge,
                EffectCategory.Resource,
                typeof(ChargeEffectModel),
                () => new ChargeEffectModel { Amount = 10 },
                resourceBehavior
            );
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