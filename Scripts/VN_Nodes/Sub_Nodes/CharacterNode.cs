using System;
using System.Collections.Generic;
using UnityEngine;

namespace KuroNovel.DataNode
{
    [Serializable]
    public class CharacterNode : ScriptableObject
    {
        public string CharacterName { get; set; }
        public List<EmotionsNode> Emotions { get; set; }
        public string Info { get; set; }

        public CharacterNode() => Emotions = new List<EmotionsNode>();

        public int EmotionsIndexOf(string s)
        {
            for (int i = 0; i < Emotions.Count; i++)
            {
                if (Emotions[i].Emotion.Equals(s))
                    return i;
            }

            return -1;
        }
    }

    [Serializable]
    public class EmotionsNode
    {
        public string Emotion { get; set; }
        public Sprite Sprite { get; set; }
    }
}