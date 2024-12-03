using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace KuroNovel.DataNode
{
    [Serializable]
    public class ChoicesNode : VNNode
    {
        public string Prompt;
        public List<Choice> Choices = new List<Choice>();
        public bool HideDialogue;

        public ChoicesNode()
        {
            NodeType = VNNodeType.Choices;
        }
    }

    [Serializable]
    public class Choice
    {
        public string Text;
        public string TargetID;
    }
}