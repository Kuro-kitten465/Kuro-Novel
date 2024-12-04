using System;
using System.Collections;
using System.Collections.Generic;
using KuroNovel.DataNode;
using UnityEngine;
using UnityEngine.UI;

namespace KuroNovel.Utils
{
    public enum VNAnimationType
    {
        None, FadeIn, FadeOut, Jumping, Shake
    }

    public interface IVNAnimation
    {
        public void ExecuteAnimation(VNAnimationType animationType, GameObject obj, Action onComplete = null);
    }

    public class AnimationManager : MonoSingleton<AnimationManager>
    {
        private Dictionary<VNAnimationType, IVNAnimation> m_Animations = new();
        private IVNAnimation m_CurrentAnimation;
        private VNAnimationType m_CurrentAnimationType;
        private GameObject obj;

        public override void Awake()
        {
            base.Awake();

            m_Animations.Add(VNAnimationType.FadeIn, new FadeAnim());
            m_Animations.Add(VNAnimationType.FadeOut, new FadeAnim());
        }

        public void PlayAnimation(VNAnimationType animationType, GameObject obj, Action onAnimationComplete = null)
        {
            m_CurrentAnimationType = animationType;
            Debug.Log("PlayAnim: " + animationType);

            if (m_Animations.TryGetValue(animationType, out m_CurrentAnimation))
            {
                this.obj = obj;
                m_CurrentAnimation.ExecuteAnimation(m_CurrentAnimationType, obj, onAnimationComplete);
            }
            else
            {
                Debug.LogWarning("No handler found for event type: " + animationType);
            }
        }
    }
}