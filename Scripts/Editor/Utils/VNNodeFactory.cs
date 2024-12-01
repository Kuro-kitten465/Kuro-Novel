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
                    return new DialogueNode();

                case VNNodeType.Choices:
                    return new ChoicesNode();

                case VNNodeType.Sprite:
                    return new SpriteNode();

                case VNNodeType.Background:
                    return new BackgroundNode();

                case VNNodeType.BGM:
                    return new BGMNode();

                case VNNodeType.SFX:
                    return new SFXNode();

                case VNNodeType.Animation:
                    return new AnimationNode();

                case VNNodeType.Video:
                    return new VideoNode();

                case VNNodeType.Live2D:
                    return new Live2DNode();

                case VNNodeType.SpecialEvent:
                    return new SpecialEventNode();

                default:
                    Debug.LogError($"Unknown node type: {type}");
                    return null;
            }
        }
    }
}