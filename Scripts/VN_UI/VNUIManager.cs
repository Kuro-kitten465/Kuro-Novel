using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using KuroNovel.DataNode;
using KuroNovel.Utils;
using System.Collections.Generic;
using System;
using System.Linq;

namespace KuroNovel.Manager
{
    public class VNUIManager : MonoSingleton<VNUIManager>
    {
        public enum VNObjectReference
        {
            Background, SpriteCenter, SpriteLeft, SpriteRight
        }

        [Header("VN Panel & Canvas Properties")]
        public CanvasGroup vn_GroupPanel;
        public Canvas vn_BackgroundCanvas;
        public Canvas vn_SpriteCanvas;
        public Canvas vn_DialogueCanvas;
        public Canvas vn_ChoicesCanvas;

        [Header("Dialogue Propeties")]
        public TextMeshProUGUI vn_DialogueText;
        public Image vn_DialogueBackground;
        public TextMeshProUGUI vn_SpeakerText;
        public Image vn_SpeakerBackground;

        [Header("Choices Properties")]
        public GameObject vn_ChoiceButtonTemplate;
        public VerticalLayoutGroup vn_ChoicesLayoutGroup;

        [Header("Background Properties")]
        public Image vn_Background;

        [Header("Sprite Properties")]
        public GameObject[] vn_Sprites = new GameObject[3];
        public HorizontalLayoutGroup vn_SpriteLayoutGroup;

        private List<GameObject> m_ActiveButtons = new();
        private List<GameObject> m_ActiveSprites = new();
        private Dictionary<VNObjectReference, GameObject> m_Objects = new();

        private VNAnimationState m_AnimationState;

        public override void Awake()
        {
            base.Awake();

            //m_Objects.Add(VNObjectReference.Background, vn_Background.gameObject);
            //m_Objects.Add(VNObjectReference.SpriteCenter, vn_Sprites[0].gameObject);
            //m_Objects.Add(VNObjectReference.SpriteLeft, vn_Sprites[1].gameObject);
            //m_Objects.Add(VNObjectReference.SpriteRight, vn_Sprites[2].gameObject);

            //m_ActiveSprites = vn_Sprites.ToList();
        }

        #region Dialogue Handler
        public void ShowDialogue(DialogueNode dialogueNode, Action onComplete)
        {
            vn_DialogueText.text = dialogueNode.DialogueText;
            vn_SpeakerText.text = dialogueNode.Speaker;

            vn_DialogueCanvas.gameObject.SetActive(true);
        }

        public void DeactiveDialogue() =>
            vn_DialogueCanvas.gameObject.SetActive(false);

        //public bool TextEnded = false;

        /*private IEnumerator WaitForInput(string text, Action onComplete)
        {
            for (int i = 0; i < text.Length; i++)
            {
                if (vn_DialogueText.text != text && !TextEnded)
                {
                    vn_DialogueText.text += text[i];
                    yield return new WaitForSeconds(0.1f);
                }
                else
                {
                    TextEnded = true;
                    vn_DialogueText.text = text;
                    break;
                }
            }

            while (!Input.GetKeyDown(KeyCode.Space))
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.1f); //DUNNO

            // Hide the dialogue panel and invoke the completion callback
            vn_DialoguePanel.SetActive(false);
            onComplete?.Invoke();
        }*/

        #endregion
        #region Choices Handler

        public void ShowChoices(ChoicesNode choicesNode, Action<int> onChoiceSelected)
        {
            ClearChoices();

            for (int i = 0; i < choicesNode.Choices.Count; i++)
            {
                var choiceText = choicesNode.Choices[i].Text;
                var choiceIndex = i;

                var buttonObject = Instantiate(vn_ChoiceButtonTemplate, vn_ChoicesLayoutGroup.transform);
                buttonObject.SetActive(true);

                var button = buttonObject.GetComponent<Button>();
                var buttonText = buttonObject.GetComponentInChildren<TextMeshProUGUI>();

                if (buttonText != null)
                    buttonText.text = choiceText;

                button.onClick.AddListener(() =>
                {
                    onChoiceSelected?.Invoke(choiceIndex);
                    ShowChoices(false);
                });

                m_ActiveButtons.Add(buttonObject);
            }

            ShowChoices(true);
        }

        private void ClearChoices()
        {
            foreach (var button in m_ActiveButtons)
            {
                Destroy(button);
            }

            m_ActiveButtons.Clear();
        }

        private void ShowChoices(bool set)
        {
            vn_ChoicesLayoutGroup.gameObject.SetActive(set);
        }
        #endregion
        #region BG Handler
        public void ShowBackground(BackgroundNode backgroundNode, Action onComplete)
        {
            if (backgroundNode == null)
            {
                Debug.LogError($"Background sprite not found");
                onComplete?.Invoke();
                return;
            }

            vn_Background.sprite = backgroundNode.Background;
            vn_BackgroundCanvas.gameObject.SetActive(true);
        }

        public void DeactiveBackground() => vn_BackgroundCanvas.gameObject.SetActive(false);
        #endregion
        #region Sprite Handler
        public void ShowSprite(SpriteNode spriteNode, Action onComplete)
        {
            /*if (spriteNode.CharacterSprite == null)
            {
                Debug.LogError($"Background sprite not found");
                onComplete?.Invoke();
                return;
            }*/

            var sprite = vn_Sprites[0].GetComponent<Image>();
            sprite.sprite = spriteNode.CharacterSprite;

            vn_Sprites[0].gameObject.SetActive(true);
        }

        public void DeactiveSprite() => vn_Sprites[0].gameObject.SetActive(false);
        #endregion
    }
}