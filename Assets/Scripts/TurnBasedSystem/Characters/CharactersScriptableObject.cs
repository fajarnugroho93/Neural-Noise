using UnityEngine;

#if UNITY_EDITOR
using System.Linq;
using NaughtyAttributes;
using SpaceKomodo.Editor;
#endif

namespace TurnBasedSystem.Characters
{
    [CreateAssetMenu(fileName = "New Characters", menuName = "TurnBasedSystem/Characters", order = -10000)]
    public class CharactersScriptableObject : ScriptableObject
    {
        public CharacterScriptableObject[] Heroes;
        public CharacterScriptableObject[] Enemies;
        
#if UNITY_EDITOR
        [Button]
        public void DoFetchAssets()
        {
            var characterScriptableObjects = EditorHelpers.GetAllInstances<CharacterScriptableObject>();
            foreach (var characterScriptableObject in characterScriptableObjects)
            {
                characterScriptableObject.RenameAsset();
            }

            Heroes = characterScriptableObjects
                .Where(characterScriptableObject => characterScriptableObject.CharacterModel.CharacterGroup == CharacterGroup.Hero)
                .OrderBy(characterScriptableObject => characterScriptableObject.CharacterModel.Character)
                .ToArray();

            Enemies = characterScriptableObjects
                .Where(characterScriptableObject => characterScriptableObject.CharacterModel.CharacterGroup == CharacterGroup.Enemy)
                .OrderBy(characterScriptableObject => characterScriptableObject.CharacterModel.Character)
                .ToArray();
            
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
        }
#endif
    }
}