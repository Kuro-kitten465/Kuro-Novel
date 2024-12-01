using System;
using Newtonsoft.Json;
using UnityEngine;

namespace KuroNovel.DataNode
{
    [Serializable]
    public class DialogueNode : VNNode
    {
        public string DialogueText { get; set; }
        public string Speaker { get; set; }
        public string VoiceLine { get; set; }

        public VNCharacter Character { get; set; }

        public DialogueNode() => NodeType = VNNodeType.Dialogue;
    }
}
