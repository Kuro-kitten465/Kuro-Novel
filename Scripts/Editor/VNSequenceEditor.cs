using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using KuroNovel.DataNode;
using KuroNovelEdior.Utils;

namespace KuroNovelEdior
{
    public class VNSequenceEditor : EditorWindow
    {
        private VNSequence currentSequence; // The sequence we are editing
        private int selectedNodeIndex = -1; // Index of the selected node in the list
        private Vector2 leftPanelScrollPosition; // Scroll position for left panel
        private Vector2 rightPanelScrollPosition; // Scroll position for right panel

        private GUIStyle nodeButtonStyle;

        [MenuItem("Kuro Novel/VN Sequence Editor", false, 0)]
        public static void ShowWindow()
        {
            GetWindow<VNSequenceEditor>("VN Sequence Editor");
        }

        private void OnEnable()
        {
            currentSequence = null;

            nodeButtonStyle = new GUIStyle()
            {
                alignment = TextAnchor.MiddleLeft,
                normal = new GUIStyleState { background = Texture2D.blackTexture, textColor = Color.white },
                active = new GUIStyleState { background = Texture2D.grayTexture, textColor = Color.cyan },
                padding = new RectOffset(5, 5, 5, 5)
            };
        }

        private void OnGUI()
        {
            if (currentSequence == null)
            {
                GUILayout.Label("No VN Sequence loaded.", EditorStyles.boldLabel);

                if (GUILayout.Button("Create New Sequence"))
                    CreateNewSequence();

                if (GUILayout.Button("Load Existing Sequence"))
                    LoadExistingSequence();

                return;
            }

            // Split the window into two panels
            GUILayout.BeginHorizontal();

            // Left Panel: Node List
            GUILayout.BeginVertical(GUILayout.Width(250));
            leftPanelScrollPosition = GUILayout.BeginScrollView(leftPanelScrollPosition, GUILayout.ExpandHeight(true));

            currentSequence.Title = EditorGUILayout.TextField("Title: ",currentSequence.Title);
            GUILayout.Label($"Nodes", EditorStyles.boldLabel);

            for (int i = 0; i < currentSequence.Nodes.Count; i++)
            {
                var node = currentSequence.Nodes[i];

                EditorGUILayout.BeginHorizontal();

                if (selectedNodeIndex == i)
                {
                    nodeButtonStyle.normal = new GUIStyleState
                    {
                        background = Texture2D.grayTexture,
                        textColor = Color.cyan
                    };
                }
                else
                {
                    nodeButtonStyle.normal = new GUIStyleState
                    {
                        background = Texture2D.blackTexture,
                        textColor = Color.white
                    };
                }

                // Make the node name a button for selecting
                if (GUILayout.Button(node.NodeName, nodeButtonStyle))
                {
                    selectedNodeIndex = i; // Select the node for editing
                }

                // Show "Delete" Button only for the selected node
                if (selectedNodeIndex == i && GUILayout.Button("Delete", GUILayout.Width(60)))
                {
                    DeleteNode(i);
                }

                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(20);

            if (GUILayout.Button("Add New Node"))
                ShowAddNodeMenu();

            if (GUILayout.Button("Save To Json"))
                SaveToJSON();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Load From Json"))
                LoadExistingSequence();

            if (GUILayout.Button("Create New Sequence"))
                CreateNewSequence();

            GUILayout.EndHorizontal();

            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            // Right Panel: Node Editing
            GUILayout.BeginVertical();

            if (selectedNodeIndex >= 0 && selectedNodeIndex < currentSequence.Nodes.Count)
            {
                VNNode selectedNode = currentSequence.Nodes[selectedNodeIndex];

                rightPanelScrollPosition = GUILayout.BeginScrollView(rightPanelScrollPosition, GUILayout.ExpandHeight(true));

                GUILayout.Label("Edit Node: " + selectedNode.NodeName, EditorStyles.boldLabel);

                // Edit node name
                selectedNode.NodeName = EditorGUILayout.TextField("Node Name", selectedNode.NodeName);

                // Change node type
                VNNodeType newType = (VNNodeType)EditorGUILayout.EnumPopup("Node Type", selectedNode.NodeType);
                if (newType != selectedNode.NodeType)
                    ChangeNodeType(selectedNodeIndex, newType);

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

        private void SaveToJSON()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            // Serialize the sequence object to JSON using Newtonsoft.Json
            string json = JsonConvert.SerializeObject(currentSequence, settings);

            // Open a save file panel to allow the user to choose the save location
            string path = EditorUtility.SaveFilePanel("Save VN Sequence", "Assets", currentSequence.Title + ".json", "json");

            if (!string.IsNullOrEmpty(path))
            {
                // Write the JSON string to the selected file
                File.WriteAllText(path, json);

                // Refresh the asset database to reflect the new file in the Unity editor
                AssetDatabase.Refresh();

                // Log a message indicating that the file was saved
                Debug.Log("Sequence saved to: " + path);
            }
        }

        private void CreateNewSequence()
        {
            currentSequence = new VNSequence() { Title = "New" };
            selectedNodeIndex = -1;
        }

        private void LoadExistingSequence()
        {
            string path = EditorUtility.OpenFilePanel("Load VN Sequence", "Assets", "json");
            if (!string.IsNullOrEmpty(path))
            {
                JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
                string json = File.ReadAllText(path);
                currentSequence = JsonConvert.DeserializeObject<VNSequence>(json, settings);
            }
        }

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
            VNNode newNode = VNNodeFactory.CreateNode(type);
            newNode.NodeName = "New " + type + " Node";
            currentSequence.Nodes.Add(newNode);
            selectedNodeIndex = currentSequence.Nodes.Count - 1; // Select the new node
        }

        private void DeleteNode(int index)
        {
            if (index >= 0 && index < currentSequence.Nodes.Count)
            {
                currentSequence.Nodes.RemoveAt(index);
                selectedNodeIndex = -1;
            }
        }

        private void ChangeNodeType(int index, VNNodeType newType)
        {
            VNNode oldNode = currentSequence.Nodes[index];
            VNNode newNode = VNNodeFactory.CreateNode(newType);
            newNode.NodeName = oldNode.NodeName; // Retain the node name
            currentSequence.Nodes[index] = newNode;
        }

        #region GUI Drawer
        private void DrawDialogueNode(ref DialogueNode node)
        {
            GUILayout.Label("Dialogue", EditorStyles.boldLabel);
            node.DialogueText = EditorGUILayout.TextArea(node.DialogueText);
            node.Speaker = EditorGUILayout.TextField("Speaker", node.Speaker);
            node.VoiceLine = EditorGUILayout.TextField("Voice Line", node.VoiceLine);
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
            //node.Character = EditorGUILayout.ObjectField()
        }

        private Sprite m_sprite = null;
        private void DrawBackgroundNode(ref BackgroundNode node)
        {
            m_sprite = EditorGUILayout.ObjectField(m_sprite, typeof(Sprite), false, GUILayout.Height(EditorGUIUtility.singleLineHeight)) as Sprite;

            if (m_sprite != null)
            {
                node.Background = AssetDatabase.GetAssetPath(m_sprite);
                GUILayout.Label($"{m_sprite.name} has path is: {node.Background}");
            }
        }
        #endregion
    }
}
