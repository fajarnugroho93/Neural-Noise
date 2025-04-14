using R3;
using SpaceKomodo.Extensions;
using SpaceKomodo.TurnBasedSystem.Characters;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceKomodo.TurnBasedSystem.Views
{
    public class CurrentTurnSelectedCharacterDetailsView : MonoBehaviour
    {
        public TMP_Text Name;
        public Image Portrait;
        public Slider HealthSlider;
        public TMP_Text HealthText;
        public TMP_Text SpeedText;

        private DisposableBag _disposableBag;
        
        public void SetCharacterModel(CharacterModel characterModel)
        {
            _disposableBag.Dispose();
            
            Name.text = characterModel.Character.ToString();
            Portrait.sprite = characterModel.Portrait;
            
            characterModel.CurrentMaxHealth
                .CombineLatest(characterModel.CurrentHealth)
                .Subscribe(OnHealthChanged)
                .AddTo(ref _disposableBag);

            void OnHealthChanged((int currentMaxHealth, int currentHealth) values)
            {
                HealthText.text = $"{values.currentHealth}/{values.currentMaxHealth}";
                HealthSlider.value = (float) values.currentHealth / values.currentMaxHealth;
            }
            
            characterModel.BaseSpeed
                .CombineLatest(characterModel.TurnSpeed)
                .Subscribe(OnSpeedChanged)
                .AddTo(ref _disposableBag);

            void OnSpeedChanged((int baseSpeed, int turnSpeed) values)
            {
                var speedBonus = values.turnSpeed == 0 ? "" : $"+ {values.turnSpeed}";
                SpeedText.text = $"Speed: {values.baseSpeed} {speedBonus}";
            }
        }
    }
}