using KuroNovelEdior;
using KuroNovel.DataNode;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace KuroNovelEdior
{
    public class VNEditorUtility : EditorWindow
    {
        private static readonly VNCharacters m_Characters = Resources.Load<VNCharacters>("VNAssets/VNCharacters");
        private static bool m_ShowPreview = true;

        public static void DrawDialogueNode(ref DialogueNode node)
        {
            if (m_Characters.Characters == null || m_Characters.Characters.Count == 0)
            {
                EditorGUILayout.HelpBox("No characters available. Add characters to assign a sprite.", MessageType.Warning);
                return;
            }

            int i = 0;
            if (node.Character == null)
                i = m_Characters.Characters.IndexOf(m_Characters.Characters.FirstOrDefault());
            else
                i = m_Characters.Characters.IndexOf(node.Character);

            i = EditorGUILayout.Popup("Select Character", i,
            m_Characters.Characters.ConvertAll(e => e.CharacterName).ToArray());

            node.Character = m_Characters.Characters[i];
            node.Speaker = EditorGUILayout.TextField("Speaker", m_Characters.Characters[i].CharacterName);

            GUILayout.Label("Dialogue", EditorStyles.boldLabel);
            node.DialogueText = EditorGUILayout.TextArea(node.DialogueText);
            //node.VoiceLine = EditorGUILayout.ObjectField(node.VoiceLine, typeof(AudioClip), false, GUILayout.Height(EditorGUIUtility.singleLineHeight)) as AudioClip;
        }

        public static void DrawChoicesNode(ref ChoicesNode node)
        {
            if (node.Choices != null || node.Choices.Count != 0)
            {
                node.Prompt = EditorGUILayout.TextField("Prompt", node.Prompt);

                for (int j = 0; j < node.Choices.Count; j++)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("Choice Text");
                    node.Choices[j].Text = EditorGUILayout.TextField(node.Choices[j].Text);
                    GUILayout.Label("Target");
                    node.Choices[j].TargetID = EditorGUILayout.TextField(node.Choices[j].TargetID);

                    if (GUILayout.Button("Delete", GUILayout.Width(60)))
                        node.Choices.RemoveAt(j);

                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                }
            }

            if (GUILayout.Button("Add Choice"))
                node.Choices.Add(new Choice());
        }

        public static void DrawSpriteNode(ref SpriteNode node)
        {
            // Ensure characters exist
            if (m_Characters.Characters == null || m_Characters.Characters.Count == 0)
            {
                EditorGUILayout.HelpBox("No characters available. Add characters to assign a sprite.", MessageType.Warning);
                return;
            }

            // Character Selection
            int selectedCharacterIndex = node.Character == null
                ? 0
                : m_Characters.Characters.IndexOf(node.Character);

            selectedCharacterIndex = Mathf.Clamp(selectedCharacterIndex, 0, m_Characters.Characters.Count - 1);

            selectedCharacterIndex = EditorGUILayout.Popup(
                "Select Character",
                selectedCharacterIndex,
                m_Characters.Characters.ConvertAll(c => c.CharacterName).ToArray()
            );

            node.Character = m_Characters.Characters[selectedCharacterIndex];

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
            EditorGUILayout.ObjectField("Selected Sprite", node.CharacterSprite, typeof(Sprite), false, GUILayout.Height(EditorGUIUtility.singleLineHeight));

            if (node.CharacterSprite != null)
                ShowImagePreview(node.CharacterSprite.texture);
        }

        public static void DrawBackgroundNode(ref BackgroundNode node)
        {
            node.Background = EditorGUILayout.ObjectField(node.Background, typeof(Sprite), false,
                                GUILayout.Height(EditorGUIUtility.singleLineHeight)) as Sprite;

            if (node.Background != null)
                ShowImagePreview(node.Background.texture);
        }

        public static void DrawBGMNode(ref BGMNode node)
        {
            node.BGM = EditorGUILayout.ObjectField(node.BGM, typeof(AudioClip), false,
                                GUILayout.Height(EditorGUIUtility.singleLineHeight)) as AudioClip;
        }

        private static void ShowImagePreview(Texture texture)
        {
            m_ShowPreview = EditorGUILayout.BeginToggleGroup("Show Preview", m_ShowPreview);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Box(texture, GUILayout.Width(texture.width / 2), GUILayout.Height(texture.height / 2));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndToggleGroup();
        }
    }
}