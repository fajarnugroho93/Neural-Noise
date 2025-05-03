using SpaceKomodo.TurnBasedSystem.Characters;
using SpaceKomodo.TurnBasedSystem.Effects;
using UnityEngine;
using VContainer;

namespace SpaceKomodo.TurnBasedSystem.Core
{
    public class EffectRegistryModel : MonoBehaviour
    {
        [SerializeField] private EffectRegistriesScriptableObject _effectRegistries;
        
        private DamageCalculator _damageCalculator;
        private StatusEffectManager _statusEffectManager;
        private ResourceManager _resourceManager;
        
        [Inject]
        public void Construct(
            DamageCalculator damageCalculator,
            StatusEffectManager statusEffectManager,
            ResourceManager resourceManager)
        {
            _damageCalculator = damageCalculator;
            _statusEffectManager = statusEffectManager;
            _resourceManager = resourceManager;
        }
        
        private void Awake()
        {
            if (_effectRegistries == null)
            {
                _effectRegistries = Resources.Load<EffectRegistriesScriptableObject>("Data/Effects");
            }
            
            if (_effectRegistries != null)
            {
                EffectRegistryInitialization.InitializeFromScriptableObjects(
                    _effectRegistries,
                    _damageCalculator,
                    _statusEffectManager,
                    _resourceManager);
            }
            else
            {
                Debug.LogWarning("EffectRegistries not found. Using fallback effect registration.");
            }
        }
    }
}