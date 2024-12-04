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
        public void OnEnter(VNAnimationType animationType, GameObject obj, Action onComplete = null);
        public void OnUpdate(VNAnimationType animationType, GameObject obj, Action onComplete = null);
        public void OnExit(VNAnimationType animationType, GameObject obj, Action onComplete = null);
    }

    public class AnimationManager : MonoSingleton<AnimationManager>
    {
        public override void Awake()
        {
            base.Awake();
        }

        private const float FADE_DURATION = 1.5f;
        private GameObject m_CurrentObj;
        private VNAnimationType m_CurrentAnimation;

        private Dictionary<VNAnimationType, IVNAnimation> vn_Animations = new();
        private IVNAnimation m_CurrentState = null;

        private void Start()
        {
            vn_Animations.Add(VNAnimationType.FadeIn, new VNAnimationState(m_CurrentAnimation, m_CurrentObj));
            vn_Animations.Add(VNAnimationType.FadeOut, new VNAnimationState(m_CurrentAnimation, m_CurrentObj));
        }

        public void PlayAnimation(VNAnimationType animationType, GameObject obj, Action onAnimationComplete = null)
        {
            ChangeState(animationType);
        }

        private void TransitionState(VNAnimationType animationType, IVNAnimation newState)
        {
            m_CurrentState?.OnExit(animationType, m_CurrentObj);
            m_CurrentState = newState;
            m_CurrentAnimation = animationType;
            m_CurrentState.OnEnter(animationType, m_CurrentObj);
        }

        private void Update()
            => m_CurrentState?.OnUpdate(m_CurrentAnimation, m_CurrentObj);

        public void ChangeState(VNAnimationType animationType)
        {
            IVNAnimation currentState;

            if (vn_Animations.TryGetValue(animationType, out currentState))
            {
                TransitionState(animationType, currentState);
            }
            else
            {
                Debug.LogWarning("No handler found for event type: " + animationType);
            }
        }
    }
}