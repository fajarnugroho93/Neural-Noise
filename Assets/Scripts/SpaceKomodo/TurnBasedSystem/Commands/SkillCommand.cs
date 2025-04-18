using SpaceKomodo.TurnBasedSystem.Characters;
using SpaceKomodo.TurnBasedSystem.Characters.Skills;

namespace SpaceKomodo.TurnBasedSystem.Commands
{
    public class SkillCommand : TurnCommand
    {
        private readonly CharacterModel _source;
        private readonly SkillModel _skill;
        private readonly CharacterModel _target;
        private readonly IEffectExecutor _effectExecutor;
        
        private int _targetOriginalHealth;
        
        public SkillCommand(
            CharacterModel source, 
            SkillModel skill, 
            CharacterModel target,
            IEffectExecutor effectExecutor)
        {
            _source = source;
            _skill = skill;
            _target = target;
            _effectExecutor = effectExecutor;
        }
        
        public override bool CanExecute()
        {
            return _source != null && _skill != null && _target != null;
        }
        
        public override void Execute()
        {
            if (!CanExecute()) return;
            
            _targetOriginalHealth = _target.CurrentHealth.Value;
            
            foreach (var effect in _skill.Effects)
            {
                _effectExecutor.ExecuteEffect(_source, _target, effect);
            }
        }
        
        public override void Undo()
        {
            _target.CurrentHealth.Value = _targetOriginalHealth;
        }
        
        public override string GetDescription()
        {
            return $"{_source.Character} uses {_skill.Skill} on {_target.Character}";
        }
    }
}