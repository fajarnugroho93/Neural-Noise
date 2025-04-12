using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpaceKomodo.Editor
{
    public static class EditorHelpers
    {
        public static List<T> GetAllInstances<T>() where T : ScriptableObject
        {
            return UnityEditor.AssetDatabase.FindAssets($"t:{typeof(T).Name}").ToList()
                .Select(UnityEditor.AssetDatabase.GUIDToAssetPath)
                .Select(UnityEditor.AssetDatabase.LoadAssetAtPath<T>)
                .ToList();
        }
    }
}