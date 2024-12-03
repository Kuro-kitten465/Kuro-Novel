using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace KuroNovel.DataNode
{
    [Serializable]
    public class ChoicesNode : VNNode
    {
        public string Prompt { get; set; }
        public List<Choice> Choices { get; set; }
        public bool HideDialogue { get; set; }

        public ChoicesNode()
        {
            NodeType = VNNodeType.Choices;
            Choices = new List<Choice>();
        }
    }

    [Serializable]
    public class Choice
    {
        public string Text { get; set; }
        public string TargetID { get; set; }
    }
}