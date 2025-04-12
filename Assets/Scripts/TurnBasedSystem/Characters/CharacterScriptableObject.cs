using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using NaughtyAttributes;
#endif

namespace TurnBasedSystem.Characters
{
    [CreateAssetMenu(fileName = "New Character", menuName = "TurnBasedSystem/Character", order = -10000)]
    public class CharacterScriptableObject : ScriptableObject
    {
        public CharacterModel CharacterModel;
        
#if UNITY_EDITOR
        [Button]
        public void DoRenameAsset()
        {
            RenameAsset();
            AssetDatabase.SaveAssets();
        }

        public void RenameAsset()
        {
            var assetPath = AssetDatabase.GetAssetPath(this);
            if (string.IsNullOrEmpty(assetPath))
                return;
                
            var newName = GetAssetName();
            
            AssetDatabase.RenameAsset(assetPath, newName);
            EditorUtility.SetDirty(this);
        }

        public string GetAssetName()
        {
            return $"{(int)CharacterModel.Character}-{CharacterModel.Character}";
        }
#endif
    }
}