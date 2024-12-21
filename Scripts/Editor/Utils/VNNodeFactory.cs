using KuroNovel.DataNode;
using UnityEngine;

namespace KuroNovelEdior.Utils
{
    public static class VNNodeFactory
    {
        public static VNNode CreateNode(VNNodeType type)
        {
            switch (type)
            {
                case VNNodeType.Dialogue:
                    return ScriptableObject.CreateInstance<DialogueNode>();

                case VNNodeType.Choices:
                    return ScriptableObject.CreateInstance<ChoicesNode>();

                case VNNodeType.Sprite:
                    return ScriptableObject.CreateInstance<SpriteNode>();

                case VNNodeType.Background:
                    return ScriptableObject.CreateInstance<BackgroundNode>();

                case VNNodeType.BGM:
                    return ScriptableObject.CreateInstance<BGMNode>();

                case VNNodeType.SFX:
                    return ScriptableObject.CreateInstance<SFXNode>();

                case VNNodeType.Animation:
                    return ScriptableObject.CreateInstance<AnimationNode>();

                case VNNodeType.Video:
                    return ScriptableObject.CreateInstance<VideoNode>();

                case VNNodeType.Live2D:
                    return ScriptableObject.CreateInstance<Live2DNode>();

                case VNNodeType.SpecialEvent:
                    return ScriptableObject.CreateInstance<SpecialEventNode>();

                default:
                    Debug.LogError($"Unknown node type: {type}");
                    return null;
            }
        }
    }
}