using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace KuroNovel.DataNode
{
    [Serializable]
    public class VNCharacters : ScriptableObject
    {
        public List<CharacterNode> Characters { get; set; }

        public VNCharacters() => Characters = new List<CharacterNode>();
    }
}