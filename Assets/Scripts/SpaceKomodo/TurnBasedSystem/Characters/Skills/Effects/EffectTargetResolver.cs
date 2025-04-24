using System.Collections.Generic;
using System.Linq;
using SpaceKomodo.TurnBasedSystem.Core;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects
{
    public class EffectTargetResolver : IEffectTargetResolver
    {
        private readonly BattleModel _battleModel;

        public EffectTargetResolver(BattleModel battleModel)
        {
            _battleModel = battleModel;
        }

        public List<CharacterModel> ResolveTargets(CharacterModel source, CharacterModel primaryTarget, RelativeTarget targeting)
        {
            var targets = new List<CharacterModel>();

            switch (targeting)
            {
                case RelativeTarget.Primary:
                    targets.Add(primaryTarget);
                    break;

                case RelativeTarget.Self:
                    targets.Add(source);
                    break;

                case RelativeTarget.AllAllies:
                    targets.AddRange(_battleModel.GetAllAllies(source));
                    break;

                case RelativeTarget.OtherAllies:
                    targets.AddRange(_battleModel.GetAllAllies(source).Where(ally => ally != source));
                    break;

                case RelativeTarget.AllEnemies:
                    targets.AddRange(_battleModel.GetAllEnemies(source));
                    break;

                case RelativeTarget.AdjacentTargets:
                    targets.AddRange(_battleModel.GetAdjacentCharacters(primaryTarget));
                    break;

                case RelativeTarget.All:
                    targets.AddRange(_battleModel.GetAllCharacters());
                    break;
            }

            return targets;
        }
    }
}