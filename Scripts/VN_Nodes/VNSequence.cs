using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace KuroNovel.DataNode
{
    [Serializable]
    public class VNSequence : ScriptableObject
    {
        public string Title { get; set; }
        public List<VNNode> Nodes { get; set; }

        public VNSequence() => Nodes = new List<VNNode>();
    }
}
