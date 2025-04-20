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
        
        public TMP_Text SkillDetailsNameText;
        
        [Inject] private readonly TurnBasedModel turnBasedModel;
        [Inject] private readonly IPublisher<NextTurnButtonClickedEvent> _nextTurnButtonClickedPublisher;
        [Inject] private readonly IPublisher<ExecuteCommandEvent> _executeCommandPublisher;
        [Inject] private readonly IPublisher<CancelCommandEvent> _cancelCommandPublisher;
        [Inject] private readonly ISubscriber<SkillClickedEvent> _skillClickedSubscriber;

        private void Start()
        {
            nextTurnButton.onClick.AddListener(OnNextTurnButtonClicked);
            ExecuteButton.onClick.AddListener(OnExecuteButtonClicked);
            CancelButton.onClick.AddListener(OnCancelButtonClicked);
            
            turnBasedModel.CurrentRound.Subscribe(OnCurrentRoundChanged);
            turnBasedModel.CurrentTurn.Subscribe(OnCurrentTurnChanged);
            turnBasedModel.CurrentPhase.Subscribe(OnCurrentPhaseChanged);
            
            _skillClickedSubscriber.Subscribe(OnSkillClicked);

            void OnNextTurnButtonClicked()
            {
                _nextTurnButtonClickedPublisher.Publish(new NextTurnButtonClickedEvent());
            }

            void OnExecuteButtonClicked()
            {
                _executeCommandPublisher.Publish(new ExecuteCommandEvent());
            }

            void OnCancelButtonClicked()
            {
                _cancelCommandPublisher.Publish(new CancelCommandEvent());
            }

            void OnCurrentRoundChanged(int currentRound)
            {
                roundText.text = $"Round {currentRound}";
            }

            void OnCurrentTurnChanged(int currentTurn)
            {
                turnText.text = $"Turn {currentTurn}";
            }

            void OnCurrentPhaseChanged(TurnPhase currentPhase)
            {
                phaseText.text = $"Phase {currentPhase}";
                
                SkillDetailsNameText.gameObject.SetActive(CanShowSkillDetails());
                CancelButton.gameObject.SetActive(CanShowCancelButton());
                ExecuteButton.gameObject.SetActive(CanShowExecuteButton());
                
                bool CanShowSkillDetails()
                {
                    return currentPhase is TurnPhase.SelectSkill or TurnPhase.SelectTarget or TurnPhase.Confirmation;
                }
                
                bool CanShowCancelButton()
                {
                    return currentPhase is TurnPhase.SelectSkill or TurnPhase.SelectTarget or TurnPhase.Confirmation;
                }

                bool CanShowExecuteButton()
                {
                    return currentPhase is TurnPhase.Confirmation;
                }
            }
            
            void OnSkillClicked(SkillClickedEvent evt)
            {
                SkillDetailsNameText.text = evt.Skill.Skill.ToString();
            }
        }
    }
}