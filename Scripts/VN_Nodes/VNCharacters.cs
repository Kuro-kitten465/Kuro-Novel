using System;
using System.Collections.Generic;
using UnityEngine;

namespace KuroNovel.DataNode
{
    [Serializable]
    public class VNCharacters : ScriptableObject
    {
        public List<CharacterNode> Characters = new List<CharacterNode>();
    }
}