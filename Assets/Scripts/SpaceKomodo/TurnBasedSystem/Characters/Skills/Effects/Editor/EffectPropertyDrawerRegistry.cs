using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects.Editor
{
    public interface IEffectPropertyDrawer
    {
        bool CanHandleProperty(PropertyInfo property);
        void DrawProperty(Rect position, PropertyInfo property, object target, string label);
        float GetPropertyHeight(PropertyInfo property, object target);
    }
    
    public static class EffectPropertyDrawerRegistry
    {
        private static readonly List<IEffectPropertyDrawer> Drawers = new()
        {
            new IntPropertyDrawer(),
            new FloatPropertyDrawer(),
            new BoolPropertyDrawer(),
            new EnumPropertyDrawer(),
            new StringPropertyDrawer()
        };
        
        public static IEffectPropertyDrawer GetDrawerForProperty(PropertyInfo property)
        {
            foreach (var drawer in Drawers)
            {
                if (drawer.CanHandleProperty(property))
                {
                    return drawer;
                }
            }
            
            return new DefaultPropertyDrawer();
        }
        
        private class IntPropertyDrawer : IEffectPropertyDrawer
        {
            public bool CanHandleProperty(PropertyInfo property)
            {
                return property.PropertyType == typeof(int);
            }
            
            public void DrawProperty(Rect position, PropertyInfo property, object target, string label)
            {
                var value = (int)property.GetValue(target);
                var newValue = UnityEditor.EditorGUI.IntField(position, label, value);
                
                if (newValue != value)
                {
                    property.SetValue(target, newValue);
                }
            }
            
            public float GetPropertyHeight(PropertyInfo property, object target)
            {
                return UnityEditor.EditorGUIUtility.singleLineHeight;
            }
        }
        
        private class FloatPropertyDrawer : IEffectPropertyDrawer
        {
            public bool CanHandleProperty(PropertyInfo property)
            {
                return property.PropertyType == typeof(float);
            }
            
            public void DrawProperty(Rect position, PropertyInfo property, object target, string label)
            {
                var value = (float)property.GetValue(target);
                
                var rangeAttr = property.GetCustomAttribute<RangeAttribute>();
                float newValue;
                
                if (rangeAttr != null)
                {
                    newValue = UnityEditor.EditorGUI.Slider(position, label, value, rangeAttr.min, rangeAttr.max);
                }
                else
                {
                    newValue = UnityEditor.EditorGUI.FloatField(position, label, value);
                }
                
                if (Math.Abs(newValue - value) > 0.0001f)
                {
                    property.SetValue(target, newValue);
                }
            }
            
            public float GetPropertyHeight(PropertyInfo property, object target)
            {
                return UnityEditor.EditorGUIUtility.singleLineHeight;
            }
        }
        
        private class BoolPropertyDrawer : IEffectPropertyDrawer
        {
            public bool CanHandleProperty(PropertyInfo property)
            {
                return property.PropertyType == typeof(bool);
            }
            
            public void DrawProperty(Rect position, PropertyInfo property, object target, string label)
            {
                var value = (bool)property.GetValue(target);
                var newValue = UnityEditor.EditorGUI.Toggle(position, label, value);
                
                if (newValue != value)
                {
                    property.SetValue(target, newValue);
                }
            }
            
            public float GetPropertyHeight(PropertyInfo property, object target)
            {
                return UnityEditor.EditorGUIUtility.singleLineHeight;
            }
        }
        
        private class EnumPropertyDrawer : IEffectPropertyDrawer
        {
            public bool CanHandleProperty(PropertyInfo property)
            {
                return property.PropertyType.IsEnum;
            }
            
            public void DrawProperty(Rect position, PropertyInfo property, object target, string label)
            {
                var value = property.GetValue(target);
                var newValue = UnityEditor.EditorGUI.EnumPopup(position, label, (Enum)value);
                
                if (!newValue.Equals(value))
                {
                    property.SetValue(target, newValue);
                }
            }
            
            public float GetPropertyHeight(PropertyInfo property, object target)
            {
                return UnityEditor.EditorGUIUtility.singleLineHeight;
            }
        }
        
        private class StringPropertyDrawer : IEffectPropertyDrawer
        {
            public bool CanHandleProperty(PropertyInfo property)
            {
                return property.PropertyType == typeof(string);
            }
            
            public void DrawProperty(Rect position, PropertyInfo property, object target, string label)
            {
                var value = (string)property.GetValue(target);
                var newValue = UnityEditor.EditorGUI.TextField(position, label, value);
                
                if (newValue != value)
                {
                    property.SetValue(target, newValue);
                }
            }
            
            public float GetPropertyHeight(PropertyInfo property, object target)
            {
                return UnityEditor.EditorGUIUtility.singleLineHeight;
            }
        }
        
        private class DefaultPropertyDrawer : IEffectPropertyDrawer
        {
            public bool CanHandleProperty(PropertyInfo property)
            {
                return true;
            }
            
            public void DrawProperty(Rect position, PropertyInfo property, object target, string label)
            {
                UnityEditor.EditorGUI.LabelField(position, label, "Unsupported property type");
            }
            
            public float GetPropertyHeight(PropertyInfo property, object target)
            {
                return UnityEditor.EditorGUIUtility.singleLineHeight;
            }
        }
    }
}