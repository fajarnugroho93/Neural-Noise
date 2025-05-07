using MessagePipe;
using R3;
using SpaceKomodo.TurnBasedSystem.Dice;
using SpaceKomodo.TurnBasedSystem.Events;
using SpaceKomodo.Utilities;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using DisposableBag = R3.DisposableBag;

namespace SpaceKomodo.TurnBasedSystem.Views
{
    public class TurnDiceView : MonoBehaviour, IInitializable<DiceModel>
    {
        public Button Button;
        public DiceFaceView DiceFaceView;
        
        private DiceModel _diceModel;
        private IPublisher<TurnDiceClickedEvent> _turnDiceClickedPublisher;
        private DisposableBag _disposableBag;
        
        [Inject]
        public void Construct(IPublisher<TurnDiceClickedEvent> turnDiceClickedPublisher)
        {
            _turnDiceClickedPublisher = turnDiceClickedPublisher;
        }
        
        public void Initialize(DiceModel diceModel)
        {
            _diceModel = diceModel;
            _diceModel.Value.Subscribe(OnDiceModelValueChanged).AddTo(ref _disposableBag);

            Button.onClick.AddListener(OnDiceTurnClicked);

            void OnDiceModelValueChanged(int value)
            {
                DiceFaceView.DiceText.text = value.ToString();
            }
        }
        
        private void OnDiceTurnClicked()
        {
            _turnDiceClickedPublisher.Publish(new TurnDiceClickedEvent(_diceModel));
        }

        private void OnDestroy()
        {
            _disposableBag.Dispose();
        }
    }
}