using System;
using Newtonsoft.Json;
using UnityEngine;

namespace KuroNovel.DataNode
{
    [Serializable]
    public class AnimationNode : VNNode
    {
        public string PlayAnimation;

        public AnimationNode() => NodeType = VNNodeType.Animation;
    }
}