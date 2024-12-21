using System;
using UnityEngine;

namespace KuroNovel.DataNode
{
    [Serializable]
    public class BGMNode : VNNode
    {
        public AudioClip BGM;

        public BGMNode() => NodeType = VNNodeType.BGM;
    }
}