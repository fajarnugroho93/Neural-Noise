using SpaceKomodo.TurnBasedSystem.Characters;
using SpaceKomodo.TurnBasedSystem.Characters.Skills;
using SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects;
using VContainer;
using EffectType = SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects.EffectType;

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
                .WithParameter("statusType", (int)EffectType.Poison);
                
            builder.RegisterInstance<IStatusEffectImplementation>(
                new BurnStatusImplementation(damageCalculator))
                .WithParameter("statusType", (int)EffectType.Burn);
                
            builder.RegisterInstance<IStatusEffectImplementation>(
                new StunStatusImplementation())
                .WithParameter("statusType", (int)EffectType.Stun);
                
            builder.RegisterInstance<IStatusEffectImplementation>(
                new SilenceStatusImplementation())
                .WithParameter("statusType", (int)EffectType.Silence);
                
            builder.RegisterInstance<IStatusEffectImplementation>(
                new RootStatusImplementation())
                .WithParameter("statusType", (int)EffectType.Root);
                
            builder.RegisterInstance<IStatusEffectImplementation>(
                new TauntStatusImplementation())
                .WithParameter("statusType", (int)EffectType.Taunt);
        }
    }
    
    public class StunStatusImplementation : IStatusEffectImplementation
    {
        public void OnApplied(CharacterModel target, int intensity) { }
        public void OnRemoved(CharacterModel target, int intensity) { }
        public void OnRoundStart(CharacterModel target, int intensity) { }
    }
    
    public class SilenceStatusImplementation : IStatusEffectImplementation
    {
        public void OnApplied(CharacterModel target, int intensity) { }
        public void OnRemoved(CharacterModel target, int intensity) { }
        public void OnRoundStart(CharacterModel target, int intensity) { }
    }
    
    public class RootStatusImplementation : IStatusEffectImplementation
    {
        public void OnApplied(CharacterModel target, int intensity) { }
        public void OnRemoved(CharacterModel target, int intensity) { }
        public void OnRoundStart(CharacterModel target, int intensity) { }
    }
    
    public class TauntStatusImplementation : IStatusEffectImplementation
    {
        public void OnApplied(CharacterModel target, int intensity) { }
        public void OnRemoved(CharacterModel target, int intensity) { }
        public void OnRoundStart(CharacterModel target, int intensity) { }
    }
}