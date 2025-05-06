using UnityEditor;
using UnityEngine;

namespace SpaceKomodo.TurnBasedSystem.Dice.Editor
{
    [CustomPropertyDrawer(typeof(DiceFaceRequirement))]
    public class DiceFaceRequirementPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var inputTypeProp = property.FindPropertyRelative("InputType");
            var inputType = (DiceFaceInputType)inputTypeProp.enumValueIndex;
            
            var height = EditorGUIUtility.singleLineHeight;
            
            switch (inputType)
            {
                case DiceFaceInputType.Single:
                    var valueProp = property.FindPropertyRelative("Value");
                    height += EditorGUIUtility.standardVerticalSpacing;
                    height += EditorGUI.GetPropertyHeight(valueProp, true);
                    break;
                case DiceFaceInputType.Range:
                    var rangeProp = property.FindPropertyRelative("Range");
                    height += EditorGUI.GetPropertyHeight(rangeProp, false) / 2;
                    break;
            }
            
            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            var rect = position;
            rect.height = EditorGUIUtility.singleLineHeight;
            
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("InputType"), label);
            
            var inputTypeProp = property.FindPropertyRelative("InputType");
            var inputType = (DiceFaceInputType)inputTypeProp.enumValueIndex;
            
            rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            
            switch (inputType)
            {
                case DiceFaceInputType.Single:
                    var valueProp = property.FindPropertyRelative("Value");
                    EditorGUI.PropertyField(rect, valueProp, new GUIContent("Value"));
                    break;
                case DiceFaceInputType.Range:
                    var rangeProp = property.FindPropertyRelative("Range");
                    EditorGUI.PropertyField(rect, rangeProp, new GUIContent("Range"));
                    break;
            }
            
            EditorGUI.EndProperty();
        }
    }
}