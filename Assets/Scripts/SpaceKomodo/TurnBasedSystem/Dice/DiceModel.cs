using System;
using R3;
using Random = UnityEngine.Random;

namespace SpaceKomodo.TurnBasedSystem.Dice
{
    [Serializable]
    public class DiceModel
    {
        public ReactiveProperty<int> Value;
        
        public DiceModel(int value, DisposableBag disposableBag)
        {
            Value = new ReactiveProperty<int>(value).AddTo(ref disposableBag);
        }

        public void Roll()
        {
            Value.Value = Random.Range(1, 7);
        }
    }
}