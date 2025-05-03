namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects
{
    public enum EffectType
    {
        // None = 0
        None = 0,
        
        // Basic = 10000
        Damage = 10000,
        Heal = 10001,
        Shield = 10002,
        
        // Status = 20000
        Poison = 20000,
        Burn = 20001,
        Stun = 20002,
        
        // Resource = 30000
        Energy = 30000,
        Rage = 30001,
    }
}