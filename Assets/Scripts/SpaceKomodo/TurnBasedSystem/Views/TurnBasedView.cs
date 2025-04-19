using MessagePipe;
using R3;
using SpaceKomodo.TurnBasedSystem.Core;
using SpaceKomodo.TurnBasedSystem.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace SpaceKomodo.TurnBasedSystem.Views
{
    public class TurnBasedView : MonoBehaviour
    {
        public Transform CharacterTurnViewParentTransform;
        public Transform CurrentTurnSkillViewParentTransform;
        public Transform HeroGridParentTransform;
        public Transform EnemyGridParentTransform;

        public TMP_Text roundText;
        public TMP_Text turnText;
        public Button nextTurnButton;
        
        public TMP_Text phaseText;
        
        public Button ExecuteButton;
        public Button CancelButton;
        
        [Inject] private readonly TurnBasedModel turnBasedModel;
        [Inject] private readonly IPublisher<NextTurnButtonClickedEvent> _nextTurnButtonClickedPublisher;
        [Inject] private readonly IPublisher<ExecuteCommandEvent> _executeCommandPublisher;
        [Inject] private readonly IPublisher<CancelCommandEvent> _cancelCommandPublisher;
        [Inject] private readonly ISubscriber<TargetSelectedEvent> _targetSelectedSubscriber;

        private void Start()
        {
            nextTurnButton.onClick.AddListener(OnNextTurnButtonClicked);
            ExecuteButton.onClick.AddListener(OnExecuteButtonClicked);
            CancelButton.onClick.AddListener(OnCancelButtonClicked);
            
            _targetSelectedSubscriber.Subscribe(OnTargetSelected).AddTo(gameObject);

            turnBasedModel.CurrentRound.Subscribe(OnCurrentRoundChanged);
            turnBasedModel.CurrentTurn.Subscribe(OnCurrentTurnChanged);

            void OnNextTurnButtonClicked()
            {
                _nextTurnButtonClickedPublisher.Publish(new NextTurnButtonClickedEvent());
            }

            void OnExecuteButtonClicked()
            {
                _executeCommandPublisher.Publish(new ExecuteCommandEvent());
                ExecuteButton.gameObject.SetActive(false);
                CancelButton.gameObject.SetActive(false);
            }

            void OnCancelButtonClicked()
            {
                _cancelCommandPublisher.Publish(new CancelCommandEvent());
                ExecuteButton.gameObject.SetActive(false);
                CancelButton.gameObject.SetActive(false);
            }
            
            void OnTargetSelected(TargetSelectedEvent evt)
            {
                ExecuteButton.gameObject.SetActive(true);
                CancelButton.gameObject.SetActive(true);
            }

            void OnCurrentRoundChanged(int currentRound)
            {
                roundText.text = $"Round {currentRound}";
            }

            void OnCurrentTurnChanged(int currentTurn)
            {
                turnText.text = $"Turn {currentTurn}";
            }
        }
    }
}