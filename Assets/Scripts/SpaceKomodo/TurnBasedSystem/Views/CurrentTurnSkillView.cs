using MessagePipe;
using R3;
using SpaceKomodo.TurnBasedSystem.Characters.Skills;
using SpaceKomodo.TurnBasedSystem.Events;
using SpaceKomodo.Utilities;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using DisposableBag = R3.DisposableBag;

namespace SpaceKomodo.TurnBasedSystem.Views
{
    public class CurrentTurnSkillView : MonoBehaviour, IInitializable<SkillModel>
    {
        public Image Portrait;
        public Button Button;
        
        public GameObject SelectableGameObject;
        public GameObject SelectedGameObject;
        public DiceFaceView DiceFaceView;
        
        private SkillModel _skillModel;
        private IPublisher<SkillClickedEvent> _skillClickedPublisher;
        private DisposableBag _disposableBag;
        
        [Inject]
        public void Construct(IPublisher<SkillClickedEvent> skillClickedPublisher)
        {
            _skillClickedPublisher = skillClickedPublisher;
        }
        
        public void Initialize(SkillModel skillModel)
        {
            _skillModel = skillModel;
            Portrait.sprite = skillModel.Portrait;
            
            Button.onClick.AddListener(OnSkillClicked);
            
            _skillModel.IsSelectable
                .Subscribe(OnSelectableChanged)
                .AddTo(ref _disposableBag);
            _skillModel.IsSelected
                .Subscribe(OnSelectedChanged)
                .AddTo(ref _disposableBag);

            DiceFaceView.DiceText.text = skillModel.DiceFaceRequirement.ToString();

            void OnSelectableChanged(bool isSelectable)
            {
                if (SelectableGameObject != null)
                {
                    SelectableGameObject.SetActive(isSelectable);
                }
            }

            void OnSelectedChanged(bool isSelected)
            {
                if (SelectedGameObject != null)
                {
                    SelectedGameObject.SetActive(isSelected);
                }
            }
        }
        
        private void OnSkillClicked()
        {
            _skillClickedPublisher.Publish(new SkillClickedEvent(_skillModel));
        }

        private void OnDestroy()
        {
            _disposableBag.Dispose();
        }
    }
}