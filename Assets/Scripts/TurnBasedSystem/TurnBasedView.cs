using MessagePipe;
using R3;
using TMPro;
using TurnBasedSystem.Characters;
using TurnBasedSystem.Events;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace TurnBasedSystem
{
    public class TurnBasedView : MonoBehaviour
    {
        public CharacterTurnView characterTurnViewPrefab;
        public Transform characterTurnParentViewTransform;

        public TMP_Text roundText;
        public TMP_Text turnText;
        public Button nextTurnButton;
        
        [Inject] private readonly TurnBasedModel turnBasedModel;
        [Inject] private readonly IPublisher<NextTurnButtonClickedEvent> nextTurnButtonClickedPublisher;

        private void Start()
        {
            nextTurnButton.onClick.AddListener(OnNextTurnButtonClicked);

            void OnNextTurnButtonClicked()
            {
                nextTurnButtonClickedPublisher.Publish(new NextTurnButtonClickedEvent());
            }

            turnBasedModel.CurrentRound.Subscribe(OnCurrentRoundChanged);

            void OnCurrentRoundChanged(int currentRound)
            {
                roundText.text = $"Round {currentRound}";
            }

            turnBasedModel.CurrentTurn.Subscribe(OnCurrentTurnChanged);

            void OnCurrentTurnChanged(int currentTurn)
            {
                turnText.text = $"Turn {currentTurn}";
            }
        }
    }
}