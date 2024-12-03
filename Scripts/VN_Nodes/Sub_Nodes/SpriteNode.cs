using System;
using Newtonsoft.Json;
using UnityEngine;

namespace KuroNovel.DataNode
{
    [Serializable]
    public class SpriteNode : VNNode
    {
        public CharacterNode Character { get; set; }
        public string Emotion { get; set; }
        public Sprite CharacterSprite { get; set; }

        public SpriteNode() => NodeType = VNNodeType.Sprite;
    }
}