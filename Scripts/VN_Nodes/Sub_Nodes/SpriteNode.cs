using System;
using KuroNovel.Utils;
using Newtonsoft.Json;
using UnityEngine;

namespace KuroNovel.DataNode
{
    [Serializable]
    public class SpriteNode : VNNode
    {
        public CharacterNode Character;
        public string Emotion;
        public Sprite CharacterSprite;
        public VNAnimationType InAnimation;
        public VNAnimationType OutAnimation;

        public SpriteNode() => NodeType = VNNodeType.Sprite;
    }
}