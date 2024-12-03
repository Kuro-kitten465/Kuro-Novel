using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace KuroNovel.DataNode
{
    [Serializable]
    public class VNCharacters : ScriptableObject
    {
        public List<CharacterNode> Characters = new List<CharacterNode>();
    }
}