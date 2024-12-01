using Newtonsoft.Json;
using KuroNovel.DataNode;
using UnityEditor;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace KuroNovelEdior
{
    public class VNCharacterEditor : EditorWindow
    {
        private List<VNCharacter> characters = new List<VNCharacter>();
        private int selectedCharacterIndex = -1; // Index of the selected character
        private Vector2 leftPanelScrollPosition; // Scroll position for left panel
        private Vector2 rightPanelScrollPosition; // Scroll position for right panel

        [MenuItem("Kuro Novel/VN Character Editor", false, 0)]
        public static void ShowWindow()
        {
            GetWindow<VNCharacterEditor>("VN Character Editor");
        }

        private void OnEnable()
        {
            //LoadCharacters(); // Load existing characters if available
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();

            // Left Panel: Character List
            GUILayout.BeginVertical(GUILayout.Width(300));
            leftPanelScrollPosition = GUILayout.BeginScrollView(leftPanelScrollPosition, GUILayout.ExpandHeight(true));

            GUILayout.Label("Characters", EditorStyles.boldLabel);

            for (int i = 0; i < characters.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                // Button for selecting a character
                if (GUILayout.Button(characters[i].CharacterName, GUILayout.ExpandWidth(true)))
                    selectedCharacterIndex = i;

                // Delete button for the selected character
                if (selectedCharacterIndex == i && GUILayout.Button("Delete", GUILayout.Width(60)))
                    DeleteCharacter(i);

                EditorGUILayout.EndHorizontal();
            }

            // Add New Character Button
            if (GUILayout.Button("Add New Character"))
            {
                AddNewCharacter();
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            // Right Panel: Character Editing
            GUILayout.BeginVertical();

            if (selectedCharacterIndex >= 0 && selectedCharacterIndex < characters.Count)
            {
                VNCharacter selectedCharacter = characters[selectedCharacterIndex];

                rightPanelScrollPosition = GUILayout.BeginScrollView(rightPanelScrollPosition, GUILayout.ExpandHeight(true));

                GUILayout.Label("Edit Character", EditorStyles.boldLabel);

                // Character Name
                selectedCharacter.CharacterName = EditorGUILayout.TextField("Character Name", selectedCharacter.CharacterName);

                // Character Info
                selectedCharacter.Info = EditorGUILayout.TextArea(selectedCharacter.Info, GUILayout.Height(60));

                GUILayout.Space(10);

                // Emotions
                GUILayout.Label("Emotions", EditorStyles.boldLabel);
                List<string> keysToRemove = new List<string>();
                foreach (var emotion in selectedCharacter.Emotions)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label(emotion.Key, GUILayout.Width(100));
                    selectedCharacter.Emotions[emotion.Key] = EditorGUILayout.TextField(emotion.Value);
                    if (GUILayout.Button("Remove", GUILayout.Width(60)))
                    {
                        keysToRemove.Add(emotion.Key);
                    }
                    EditorGUILayout.EndHorizontal();
                }

                // Remove emotions marked for deletion
                foreach (var key in keysToRemove)
                {
                    selectedCharacter.Emotions.Remove(key);
                }

                // Add Emotion
                if (GUILayout.Button("Add Emotion"))
                {
                    selectedCharacter.Emotions.Add("New Emotion", "");
                }

                GUILayout.EndScrollView();
            }
            else
            {
                GUILayout.Label("Select a character to edit.", EditorStyles.boldLabel);
            }

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            // Save Button at the bottom
            if (GUILayout.Button("Save Characters"))
            {
                SaveCharacters();
            }
        }

        private void AddNewCharacter()
        {
            VNCharacter newCharacter = new VNCharacter { CharacterName = "New Character" };
            characters.Add(newCharacter);
            selectedCharacterIndex = characters.Count - 1;
        }

        private void DeleteCharacter(int index)
        {
            if (index >= 0 && index < characters.Count)
            {
                characters.RemoveAt(index);
                selectedCharacterIndex = -1;
            }
        }

        private void SaveCharacters()
        {
            string path = EditorUtility.SaveFilePanel("Save Characters", "Assets", "characters.json", "json");
            if (!string.IsNullOrEmpty(path))
            {
                string json = JsonConvert.SerializeObject(characters, Formatting.Indented);
                File.WriteAllText(path, json);
                AssetDatabase.Refresh();
                Debug.Log("Characters saved to: " + path);
            }
        }

        private void LoadCharacters()
        {
            string path = EditorUtility.OpenFilePanel("Load Characters", "Assets", "json");
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                string json = File.ReadAllText(path);
                characters = JsonConvert.DeserializeObject<List<VNCharacter>>(json) ?? new List<VNCharacter>();
                Debug.Log("Characters loaded from: " + path);
            }
            else
            {
                characters = new List<VNCharacter>();
            }
        }
    }
}