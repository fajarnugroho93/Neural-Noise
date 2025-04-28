using SpaceKomodo.TurnBasedSystem.Characters;
using SpaceKomodo.TurnBasedSystem.Characters.Skills;
using SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects;
using SpaceKomodo.TurnBasedSystem.Effects;
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
            builder.Register<PoisonStatusImplementation>(Lifetime.Singleton)
                .AsImplementedInterfaces()
                .WithParameter(StatusType.Poison);
               
            builder.Register<BurnStatusImplementation>(Lifetime.Singleton)
                .AsImplementedInterfaces() 
                .WithParameter(StatusType.Burn);
            
            builder.Register<StunStatusImplementation>(Lifetime.Singleton)
                .AsImplementedInterfaces()
                .WithParameter(StatusType.Stun);
                
            builder.Register<BlindStatusImplementation>(Lifetime.Singleton)
                .AsImplementedInterfaces()
                .WithParameter(StatusType.Blind);
                
            builder.Register<SilenceStatusImplementation>(Lifetime.Singleton)
                .AsImplementedInterfaces()
                .WithParameter(StatusType.Silence);
                
            builder.Register<RootStatusImplementation>(Lifetime.Singleton)
                .AsImplementedInterfaces()
                .WithParameter(StatusType.Root);
                
            builder.Register<TauntStatusImplementation>(Lifetime.Singleton)
                .AsImplementedInterfaces()
                .WithParameter(StatusType.Taunt);
        }
    }
}