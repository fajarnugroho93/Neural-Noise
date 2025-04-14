using R3;
using SpaceKomodo.Extensions;
using TMPro;
using TurnBasedSystem.Characters;
using UnityEngine;
using UnityEngine.UI;

namespace TurnBasedSystem.Views
{
    public class CurrentTurnSelectedCharacterDetailsView : MonoBehaviour
    {
        public TMP_Text Name;
        public Image Portrait;
        public Slider HealthSlider;
        public TMP_Text HealthText;
        public TMP_Text SpeedText;

        public void SetCharacterModel(CharacterModel characterModel)
        {
            Name.text = characterModel.Character.ToString();
            Portrait.sprite = characterModel.Portrait;
            
            characterModel.CurrentMaxHealth
                .CombineLatest(characterModel.CurrentHealth)
                .Subscribe(OnHealthChanged);

            void OnHealthChanged((int currentMaxHealth, int currentHealth) values)
            {
                HealthText.text = $"{values.currentHealth}/{values.currentMaxHealth}";
                HealthSlider.value = (float) values.currentHealth / values.currentMaxHealth;
            }
            
            characterModel.BaseSpeed
                .CombineLatest(characterModel.TurnSpeed)
                .Subscribe(OnSpeedChanged);

            void OnSpeedChanged((int baseSpeed, int turnSpeed) values)
            {
                var speedBonus = values.turnSpeed == 0 ? "" : $"+ {values.turnSpeed}";
                SpeedText.text = $"Speed: {values.baseSpeed} {speedBonus}";
            }
        }
    }
}