using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace KuroNovel.DataNode
{
    [Serializable]
    public class VNCharacter
    {
        [JsonProperty("name")]
        public string CharacterName { get; set; }
        [JsonProperty("emotions")]
        public Dictionary<string, string> Emotions { get; set; }
        [JsonProperty("info")]
        public string Info { get; set; }

        public VNCharacter() => Emotions = new Dictionary<string, string>();
    }
}