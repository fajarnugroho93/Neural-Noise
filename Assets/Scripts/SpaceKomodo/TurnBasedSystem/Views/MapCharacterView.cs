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
        [SerializeField] private GameObject selectionIndicator;
        [SerializeField] private GameObject activeIndicator;
        
        private MapCharacterModel _model;
        private DisposableBag _disposableBag;
        private IPublisher<TargetClickedEvent> _targetClickedPublisher;
        
        [Inject]
        public void Construct(IPublisher<TargetClickedEvent> targetClickedPublisher)
        {
            _targetClickedPublisher = targetClickedPublisher;
        }
        
        public void Initialize(MapCharacterModel model)
        {
            _model = model;
            name = $"MapCharacterView_{model.CharacterModel.Character}";
            
            portrait.sprite = model.CharacterModel.Portrait;
            
            _disposableBag.Dispose();
            _disposableBag = new DisposableBag();
            
            model.CharacterModel.IsCurrentTurn
                .Subscribe(isCurrentTurn => 
                {
                    if (activeIndicator != null)
                    {
                        activeIndicator.SetActive(isCurrentTurn);
                    }
                })
                .AddTo(ref _disposableBag);
        }
        
        public void SetSelected(bool isSelected)
        {
            if (selectionIndicator != null)
            {
                selectionIndicator.SetActive(isSelected);
            }
        }
        
        private void OnDestroy()
        {
            _disposableBag.Dispose();
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