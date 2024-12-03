using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using KuroNovel.DataNode;
using KuroNovelEdior.Utils;
using KuroNovel;
using System.Linq;

namespace KuroNovelEdior
{
    public class VNSequenceEditor : EditorWindow
    {
        private VNSequence currentSequence; // The sequence we are editing
        private int selectedNodeIndex = -1; // Index of the selected node in the list
        private Vector2 leftPanelScrollPosition; // Scroll position for left panel
        private Vector2 rightPanelScrollPosition; // Scroll position for right panel
        private GUIStyle nodeButtonStyle;
        //private int m_SelectedCharacterIndex = 0;
        private VNCharacters vn_Characters;

        [MenuItem("Kuro Novel/VN Sequence Editor", false, 0)]
        public static void ShowWindow()
        {
            GetWindow<VNSequenceEditor>("VN Sequence Editor");
        }

        private void OnEnable()
        {
            vn_Characters = Resources.Load<VNCharacters>("VNAssets/VNCharacters");

            nodeButtonStyle = new GUIStyle()
            {
                alignment = TextAnchor.MiddleLeft,
                normal = new GUIStyleState { background = Texture2D.blackTexture, textColor = Color.white },
                active = new GUIStyleState { background = Texture2D.grayTexture, textColor = Color.cyan },
                padding = new RectOffset(5, 5, 5, 5)
            };
        }

        private void SwapNodes(int index1, int index2)
        {
            if (index1 < 0 || index2 < 0 || index1 >= currentSequence.Nodes.Count || index2 >= currentSequence.Nodes.Count)
                return;

            var temp = currentSequence.Nodes[index1];
            currentSequence.Nodes[index1] = currentSequence.Nodes[index2];
            currentSequence.Nodes[index2] = temp;

            selectedNodeIndex = index2;
            EditorUtility.SetDirty(currentSequence);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void OnGUI()
        {
            if (currentSequence == null)
            {
                GUILayout.Label("No VN Sequence loaded.", EditorStyles.boldLabel);

                LoadExistingSequence();

                if (GUILayout.Button("Create New Sequence"))
                    CreateNewSequence();

                return;
            }

            // Split the window into two panels
            GUILayout.BeginHorizontal();

            // Left Panel
            GUILayout.BeginVertical(GUILayout.Width(300));
            leftPanelScrollPosition = GUILayout.BeginScrollView(leftPanelScrollPosition, GUILayout.ExpandHeight(true));

            GUILayout.Label($"Sequence Title", EditorStyles.boldLabel);
            string title = EditorGUILayout.TextField(currentSequence.Title);
            if (title != currentSequence.Title)
            {
                string path = AssetDatabase.GetAssetPath(currentSequence);

                AssetDatabase.RenameAsset(path, title);
                AssetDatabase.SaveAssets();

                currentSequence.name = title;
                currentSequence.Title = title;
                EditorUtility.SetDirty(currentSequence);
                AssetDatabase.Refresh();
            }

            if (GUILayout.Button("Restore"))
                ValidateSequenceNodes();
            GUILayout.Space(5);

            // Left Header
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Nodes", EditorStyles.boldLabel);
            if (GUILayout.Button("Add Node"))
                ShowAddNodeMenu();
            GUILayout.EndHorizontal();

            // Loop for Show each node in Sequence
            for (int i = 0; i < currentSequence.Nodes.Count; i++)
            {
                var node = currentSequence.Nodes[i];

                EditorGUILayout.BeginHorizontal();

                if (selectedNodeIndex == i)
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

                if (GUILayout.Button($"{i}. {node.NodeName}", nodeButtonStyle))
                    selectedNodeIndex = i;

                if (selectedNodeIndex == i && GUILayout.Button("▲", GUILayout.Width(30)))
                    SwapNodes(i, i - 1);
                if (selectedNodeIndex == i && GUILayout.Button("▼", GUILayout.Width(30)))
                    SwapNodes(i, i + 1);

                if (selectedNodeIndex == i && GUILayout.Button("Delete", GUILayout.Width(60)))
                    DeleteNode(i);

                EditorGUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();

            GUILayout.FlexibleSpace();
            GUILayout.Space(5);

            LoadExistingSequence();

            if (GUILayout.Button("Create New Sequence"))
                CreateNewSequence();

            if (GUILayout.Button("Save Sequence"))
            {
                if (VNSettingsManager.GetSettings().SaveDataAs.Equals(VNSettings.SaveDataMode.JSON))
                    SaveToJSON();
                else
                    SaveToScriptableObject();
            }

            GUILayout.EndVertical();
            GUILayout.Space(5);

            // Right Panel: Node Editing
            GUILayout.BeginVertical();

            if (selectedNodeIndex >= 0 && selectedNodeIndex < currentSequence.Nodes.Count)
            {
                VNNode selectedNode = currentSequence.Nodes[selectedNodeIndex];

                rightPanelScrollPosition = GUILayout.BeginScrollView(rightPanelScrollPosition, GUILayout.ExpandHeight(true));

                GUILayout.Label("Edit Node: " + selectedNode.NodeName, EditorStyles.boldLabel);

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                GUILayout.Label("Node Name: ", EditorStyles.boldLabel);
                string newNodeName = EditorGUILayout.TextField(selectedNode.NodeName);
                if (newNodeName != selectedNode.NodeName)
                {
                    selectedNode.NodeName = newNodeName;
                    selectedNode.name = newNodeName;  // Update object name
                    EditorUtility.SetDirty(currentSequence);
                    AssetDatabase.SaveAssets();
                }

                VNNodeType newType = (VNNodeType)EditorGUILayout.EnumPopup(selectedNode.NodeType, GUILayout.Width(100));
                if (newType != selectedNode.NodeType)
                    ChangeNodeType(selectedNodeIndex, newType);

                GUILayout.EndHorizontal();

                // Edit properties based on the node type
                if (selectedNode is DialogueNode dialogueNode)
                    DrawDialogueNode(ref dialogueNode);
                else if (selectedNode is ChoicesNode choiceNode)
                    DrawChoicesNode(ref choiceNode);
                else if (selectedNode is SpriteNode spriteNode)
                    DrawSpriteNode(ref spriteNode);
                else if (selectedNode is BackgroundNode backgroundNode)
                    DrawBackgroundNode(ref backgroundNode);

                GUILayout.EndScrollView();
            }
            else
            {
                GUILayout.Label("Select a node to edit.", EditorStyles.boldLabel);
            }

            GUILayout.EndVertical(); // End right panel

            GUILayout.EndHorizontal(); // End split layout
        }

        #region Save Load Func

        private void SaveToScriptableObject()
        {
            //string path = EditorUtility.SaveFilePanel("Save VN Sequence", VNSettingsManager.GetSettings().CharactersFolder, $"{currentSequence.Title}.asset", "asset");
            CleanupNullNodes();
            string path = VNSettingsManager.GetSettings().SequencesFolder;

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            if (!string.IsNullOrEmpty(path))
            {
                if (!File.Exists(path + currentSequence.Title + ".asset"))
                    AssetDatabase.CreateAsset(currentSequence, path + $"{currentSequence.Title}.asset");

                EditorUtility.SetDirty(currentSequence);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log("Sequence saved as ScriptableObject at: " + path);
            }
        }

        private void SaveToJSON()
        {
            /*string json = JsonConvert.SerializeObject(currentSequence, VNSettings.serializerSettings);
            string path = EditorUtility.SaveFilePanel("Save VN Sequence", "Assets", currentSequence.Title + ".json", "json");

            if (!string.IsNullOrEmpty(path))
            {
                File.WriteAllText(path, json);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log("Sequence saved to: " + path);
            }*/
        }

        private void CreateNewSequence()
        {
            currentSequence = CreateInstance<VNSequence>();
            currentSequence.Title = "New Sequence";
            currentSequence.name = "New Sequence";
            selectedNodeIndex = -1;
        }

        private void LoadExistingSequence()
        {
            /*if (VNSettingsManager.GetSettings().SaveDataAs == VNSettings.SaveDataMode.JSON)
            {
                if (GUILayout.Button("Load Json"))
                {
                    string path = EditorUtility.OpenFilePanel("Save VN Sequence", "Assets", ".json");
                    if (string.IsNullOrEmpty(path)) return;
                    string json = File.ReadAllText(path);
                    currentSequence = JsonConvert.DeserializeObject<VNSequence>(json, VNSettings.serializerSettings);
                }

                return;
            }*/

            currentSequence = EditorGUILayout.ObjectField("Load Sequence", currentSequence, typeof(VNSequence), false) as VNSequence;
        }

        private void ValidateSequenceNodes()
        {
            if (currentSequence == null || currentSequence.Nodes == null || currentSequence.Nodes.Count == 0)
            {
                Debug.LogWarning("Nodes list is null, attempting to restore.");
                currentSequence.Nodes = new List<VNNode>(AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(currentSequence))
                                        .OfType<VNNode>());
                if (currentSequence.Nodes.Count == 0)
                {
                    Debug.LogError("Failed to restore nodes.");
                }
            }
        }
        #endregion

        private void ShowAddNodeMenu()
        {
            GenericMenu menu = new GenericMenu();
            foreach (VNNodeType type in System.Enum.GetValues(typeof(VNNodeType)))
            {
                menu.AddItem(new GUIContent(type.ToString()), false, () => AddNewNode(type));
            }
            menu.ShowAsContext();
        }

        private void AddNewNode(VNNodeType type)
        {
            bool isJson = VNSettingsManager.GetSettings().SaveDataAs == VNSettings.SaveDataMode.JSON;
            VNNode newNode = VNNodeFactory.CreateNode(type, isJson);
            newNode.NodeName = GetUniqueNodeName("New " + type + " Node");

            newNode.name = newNode.NodeName;
            AssetDatabase.AddObjectToAsset(newNode, currentSequence);
            currentSequence.Nodes.Add(newNode);

            EditorUtility.SetDirty(newNode);
            EditorUtility.SetDirty(currentSequence);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            selectedNodeIndex = currentSequence.Nodes.Count - 1; // Select the new node
        }

        private string GetUniqueNodeName(string baseName)
        {
            int index = 1;
            string uniqueName = baseName;
            while (currentSequence.Nodes.Any(node => node.NodeName == uniqueName))
            {
                uniqueName = baseName + " " + index++;
            }
            return uniqueName;
        }

        private void DeleteNode(int index)
        {
            if (index >= 0 && index < currentSequence.Nodes.Count)
            {
                DestroyImmediate(currentSequence.Nodes[index], true);
                currentSequence.Nodes.RemoveAt(index);
                EditorUtility.SetDirty(currentSequence);
                AssetDatabase.SaveAssets();
                selectedNodeIndex = -1;
            }
        }

        private void CleanupNullNodes()
        {
            if (currentSequence != null && currentSequence.Nodes != null)
            {
                currentSequence.Nodes.RemoveAll(node => node == null);
                EditorUtility.SetDirty(currentSequence);
                AssetDatabase.SaveAssets();
            }
        }

        private void ChangeNodeType(int index, VNNodeType newType)
        {
            var isJson = VNSettingsManager.GetSettings().SaveDataAs == VNSettings.SaveDataMode.JSON;
            VNNode oldNode = currentSequence.Nodes[index];
            VNNode newNode = VNNodeFactory.CreateNode(newType, isJson);
            newNode.NodeName = oldNode.NodeName; // Retain the node name
            currentSequence.Nodes[index] = newNode;
        }

        #region GUI Drawer
        private void DrawDialogueNode(ref DialogueNode node)
        {
            int i = 0;
            if (node.Character == null)
                i = vn_Characters.Characters.IndexOf(vn_Characters.Characters.FirstOrDefault());
            else
                i = vn_Characters.Characters.IndexOf(node.Character);

            i = EditorGUILayout.Popup("Select Character", i,
            vn_Characters.Characters.ConvertAll(e => e.CharacterName).ToArray());

            node.Character = vn_Characters.Characters[i];
            node.Speaker = EditorGUILayout.TextField("Speaker", vn_Characters.Characters[i].CharacterName);

            GUILayout.Label("Dialogue", EditorStyles.boldLabel);
            node.DialogueText = EditorGUILayout.TextArea(node.DialogueText);
            node.VoiceLine = EditorGUILayout.ObjectField(node.VoiceLine, typeof(AudioClip), false, GUILayout.Height(EditorGUIUtility.singleLineHeight)) as AudioClip;
        }

        private void DrawChoicesNode(ref ChoicesNode node)
        {
            if (node.Choices != null || node.Choices.Count != 0)
            {
                node.Prompt = EditorGUILayout.TextField("Prompt", node.Prompt);

                for (int j = 0; j < node.Choices.Count; j++)
                {
                    EditorGUILayout.BeginHorizontal();
                    node.Choices[j].Text = EditorGUILayout.TextField("Choice Text", node.Choices[j].Text);
                    node.Choices[j].TargetID = EditorGUILayout.TextField("Target Node ID", node.Choices[j].TargetID);

                    if (GUILayout.Button("Delete Choice", GUILayout.Width(100)))
                        node.Choices.RemoveAt(j);

                    EditorGUILayout.EndHorizontal();
                }
            }

            if (GUILayout.Button("Add Choice"))
                node.Choices.Add(new Choice());
        }

        private void DrawSpriteNode(ref SpriteNode node)
        {
            // Ensure characters exist
            if (vn_Characters.Characters == null || vn_Characters.Characters.Count == 0)
            {
                EditorGUILayout.HelpBox("No characters available. Add characters to assign a sprite.", MessageType.Warning);
                return;
            }

            // Character Selection
            int selectedCharacterIndex = node.Character == null
                ? 0
                : vn_Characters.Characters.IndexOf(node.Character);

            selectedCharacterIndex = Mathf.Clamp(selectedCharacterIndex, 0, vn_Characters.Characters.Count - 1);

            selectedCharacterIndex = EditorGUILayout.Popup(
                "Select Character",
                selectedCharacterIndex,
                vn_Characters.Characters.ConvertAll(c => c.CharacterName).ToArray()
            );

            node.Character = vn_Characters.Characters[selectedCharacterIndex];

            // Ensure the selected character has emotions
            if (node.Character.Emotions == null || node.Character.Emotions.Count == 0)
            {
                EditorGUILayout.HelpBox($"Character '{node.Character.CharacterName}' has no emotions. Add emotions to assign a sprite.", MessageType.Info);
                return;
            }

            int selectedEmotionIndex = node.Character.EmotionsIndexOf(node.Emotion);

            if (selectedEmotionIndex == -1) selectedEmotionIndex = 0;

            selectedEmotionIndex = EditorGUILayout.Popup(
                "Select Emotion",
                selectedEmotionIndex,
                node.Character.Emotions.ConvertAll(e => e.Emotion).ToArray()
            );

            node.Emotion = node.Character.Emotions[selectedEmotionIndex].Emotion;

            // Assign Sprite
            node.CharacterSprite = node.Character.Emotions[selectedEmotionIndex].Sprite;

            // Display Assigned Sprite
            EditorGUILayout.ObjectField("Selected Sprite", node.CharacterSprite, typeof(Sprite), false);
        }

        private void DrawBackgroundNode(ref BackgroundNode node)
        {
            node.Background = EditorGUILayout.ObjectField(node.Background, typeof(Sprite), false, GUILayout.Height(EditorGUIUtility.singleLineHeight)) as Sprite;

            /*if (m_sprite != null)
            {
                var filePath = AssetDatabase.GetAssetPath(m_sprite);
                GUILayout.Label($"{m_sprite.name} will load from: {filePath}");
            }*/
        }
        #endregion
    }
}
