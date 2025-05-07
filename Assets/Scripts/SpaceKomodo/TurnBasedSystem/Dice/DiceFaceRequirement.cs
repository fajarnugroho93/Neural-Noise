using System;
using NaughtyAttributes;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Dice
{
    [Serializable]
    public class DiceFaceRequirement : ICloneable
    {
        public DiceFaceInputType InputType;
        [Range(1, 6)]
        public int Value;
        [MinMaxSlider(1, 6)]
        public Vector2Int Range;

        public bool Validate(DiceFaceModel diceFaceModel)
        {
            return InputType switch
            {
                DiceFaceInputType.None => false,
                DiceFaceInputType.Any => true,
                DiceFaceInputType.Single => diceFaceModel.Value == Value,
                DiceFaceInputType.Range => diceFaceModel.Value >= Range.x && diceFaceModel.Value <= Range.y,
                DiceFaceInputType.Even => diceFaceModel.Value % 2 == 0,
                DiceFaceInputType.Odd => diceFaceModel.Value % 2 != 0,
                _ => false
            };
        }

        public override string ToString()
        {
            return InputType switch
            {
                DiceFaceInputType.None => "None",
                DiceFaceInputType.Any => "Any",
                DiceFaceInputType.Single => $"{Value}",
                DiceFaceInputType.Range => $"{Range.x}-{Range.y}",
                DiceFaceInputType.Even => "Even",
                DiceFaceInputType.Odd => "Odd",
                _ => "None"
            };
        }

        public object Clone()
        {
            return new DiceFaceRequirement
            {
                InputType = InputType,
                Value = Value,
                Range = Range
            };
        }
    }
}