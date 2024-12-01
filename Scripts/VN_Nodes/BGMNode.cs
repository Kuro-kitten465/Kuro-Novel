using System;
using Newtonsoft.Json;
using UnityEngine;

namespace KuroNovel.DataNode
{
    [Serializable]
    public class BGMNode : VNNode
    {
        public string BGM;

        public BGMNode() => NodeType = VNNodeType.BGM;
    }
}