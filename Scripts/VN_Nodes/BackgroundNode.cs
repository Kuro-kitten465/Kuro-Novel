using System;
using Newtonsoft.Json;
using UnityEngine;

namespace KuroNovel.DataNode
{
    [Serializable]
    public class BackgroundNode : VNNode
    {
        public string Background;

        public BackgroundNode() => NodeType = VNNodeType.Background;
    }
}