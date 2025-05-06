using MessagePipe;
using R3;
using SpaceKomodo.TurnBasedSystem.Events;
using SpaceKomodo.TurnBasedSystem.Maps;
using SpaceKomodo.Utilities;
using UnityEngine;
using VContainer;
using DisposableBag = R3.DisposableBag;

namespace SpaceKomodo.TurnBasedSystem.Views
{
    public class MapCharacterView : MonoBehaviour, IInitializable<MapCharacterModel>
    {
        [SerializeField] private SpriteRenderer portrait;
        [SerializeField] private GameObject activeIndicator;
        [SerializeField] private GameObject targetableIndicator;
        [SerializeField] private GameObject targetedIndicator;
        
        private MapCharacterModel _model;
        private DisposableBag _disposableBag;
        private IPublisher<TargetClickedEvent> _targetClickedPublisher;
        
        [Inject]
        public void Construct(IPublisher<TargetClickedEvent> targetClickedPublisher,
            DisposableBag disposableBag)
        {
            _targetClickedPublisher = targetClickedPublisher;
            _disposableBag = disposableBag;
        }
        
        public void Initialize(MapCharacterModel model)
        {
            _model = model;
            name = $"MapCharacterView_{model.CharacterModel.Character}";
            
            portrait.sprite = model.CharacterModel.Portrait;
            
            model.CharacterModel.IsCurrentTurn
                .Subscribe(OnCurrentTurnChanged)
                .AddTo(ref _disposableBag);
            model.CharacterModel.IsTargetable
                .Subscribe(OnTargetableChanged)
                .AddTo(ref _disposableBag);
            model.CharacterModel.IsTargeted
                .Subscribe(OnTargetedChanged)
                .AddTo(ref _disposableBag);

            void OnCurrentTurnChanged(bool isCurrentTurn)
            {
                if (activeIndicator != null)
                {
                    activeIndicator.SetActive(isCurrentTurn);
                }
            }

            void OnTargetableChanged(bool isTargetable)
            {
                if (targetableIndicator != null)
                {
                    targetableIndicator.SetActive(isTargetable);
                }
            }

            void OnTargetedChanged(bool isTargeted)
            {
                if (targetedIndicator != null)
                {
                    targetedIndicator.SetActive(isTargeted);
                }
            }
        }
        
        private void OnMouseDown()
        {
            if (_model != null)
            {
                _targetClickedPublisher.Publish(new TargetClickedEvent(_model.CharacterModel));
            }
        }
    }
}