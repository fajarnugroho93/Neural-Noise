using System;
using NaughtyAttributes;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Dice
{
    [Serializable]
    public class DiceFaceRequirement
    {
        public DiceFaceInputType InputType;
        [Range(1, 6)]
        public int Value;
        [MinMaxSlider(1, 6)]
        public Vector2Int Range;

        public bool Validate(DiceFaceModel diceFaceModel)
        {
            switch (InputType)
            {
                case DiceFaceInputType.None:
                    return false;
                case DiceFaceInputType.Any:
                    return true;
                case DiceFaceInputType.Single:
                    return diceFaceModel.Value == Value;
                case DiceFaceInputType.Range:
                    return diceFaceModel.Value >= Range.x && diceFaceModel.Value <= Range.y;
                case DiceFaceInputType.Even:
                    return diceFaceModel.Value % 2 == 0;
                case DiceFaceInputType.Odd:
                    return diceFaceModel.Value % 2 != 0;
                default:
                    return false;
            }
        }
    }
}