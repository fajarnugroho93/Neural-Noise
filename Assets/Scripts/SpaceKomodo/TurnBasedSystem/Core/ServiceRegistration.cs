using SpaceKomodo.TurnBasedSystem.Characters;
using SpaceKomodo.TurnBasedSystem.Characters.Skills;
using SpaceKomodo.TurnBasedSystem.Core;
using VContainer;

namespace SpaceKomodo.TurnBasedSystem
{
    public static class ServiceRegistration
    {
        public static void RegisterEffectSystem(IContainerBuilder builder)
        {
            builder.Register<DamageCalculator>(Lifetime.Singleton);
            builder.Register<ResourceManager>(Lifetime.Singleton);
            builder.Register<StatusEffectManager>(Lifetime.Singleton);
            builder.Register<EffectFactory>(Lifetime.Singleton);
            builder.Register<SkillExecutor>(Lifetime.Singleton);
            builder.Register<BattleModel>(Lifetime.Singleton);
            
            builder.RegisterInstance<IStatusEffectImplementation>(
                    new PoisonStatusImplementation(new DamageCalculator()))
                .WithParameter("statusType", 1);
                
            builder.RegisterInstance<IStatusEffectImplementation>(
                    new BurnStatusImplementation(new DamageCalculator()))
                .WithParameter("statusType", 2);
        }
    }
}