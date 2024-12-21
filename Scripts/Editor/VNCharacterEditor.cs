using KuroNovel.DataNode;
using UnityEditor;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using KuroNovel;
using Unity.VisualScripting;

namespace KuroNovelEdior
{
    public class VNCharacterEditor : EditorWindow
    {
        [MenuItem("Kuro Novel/VN Characters Editor", false, 0)]
        public static void ShowWindow()
        {
            GetWindow<VNCharacterEditor>("VN Characters Editor");
        }

        private string CharactersFolderPath => VNSettingsManager.GetSettings().CharactersFolder;
        private const string VNCharactersAssetPath = "Assets/Kuro-Novel/Resources/VNAssets/VNCharacters.asset";

        private VNCharacters vnCharacters;
        private List<CharacterNode> characters = new List<CharacterNode>();
        private int selectedCharacterIndex = -1;
        private Vector2 leftPanelScrollPosition;
        private Vector2 rightPanelScrollPosition;
        private GUIStyle nodeButtonStyle = new GUIStyle();

        private void OnEnable()
        {
            EnsureCharactersFolderExists();
            LoadVNCharactersAsset();
            LoadCharactersFromFolder();
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();

            // Left Panel: Character List
            GUILayout.BeginVertical(GUILayout.Width(300));
            leftPanelScrollPosition = GUILayout.BeginScrollView(leftPanelScrollPosition, GUILayout.ExpandHeight(true));

            // Left Header
            GUILayout.Label("Characters", EditorStyles.boldLabel);

            // Loop to show each character in system
            for (int i = 0; i < vnCharacters.Characters.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                if (selectedCharacterIndex == i)
                {
                    nodeButtonStyle.normal = new GUIStyleState
                    {
                        textColor = Color.cyan
                    };
                }
                else
                {
                    nodeButtonStyle.normal = new GUIStyleState
                    {
                        textColor = Color.white
                    };
                }

                // Button for selecting a character
                if (GUILayout.Button($" {i}. {vnCharacters.Characters[i].CharacterName}", nodeButtonStyle))
                    selectedCharacterIndex = i;

                // Delete button for the selected character
                if (selectedCharacterIndex == i && GUILayout.Button("Delete", GUILayout.Width(60)))
                    DeleteCharacter(i);

                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add New Character"))
            {
                AddNewCharacter();
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            // Right Panel: Character Editing
            GUILayout.BeginVertical();

            if (selectedCharacterIndex >= 0 && selectedCharacterIndex < vnCharacters.Characters.Count)
            {
                CharacterNode selectedCharacter = vnCharacters.Characters[selectedCharacterIndex];

                rightPanelScrollPosition = GUILayout.BeginScrollView(rightPanelScrollPosition, GUILayout.ExpandHeight(true));

                GUILayout.Label("Edit Character", EditorStyles.boldLabel);

                // Character Name
                string newName = EditorGUILayout.TextField("Character Name", selectedCharacter.CharacterName);
                if (newName != selectedCharacter.CharacterName && !string.IsNullOrWhiteSpace(newName))
                {
                    RenameCharacterAsset(selectedCharacter, newName);
                }

                // Character Info
                selectedCharacter.Info = EditorGUILayout.TextArea(selectedCharacter.Info, GUILayout.Height(60));

                GUILayout.Space(10);

                // Emotions Management
                GUILayout.Label("Emotions", EditorStyles.boldLabel);
                List<EmotionsNode> emotionsToRemove = new List<EmotionsNode>();

                foreach (var emotion in selectedCharacter.Emotions)
                {
                    EditorGUILayout.BeginHorizontal();

                    emotion.Emotion = EditorGUILayout.TextField("Emotion", emotion.Emotion);
                    emotion.Sprite = (Sprite)EditorGUILayout.ObjectField("Sprite", emotion.Sprite, typeof(Sprite), false);

                    if (GUILayout.Button("Remove", GUILayout.Width(60)))
                        emotionsToRemove.Add(emotion);

                    EditorGUILayout.EndHorizontal();
                }

                foreach (var emotion in emotionsToRemove)
                {
                    selectedCharacter.Emotions.Remove(emotion);
                }

                if (GUILayout.Button("Add New Emotion"))
                {
                    selectedCharacter.Emotions.Add(new EmotionsNode { Emotion = "New Emotion", Sprite = null });
                }

                GUILayout.EndScrollView();
            }
            else
            {
                GUILayout.Label("Select a character to edit.", EditorStyles.boldLabel);
            }

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        private void EnsureCharactersFolderExists()
        {
            if (!Directory.Exists(CharactersFolderPath))
            {
                Directory.CreateDirectory(CharactersFolderPath);
                AssetDatabase.Refresh();
            }
        }

        private void LoadVNCharactersAsset()
        {
            vnCharacters = AssetDatabase.LoadAssetAtPath<VNCharacters>(VNCharactersAssetPath);
            if (vnCharacters == null)
            {
                vnCharacters = ScriptableObject.CreateInstance<VNCharacters>();
                AssetDatabase.CreateAsset(vnCharacters, VNCharactersAssetPath);
                AssetDatabase.SaveAssets();
            }
        }

        private void LoadCharactersFromFolder()
        {
            if (vnCharacters.Characters == null)
            {
                vnCharacters.Characters = new List<CharacterNode>();
            }

            string[] guids = AssetDatabase.FindAssets("t:CharacterNode", new[] { CharactersFolderPath });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                CharacterNode character = AssetDatabase.LoadAssetAtPath<CharacterNode>(path);

                if (character != null && !vnCharacters.Characters.Contains(character))
                {
                    vnCharacters.Characters.Add(character);
                }
            }

            // Remove missing references
            vnCharacters.Characters.RemoveAll(character => character == null);
        }

        private void AddNewCharacter()
        {
            CharacterNode newCharacter = ScriptableObject.CreateInstance<CharacterNode>();
            newCharacter.CharacterName = "New Character";

            string path = Path.Combine(CharactersFolderPath, $"{newCharacter.CharacterName}.asset");
            path = AssetDatabase.GenerateUniqueAssetPath(path);

            AssetDatabase.CreateAsset(newCharacter, path);
            AssetDatabase.SaveAssets();

            vnCharacters.Characters.Add(newCharacter);
            selectedCharacterIndex = vnCharacters.Characters.Count - 1;

            EditorUtility.SetDirty(vnCharacters);
        }

        private void DeleteCharacter(int index)
        {
            if (index < 0 || index >= vnCharacters.Characters.Count) return;

            CharacterNode character = vnCharacters.Characters[index];
            string path = AssetDatabase.GetAssetPath(character);

            AssetDatabase.DeleteAsset(path);
            AssetDatabase.SaveAssets();

            vnCharacters.Characters.RemoveAt(index);
            selectedCharacterIndex = -1;

            EditorUtility.SetDirty(vnCharacters);
        }

        private void RenameCharacterAsset(CharacterNode character, string newName)
        {
            string oldPath = AssetDatabase.GetAssetPath(character);
            string newPath = Path.Combine(CharactersFolderPath, $"{newName}.asset");

            //newPath = AssetDatabase.GenerateUniqueAssetPath(newPath);

            AssetDatabase.RenameAsset(oldPath, newName);
            AssetDatabase.SaveAssets();

            character.CharacterName = newName;
            EditorUtility.SetDirty(character);
            EditorUtility.SetDirty(vnCharacters);
        }
    }
}