using System;
using TurnBasedSystem.Dice;

namespace TurnBasedSystem.Characters.Skills
{
    [Serializable]
    public class DiceFaceRequirement
    {
        public DiceFaceInputType InputType;
        public int Value;
        public int RangeEnd;

        public bool Validate(DiceFaceModel diceFaceModel)
        {
            switch (InputType)
            {
                case DiceFaceInputType.Single:
                    return diceFaceModel.Value == Value;
                case DiceFaceInputType.Range:
                    return diceFaceModel.Value >= Value && diceFaceModel.Value <= RangeEnd;
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