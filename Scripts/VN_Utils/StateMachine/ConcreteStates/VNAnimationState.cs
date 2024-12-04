using System;
using KuroNovel.DataNode;
using UnityEngine;

namespace KuroNovel.Utils
{
    public class VNAnimationState : IVNAnimation
    {
        public float ActionTime => m_ActionTime;
        private float m_ActionTime = 0;
        private VNAnimationType m_AnimationType;
        private GameObject m_CurrentObj;

        public VNAnimationState(VNAnimationType animationType, GameObject obj)
        {
            m_AnimationType = animationType;
            m_CurrentObj = obj;
        }
        
        public void OnEnter(VNAnimationType animationType, GameObject obj, Action onComplete = null)
        {
            
        }

        public void OnUpdate(VNAnimationType animationType, GameObject obj, Action onComplete = null)
        {
            
        }

        public void OnExit(VNAnimationType animationType, GameObject obj, Action onComplete = null)
        {
            throw new NotImplementedException();
        }
    }
}