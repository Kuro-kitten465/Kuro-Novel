using System;
using Newtonsoft.Json;
using UnityEngine;
using KuroNovel.Utils;

namespace KuroNovel.DataNode
{
    [Serializable]
    public class BackgroundNode : VNNode
    {
        public Sprite Background;
        public VNAnimationType InAnimation;
        public VNAnimationType OutAnimation;

        public BackgroundNode() => NodeType = VNNodeType.Background;
    }
}