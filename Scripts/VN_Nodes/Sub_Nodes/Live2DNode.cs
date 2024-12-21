using System;
using UnityEngine;

namespace KuroNovel.DataNode
{
    [Serializable]
    public class Live2DNode : VNNode
    {
        public string Live2D;

        public Live2DNode() => NodeType = VNNodeType.Live2D;
    }
}