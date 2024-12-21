using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using KuroNovel.DataNode;
using KuroNovelEdior.Utils;
using KuroNovel;
using System.Linq;
using System;

namespace KuroNovelEdior
{
    public class VNSequenceEditor : EditorWindow
    {
        private VNSequence m_CurrentSequence;
        private int m_SelectedNodeIndex = -1;
        private Vector2 m_LeftPanelScrollPos;
        private Vector2 m_RightPanelScrollPos;
        private GUIStyle m_NodeButtonStyle;
        private GUIStyle m_CenterLabelStyle;
        private Color m_textColor;
        private VNCharacters m_Characters;

        [MenuItem("Kuro Novel/VN Sequence Editor", false, 0)]
        public static void ShowWindow()
        {
            GetWindow<VNSequenceEditor>("VN Sequence Editor");
        }

        private void OnEnable()
        {
            m_Characters = Resources.Load<VNCharacters>("VNAssets/VNCharacters");

            m_textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;

            m_NodeButtonStyle = new GUIStyle()
            {
                alignment = TextAnchor.MiddleLeft,
                normal = new GUIStyleState { background = Texture2D.blackTexture, textColor = m_textColor },
                padding = new RectOffset(5, 5, 5, 5)
            };

            m_CenterLabelStyle = new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                normal = new GUIStyleState { textColor = m_textColor }
            };
        }

        private void CleanInvalidNodes()
        {
            string[] guids = AssetDatabase.FindAssets("t:VNNode");

            foreach (string guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var sobj = AssetDatabase.LoadAssetAtPath<VNNode>(path);

                if (sobj == null) continue;

                if (!m_CurrentSequence.Nodes.Contains(sobj))
                {
                    AssetDatabase.RemoveObjectFromAsset(sobj);
                    EditorUtility.SetDirty(m_CurrentSequence);
                    AssetDatabase.SaveAssets();
                }
            }
        }

        private void OnGUI()
        {
            //Create or Load the sequence
            if (m_CurrentSequence == null)
            {
                GUILayout.Label("No VN Sequence loaded.", EditorStyles.boldLabel);

                LoadExistingSequence();

                if (GUILayout.Button("Create New Sequence"))
                    CreateNewSequence();

                return;
            }

            // Remove Invalid Node
            m_CurrentSequence.Nodes.RemoveAll(node => node == null);
            CleanInvalidNodes();

            // Split the window into two panels
            GUILayout.BeginHorizontal();

            //////////////////// Left Panel ////////////////////
            GUILayout.BeginVertical(GUILayout.Width(300));
            m_LeftPanelScrollPos = GUILayout.BeginScrollView(m_LeftPanelScrollPos, GUILayout.ExpandHeight(true));

            //Header of naming sequence
            GUILayout.Label($"Sequence Title", EditorStyles.boldLabel);
            string title = EditorGUILayout.TextField(m_CurrentSequence.Title);
            if (title != m_CurrentSequence.Title)
            {
                var path = AssetDatabase.GetAssetPath(m_CurrentSequence);

                AssetDatabase.RenameAsset(path, title);
                AssetDatabase.SaveAssets();

                m_CurrentSequence.name = title;
                m_CurrentSequence.Title = title;
                EditorUtility.SetDirty(m_CurrentSequence);
                AssetDatabase.Refresh();
            }

            GUILayout.Space(5);

            // Left Header
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Nodes", EditorStyles.boldLabel);
            if (GUILayout.Button("Add Node"))
                ShowAddNodeMenu();
            GUILayout.EndHorizontal();

            // Loop for Show each node in Sequence
            for (int i = 0; i < m_CurrentSequence.Nodes.Count; i++)
            {
                var node = m_CurrentSequence.Nodes[i];

                EditorGUILayout.BeginHorizontal();

                //Highlight the selected node
                if (m_SelectedNodeIndex == i)
                {
                    var activeTextColor = EditorGUIUtility.isProSkin ? Color.cyan : Color.blue;

                    m_NodeButtonStyle.normal = new GUIStyleState
                    {
                        textColor = activeTextColor
                    };
                }
                else
                {
                    m_NodeButtonStyle.normal = new GUIStyleState
                    {
                        textColor = m_textColor
                    };
                }

                // Button for node to select
                if (GUILayout.Button($"{i}. {node.NodeName}", m_NodeButtonStyle))
                    m_SelectedNodeIndex = i;

                // Swap position of node
                if (m_SelectedNodeIndex == i && GUILayout.Button("▲", GUILayout.Width(30)))
                    SwapNodes(i, i - 1);
                if (m_SelectedNodeIndex == i && GUILayout.Button("▼", GUILayout.Width(30)))
                    SwapNodes(i, i + 1);

                // Delete the selected node
                if (m_SelectedNodeIndex == i && GUILayout.Button("Delete", GUILayout.Width(60)))
                    DeleteNode(i);

                EditorGUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();

            GUILayout.FlexibleSpace();
            GUILayout.Space(5);

            // Footer of Left Panel
            LoadExistingSequence();

            if (GUILayout.Button("Create New Sequence"))
                CreateNewSequence();

            if (GUILayout.Button("Save Sequence"))
            {
                SaveToScriptableObject();
            }

            GUILayout.EndVertical();
            GUILayout.Space(5);

            //////////////////// Right Panel ////////////////////
            GUILayout.BeginVertical();

            if (m_SelectedNodeIndex >= 0 && m_SelectedNodeIndex < m_CurrentSequence.Nodes.Count)
            {
                var selectedNode = m_CurrentSequence.Nodes[m_SelectedNodeIndex];

                m_RightPanelScrollPos = GUILayout.BeginScrollView(m_RightPanelScrollPos, GUILayout.ExpandHeight(true));

                GUILayout.Label("Edit Node: " + selectedNode.NodeName, EditorStyles.boldLabel);
                GUILayout.BeginHorizontal();

                // Naming VN Node
                GUILayout.Label("Node Name: ", EditorStyles.boldLabel);
                var newNodeName = EditorGUILayout.TextField(selectedNode.NodeName);
                if (newNodeName != selectedNode.NodeName)
                {
                    selectedNode.NodeName = newNodeName;
                    selectedNode.name = newNodeName;
                    EditorUtility.SetDirty(m_CurrentSequence);
                    AssetDatabase.SaveAssets();
                }

                // Editing Node Type
                var newType = (VNNodeType)EditorGUILayout.EnumPopup(selectedNode.NodeType, GUILayout.Width(100));
                if (newType != selectedNode.NodeType)
                    ChangeNodeType(m_SelectedNodeIndex, newType);

                GUILayout.EndHorizontal();

                // Edit properties based on the node type
                if (selectedNode is DialogueNode dialogueNode)
                    VNEditorUtility.DrawDialogueNode(ref dialogueNode);
                //DrawDialogueNode(ref dialogueNode);
                else if (selectedNode is ChoicesNode choiceNode)
                    VNEditorUtility.DrawChoicesNode(ref choiceNode);
                else if (selectedNode is SpriteNode spriteNode)
                    VNEditorUtility.DrawSpriteNode(ref spriteNode);
                else if (selectedNode is BackgroundNode backgroundNode)
                    VNEditorUtility.DrawBackgroundNode(ref backgroundNode);
                else if (selectedNode is BGMNode bgmNode)
                    VNEditorUtility.DrawBGMNode(ref bgmNode);
                else
                    DrawCenterLabel("Coming Soon!");

                GUILayout.EndScrollView();
            }
            else
                DrawCenterLabel("Select a node to edit.");

            GUILayout.EndVertical(); // End right panel
            GUILayout.EndHorizontal(); // End split layout
        }

        private void DrawCenterLabel(string message)
        {
            GUILayout.FlexibleSpace();
            GUILayout.Label(message, m_CenterLabelStyle);
            GUILayout.FlexibleSpace();
        }

        #region Save & Load Sequence
        // Save Sequence
        private void SaveToScriptableObject()
        {
            CleanupNullNodes();
            var path = VNSettingsManager.GetSettings().SequencesFolder;

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            if (!string.IsNullOrEmpty(path))
            {
                Debug.Log(path + $"{m_CurrentSequence.Title}.asset");

                if (!File.Exists(path + m_CurrentSequence.Title + ".asset"))
                    AssetDatabase.CreateAsset(m_CurrentSequence, path + $"{m_CurrentSequence.Title}.asset");

                EditorUtility.SetDirty(m_CurrentSequence);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log("Sequence saved as ScriptableObject at: " + path);
            }
        }

        // Load Sequence
        private void LoadExistingSequence() =>
             m_CurrentSequence = EditorGUILayout.ObjectField("Load Sequence", m_CurrentSequence,
                                    typeof(VNSequence), false) as VNSequence;

        // Create new Sequence
        private void CreateNewSequence()
        {
            m_CurrentSequence = CreateInstance<VNSequence>();
            m_CurrentSequence.Title = "New Sequence";
            m_CurrentSequence.name = "New Sequence";
            m_SelectedNodeIndex = -1;
        }
        #endregion

        #region Node Utilities
        private void AddNewNode(VNNodeType type)
        {
            VNNode newNode = VNNodeFactory.CreateNode(type);
            newNode.NodeName = GetUniqueNodeName("New " + type + " Node");

            newNode.name = newNode.NodeName;
            AssetDatabase.AddObjectToAsset(newNode, m_CurrentSequence);
            m_CurrentSequence.Nodes.Add(newNode);

            EditorUtility.SetDirty(newNode);
            EditorUtility.SetDirty(m_CurrentSequence);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            m_SelectedNodeIndex = m_CurrentSequence.Nodes.Count - 1; // Select the new node
        }

        private void DeleteNode(int index)
        {
            if (index >= 0 && index < m_CurrentSequence.Nodes.Count)
            {
                DestroyImmediate(m_CurrentSequence.Nodes[index], true);
                m_CurrentSequence.Nodes.RemoveAt(index);
                EditorUtility.SetDirty(m_CurrentSequence);
                AssetDatabase.SaveAssets();
                m_SelectedNodeIndex = -1;
            }
        }

        private void SwapNodes(int index1, int index2)
        {
            if (index1 < 0 || index2 < 0 || index1 >= m_CurrentSequence.Nodes.Count || index2 >= m_CurrentSequence.Nodes.Count)
                return;

            var temp = m_CurrentSequence.Nodes[index1];
            m_CurrentSequence.Nodes[index1] = m_CurrentSequence.Nodes[index2];
            m_CurrentSequence.Nodes[index2] = temp;

            m_SelectedNodeIndex = index2;
            EditorUtility.SetDirty(m_CurrentSequence);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private string GetUniqueNodeName(string baseName)
        {
            int index = 1;
            string uniqueName = baseName;
            while (m_CurrentSequence.Nodes.Any(node => node.NodeName == uniqueName))
            {
                uniqueName = baseName + " " + index++;
            }
            return uniqueName;
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

        private void CleanupNullNodes()
        {
            if (m_CurrentSequence != null && m_CurrentSequence.Nodes != null)
            {
                m_CurrentSequence.Nodes.RemoveAll(node => node == null);
                EditorUtility.SetDirty(m_CurrentSequence);
                AssetDatabase.SaveAssets();
            }
        }

        private void ChangeNodeType(int index, VNNodeType newType)
        {
            VNNode oldNode = m_CurrentSequence.Nodes[index];
            VNNode newNode = VNNodeFactory.CreateNode(newType);
            newNode.NodeName = oldNode.NodeName; // Retain the node name
            m_CurrentSequence.Nodes[index] = newNode;
        }
        #endregion
    }
}
