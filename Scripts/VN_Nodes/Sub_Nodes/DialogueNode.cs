using System;
using Newtonsoft.Json;
using UnityEngine;

namespace KuroNovel.DataNode
{
    [Serializable]
    public class DialogueNode : VNNode
    {
        public string Speaker;
        public string DialogueText;
        public AudioClip VoiceLine;

        public CharacterNode Character;

        public DialogueNode() => NodeType = VNNodeType.Dialogue;
    }
}
