using System;
using Newtonsoft.Json;
using UnityEngine;

namespace KuroNovel.DataNode
{   
    [Serializable]
    public class VNNode : ScriptableObject
    {
        public string NodeName;
        public VNNodeType NodeType;
    }

    public enum VNNodeType
    {
        Dialogue,
        Choices,
        Sprite,
        Background,
        BGM,
        SFX,
        Animation,
        Video,
        Live2D,
        SpecialEvent      
    }
}
