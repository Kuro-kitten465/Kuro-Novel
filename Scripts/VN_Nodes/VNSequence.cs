using System;
using System.Collections.Generic;
using UnityEngine;

namespace KuroNovel.DataNode
{
    [Serializable]
    public class VNSequence : ScriptableObject
    {
        public string Title;
        public List<VNNode> Nodes = new List<VNNode>();
    }
}
