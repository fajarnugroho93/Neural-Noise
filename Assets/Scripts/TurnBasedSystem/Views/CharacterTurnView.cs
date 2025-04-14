using R3;
using SpaceKomodo.Extensions;
using SpaceKomodo.Utilities;
using TMPro;
using TurnBasedSystem.Characters;
using UnityEngine;
using UnityEngine.UI;

namespace TurnBasedSystem.Views
{
    public class CharacterTurnView : MonoBehaviour, IInitializable<CharacterModel>
    {
        public Image portrait;
        public Slider healthSlider;
        public TMP_Text healthText;
        public TMP_Text speedText;
        public GameObject isHeroSpace;
        public GameObject isEnemySpace;
        public GameObject heroTurnMarker;
        public GameObject enemyTurnMarker;
        public HorizontalLayoutGroup characterTurnLayoutGroup;
        
        public Image background;
        public Color heroColor;
        public Color enemyColor;

        public void Initialize(CharacterModel model)
        {
            name = $"CharacterTurnView_{model.Character}";
            
            portrait.sprite = model.Portrait;

            model.CurrentMaxHealth
                .CombineLatest(model.CurrentHealth)
                .Subscribe(OnHealthChanged);

            void OnHealthChanged((int currentMaxHealth, int currentHealth) values)
            {
                healthText.text = $"{values.currentHealth}/{values.currentMaxHealth}";
                healthSlider.value = (float) values.currentHealth / values.currentMaxHealth;
            }

            model.BaseSpeed
                .CombineLatest(model.TurnSpeed)
                .Subscribe(OnSpeedChanged);

            void OnSpeedChanged((int baseSpeed, int turnSpeed) values)
            {
                var speedBonus = values.turnSpeed == 0 ? "" : $"+ {values.turnSpeed}";
                speedText.text = $"Speed: {values.baseSpeed} {speedBonus}";
            }

            characterTurnLayoutGroup.reverseArrangement = !model.IsHero();
            isHeroSpace.SetActive(model.IsHero());
            isEnemySpace.SetActive(!model.IsHero());
            background.color = model.IsHero() ? heroColor : enemyColor;

            model.IsCurrentTurn.Subscribe(OnIsCurrentTurnChanged);

            void OnIsCurrentTurnChanged(bool isCurrentTurn)
            {
                heroTurnMarker.SetActive(isCurrentTurn && model.IsHero());
                enemyTurnMarker.SetActive(isCurrentTurn && !model.IsHero());
            }
        }
    }
}