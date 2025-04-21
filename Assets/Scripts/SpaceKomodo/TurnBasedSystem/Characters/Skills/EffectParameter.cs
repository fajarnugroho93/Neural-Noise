using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills
{
    [Serializable]
    public class EffectParameter
    {
        public string Name;
        public int IntValue;
        public float FloatValue;
        public bool BoolValue;
        public ParameterType Type;
    }

    public enum ParameterType
    {
        Int,
        Float,
        Bool
    }

    [Serializable]
    public class EffectParameters
    {
        [SerializeField] private List<EffectParameter> parameters = new List<EffectParameter>();
        
        public int GetInt(string name, int defaultValue = 0)
        {
            foreach (var param in parameters)
            {
                if (param.Name == name && param.Type == ParameterType.Int)
                {
                    return param.IntValue;
                }
            }
            return defaultValue;
        }
        
        public float GetFloat(string name, float defaultValue = 0f)
        {
            foreach (var param in parameters)
            {
                if (param.Name == name && param.Type == ParameterType.Float)
                {
                    return param.FloatValue;
                }
            }
            return defaultValue;
        }
        
        public bool GetBool(string name, bool defaultValue = false)
        {
            foreach (var param in parameters)
            {
                if (param.Name == name && param.Type == ParameterType.Bool)
                {
                    return param.BoolValue;
                }
            }
            return defaultValue;
        }
        
        public void SetInt(string name, int value)
        {
            for (int i = 0; i < parameters.Count; i++)
            {
                if (parameters[i].Name == name && parameters[i].Type == ParameterType.Int)
                {
                    parameters[i].IntValue = value;
                    return;
                }
            }
            
            parameters.Add(new EffectParameter
            {
                Name = name,
                IntValue = value,
                Type = ParameterType.Int
            });
        }
        
        public void SetFloat(string name, float value)
        {
            for (int i = 0; i < parameters.Count; i++)
            {
                if (parameters[i].Name == name && parameters[i].Type == ParameterType.Float)
                {
                    parameters[i].FloatValue = value;
                    return;
                }
            }
            
            parameters.Add(new EffectParameter
            {
                Name = name,
                FloatValue = value,
                Type = ParameterType.Float
            });
        }
        
        public void SetBool(string name, bool value)
        {
            for (int i = 0; i < parameters.Count; i++)
            {
                if (parameters[i].Name == name && parameters[i].Type == ParameterType.Bool)
                {
                    parameters[i].BoolValue = value;
                    return;
                }
            }
            
            parameters.Add(new EffectParameter
            {
                Name = name,
                BoolValue = value,
                Type = ParameterType.Bool
            });
        }
    }
}