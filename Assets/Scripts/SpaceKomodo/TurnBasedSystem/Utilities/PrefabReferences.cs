using System;
using SpaceKomodo.TurnBasedSystem.Views;

namespace SpaceKomodo.TurnBasedSystem.Utilities
{
    [Serializable]
    public class PrefabReferences
    {
        public EffectVisualView DamageVisualPrefab;
        public EffectVisualView HealingVisualPrefab;
        public TargetIndicatorView TargetIndicatorPrefab;
    }
}