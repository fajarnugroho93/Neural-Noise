using SpaceKomodo.TurnBasedSystem.Characters;
using SpaceKomodo.TurnBasedSystem.Characters.Skills;
using SpaceKomodo.TurnBasedSystem.Commands;

namespace SpaceKomodo.TurnBasedSystem.Commands
{
    public class SkillCommand : TurnCommand
    {
        private readonly CharacterModel _source;
        private readonly SkillModel _skill;
        private readonly CharacterModel _target;
        private readonly SkillExecutor _skillExecutor;
        
        private int _targetOriginalHealth;
        private int _targetOriginalShield;
        
        public SkillCommand(
            CharacterModel source, 
            SkillModel skill, 
            CharacterModel target,
            SkillExecutor skillExecutor)
        {
            _source = source;
            _skill = skill;
            _target = target;
            _skillExecutor = skillExecutor;
        }
        
        public override bool CanExecute()
        {
            return _source != null && _skill != null && _target != null;
        }
        
        public override void Execute()
        {
            if (!CanExecute()) return;
            
            _targetOriginalHealth = _target.CurrentHealth.Value;
            _targetOriginalShield = _target.CurrentShield.Value;
            
            _skillExecutor.ExecuteSkill(_source, _target, _skill);
        }
        
        public override void Undo()
        {
            _target.CurrentHealth.Value = _targetOriginalHealth;
            _target.CurrentShield.Value = _targetOriginalShield;
        }
        
        public override string GetDescription()
        {
            return $"{_source.Character} uses {_skill.Skill} on {_target.Character}";
        }
    }
}