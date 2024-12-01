using System;
using Newtonsoft.Json;
using UnityEngine;

namespace KuroNovel.DataNode
{
    [Serializable]
    public class SpriteNode : VNNode
    {
        public VNCharacter Character;
        public string CharacterSprite;

        public SpriteNode() => NodeType = VNNodeType.Sprite;
    }
}