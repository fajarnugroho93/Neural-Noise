using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using SpaceKomodo.Extensions;
using SpaceKomodo.TurnBasedSystem.Characters.Skills;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Characters
{
    [Serializable]
    public class CharacterModel : ICloneable
    {
        public CharacterGroup CharacterGroup;
        public Character Character;
        public Sprite Portrait;
        public int Health;
        public int Speed;
        public List<SkillModel> Skills;

        public readonly ReactiveProperty<int> BaseMaxHealth;
        public readonly ReactiveProperty<int> CurrentMaxHealth;
        
        public readonly ReactiveProperty<int> CurrentHealth;
        public ReactiveProperty<int> CurrentShield;
        
        public readonly ReactiveProperty<int> BaseSpeed;
        public readonly ReactiveProperty<int> TurnSpeed;
        public readonly ReactiveProperty<int> CurrentSpeed;
        
        public readonly ReactiveProperty<int> TurnOrder;
        public readonly ReactiveProperty<bool> IsCurrentTurn;
        public readonly ReactiveProperty<bool> IsTargetable;
        public readonly ReactiveProperty<bool> IsTargeted;

        public CharacterModel()
        {
            
        }

        public CharacterModel(CharacterModel characterModel)
        {
            CharacterGroup = characterModel.CharacterGroup;
            Character = characterModel.Character;
            Portrait = characterModel.Portrait;
            Health = characterModel.Health;
            Speed = characterModel.Speed;
            Skills = characterModel.Skills?
                .Select(skill => (SkillModel)skill.Clone()).ToList() ?? new List<SkillModel>();

            BaseMaxHealth = new ReactiveProperty<int>(Health);
            CurrentMaxHealth = new ReactiveProperty<int>(Health);
            
            CurrentHealth = new ReactiveProperty<int>(Health);
            CurrentShield = new ReactiveProperty<int>(0);
            
            BaseSpeed = new ReactiveProperty<int>(Speed);
            TurnSpeed = new ReactiveProperty<int>(0);
            CurrentSpeed = new ReactiveProperty<int>(0);

            BaseSpeed.CombineLatest(TurnSpeed)
                .Subscribe(OnSpeedChanged);

            void OnSpeedChanged((int baseSpeed, int randomSpeed) values)
            {
                CurrentSpeed.Value = values.baseSpeed + values.randomSpeed;
            }
            
            TurnOrder = new ReactiveProperty<int>(0);
            IsCurrentTurn = new ReactiveProperty<bool>(false);
            IsTargetable = new ReactiveProperty<bool>(false);
            IsTargeted = new ReactiveProperty<bool>(false);
        }

        public void ResetTarget()
        {
            IsTargetable.Value = false;
            IsTargeted.Value = false;
        }

        public object Clone()
        {
            return new CharacterModel(this);
        }

        public bool IsHero()
        {
            return CharacterGroup == CharacterGroup.Hero;
        }
    }
}