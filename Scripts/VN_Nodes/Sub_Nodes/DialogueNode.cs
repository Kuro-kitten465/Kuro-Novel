using System;
using Newtonsoft.Json;
using UnityEngine;

namespace KuroNovel.DataNode
{
    [Serializable]
    public class DialogueNode : VNNode
    {
        public string Speaker { get; set; }
        public string DialogueText { get; set; }
        public AudioClip VoiceLine { get; set; }

        public CharacterNode Character { get; set; }

        public DialogueNode() => NodeType = VNNodeType.Dialogue;
    }
}
