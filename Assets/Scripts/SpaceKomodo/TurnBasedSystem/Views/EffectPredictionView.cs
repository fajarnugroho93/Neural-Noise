using System.Collections.Generic;
using System.Text;
using SpaceKomodo.TurnBasedSystem.Characters;
using SpaceKomodo.TurnBasedSystem.Characters.Skills;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceKomodo.TurnBasedSystem.Views
{
    public class EffectPredictionView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _targetNameText;
        [SerializeField] private TMP_Text _predictedEffectsText;
        [SerializeField] private Image _targetHealthBar;
        
        public void UpdatePrediction(CharacterModel target, Dictionary<SkillEffect, int> predictions)
        {
            _targetNameText.text = target.Character.ToString();
            
            // Simulate the effect to show before/after health
            var damage = 0;
            var healing = 0;
            
            if (predictions.TryGetValue(SkillEffect.TargetDamage, out var damageValue))
            {
                damage += damageValue;
            }
            
            if (predictions.TryGetValue(SkillEffect.TargetHeal, out var healValue))
            {
                healing += healValue;
            }
            
            var currentHealth = target.CurrentHealth.Value;
            var maxHealth = target.CurrentMaxHealth.Value;
            var newHealth = Mathf.Clamp(currentHealth - damage + healing, 0, maxHealth);
            
            var healthRatio = (float)currentHealth / maxHealth;
            var newHealthRatio = (float)newHealth / maxHealth;
            
            _targetHealthBar.fillAmount = healthRatio;
            
            var stringBuilder = new StringBuilder();
            
            if (damage > 0)
            {
                stringBuilder.AppendLine($"Damage: {damage}");
            }
            
            if (healing > 0)
            {
                stringBuilder.AppendLine($"Healing: {healing}");
            }
            
            foreach (var prediction in predictions)
            {
                if (prediction.Key != SkillEffect.TargetDamage && prediction.Key != SkillEffect.TargetHeal)
                {
                    stringBuilder.AppendLine($"{prediction.Key}: {prediction.Value}");
                }
            }
            
            stringBuilder.AppendLine($"Health: {currentHealth} → {newHealth}");
            
            _predictedEffectsText.text = stringBuilder.ToString();
        }
    }
}