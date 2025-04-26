using SpaceKomodo.TurnBasedSystem.Characters;
using SpaceKomodo.TurnBasedSystem.Characters.Skills;
using SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects;
using VContainer;

namespace SpaceKomodo.TurnBasedSystem.Core
{
    public static class ServiceRegistration
    {
        public static void RegisterEffectSystem(IContainerBuilder builder)
        {
            builder.Register<DamageCalculator>(Lifetime.Singleton);
            builder.Register<ResourceManager>(Lifetime.Singleton);
            builder.Register<StatusEffectManager>(Lifetime.Singleton);
            builder.Register<EffectRegistry>(Lifetime.Singleton);
            builder.Register<EffectTargetResolver>(Lifetime.Singleton).As<IEffectTargetResolver>();
            builder.Register<SkillExecutor>(Lifetime.Singleton);
            builder.Register<BattleModel>(Lifetime.Singleton);
            
            RegisterStatusImplementations(builder);
        }
        
        private static void RegisterStatusImplementations(IContainerBuilder builder)
        {
            var damageCalculator = new DamageCalculator();
            
            builder.RegisterInstance<IStatusEffectImplementation>(
                new PoisonStatusImplementation(damageCalculator))
                .WithParameter("statusType", (int)StatusType.Poison);
                
            builder.RegisterInstance<IStatusEffectImplementation>(
                new BurnStatusImplementation(damageCalculator))
                .WithParameter("statusType", (int)StatusType.Burn);
            
            // Register simple implementations with no constructor parameters
            builder.RegisterInstance<IStatusEffectImplementation>(new StunStatusImplementation())
                .WithParameter("statusType", (int)StatusType.Stun);
                
            builder.RegisterInstance<IStatusEffectImplementation>(new BlindStatusImplementation())
                .WithParameter("statusType", (int)StatusType.Blind);
                
            builder.RegisterInstance<IStatusEffectImplementation>(new SilenceStatusImplementation())
                .WithParameter("statusType", (int)StatusType.Silence);
                
            builder.RegisterInstance<IStatusEffectImplementation>(new RootStatusImplementation())
                .WithParameter("statusType", (int)StatusType.Root);
                
            builder.RegisterInstance<IStatusEffectImplementation>(new TauntStatusImplementation())
                .WithParameter("statusType", (int)StatusType.Taunt);
        }
    }
}