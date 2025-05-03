using UnityEditor;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Characters.Skills.Effects.Editor
{
    [CustomPropertyDrawer(typeof(IEffectModel), true)]
    public class EffectModelPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
        
            var referenceProperty = property.FindPropertyRelative("$type");
            var managedReferenceValue = property.managedReferenceValue;
        
            if (managedReferenceValue == null)
            {
                position.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(position, label.text, "Null");
            }
            else
            {
                position.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(position, label.text, managedReferenceValue.GetType().Name);
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            
                var children = property.GetEnumerator();
                while (children.MoveNext())
                {
                    var child = children.Current as SerializedProperty;
                    if (child.name == "$type") continue;
                    EditorGUI.PropertyField(position, child, true);
                    position.y += EditorGUI.GetPropertyHeight(child, true) + EditorGUIUtility.standardVerticalSpacing;
                }
            }
        
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;
        
            if (property.managedReferenceValue != null)
            {
                var children = property.GetEnumerator();
                while (children.MoveNext())
                {
                    var child = children.Current as SerializedProperty;
                    if (child.name == "$type") continue;
                    height += EditorGUI.GetPropertyHeight(child, true) + EditorGUIUtility.standardVerticalSpacing;
                }
            }
        
            return height;
        }
    }
}