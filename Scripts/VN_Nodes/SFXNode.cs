using System;
using Newtonsoft.Json;
using UnityEngine;

namespace KuroNovel.DataNode
{
    [Serializable]
    public class SFXNode : VNNode
    {
        public string SFX;

        public SFXNode() => NodeType = VNNodeType.SFX;
    }
}