using System;
using System.Collections.Generic;
using System.Reflection;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects.Editor
{
    public class EffectModelPropertyCache
    {
        private static readonly Dictionary<Type, List<PropertyInfo>> TypeProperties = new();
        private static readonly Dictionary<Type, List<PropertyInfo>> InterfaceProperties = new();
        
        public static List<PropertyInfo> GetPropertiesForModel(IEffectModel model)
        {
            var modelType = model.GetType();
            if (TypeProperties.TryGetValue(modelType, out var properties))
            {
                return properties;
            }
            
            var modelProperties = new List<PropertyInfo>();
            
            if (model is IAmountEffect)
            {
                modelProperties.AddRange(GetPropertiesForInterface(typeof(IAmountEffect)));
            }
            
            if (model is IDurationEffect)
            {
                modelProperties.AddRange(GetPropertiesForInterface(typeof(IDurationEffect)));
            }
            
            if (model is ICriticalEffect)
            {
                modelProperties.AddRange(GetPropertiesForInterface(typeof(ICriticalEffect)));
            }
            
            var additionalProperties = modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var property in additionalProperties)
            {
                if (property.CanRead && property.CanWrite && property.Name != "Type")
                {
                    modelProperties.Add(property);
                }
            }
            
            TypeProperties[modelType] = modelProperties;
            return modelProperties;
        }
        
        private static List<PropertyInfo> GetPropertiesForInterface(Type interfaceType)
        {
            if (InterfaceProperties.TryGetValue(interfaceType, out var properties))
            {
                return properties;
            }
            
            var interfaceProperties = new List<PropertyInfo>();
            var allProperties = interfaceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            
            foreach (var property in allProperties)
            {
                if (property.CanRead && property.CanWrite)
                {
                    interfaceProperties.Add(property);
                }
            }
            
            InterfaceProperties[interfaceType] = interfaceProperties;
            return interfaceProperties;
        }
    }
}