using KuroNovel.DataNode;
using UnityEngine;

namespace KuroNovelEdior.Utils
{
    public static class VNNodeFactory
    {
        public static VNNode CreateNode(VNNodeType type, bool isJson)
        {
            switch (type)
            {
                case VNNodeType.Dialogue:
                    return isJson ? new DialogueNode() :
                    ScriptableObject.CreateInstance<DialogueNode>();

                case VNNodeType.Choices:
                    return isJson ? new ChoicesNode() :
                    ScriptableObject.CreateInstance<ChoicesNode>();

                case VNNodeType.Sprite:
                    return isJson ? new SpriteNode() :
                    ScriptableObject.CreateInstance<SpriteNode>();

                case VNNodeType.Background:
                    return isJson ? new BackgroundNode() :
                    ScriptableObject.CreateInstance<BackgroundNode>();

                case VNNodeType.BGM:
                    return isJson ? new BGMNode() :
                    ScriptableObject.CreateInstance<BGMNode>();

                case VNNodeType.SFX:
                    return isJson ? new SFXNode() :
                    ScriptableObject.CreateInstance<SFXNode>();

                case VNNodeType.Animation:
                    return isJson ? new AnimationNode() :
                    ScriptableObject.CreateInstance<AnimationNode>();

                case VNNodeType.Video:
                    return isJson ? new VideoNode() :
                    ScriptableObject.CreateInstance<VideoNode>();

                case VNNodeType.Live2D:
                    return isJson ? new Live2DNode() :
                    ScriptableObject.CreateInstance<Live2DNode>();

                case VNNodeType.SpecialEvent:
                    return isJson ? new SpecialEventNode() :
                    ScriptableObject.CreateInstance<SpecialEventNode>();

                default:
                    Debug.LogError($"Unknown node type: {type}");
                    return null;
            }
        }
    }
}