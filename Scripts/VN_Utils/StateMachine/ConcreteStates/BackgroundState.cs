using System;
using KuroNovel.DataNode;
using KuroNovel.Manager;
using UnityEngine;

namespace KuroNovel.Utils
{
    public class BackgroundState : IStateMachine
    {
        public void EnterState(VNNode node, Action onComplete)
        {
            Debug.Log($"{this} Enter");
            var n = node as BackgroundNode;
            VNUIManager.Instance.ShowBackground(n, onComplete);
        }

        public void ExitState(VNNode node, Action onComplete)
        {
            Debug.Log($"{this} Exit");
            VNUIManager.Instance.DeactiveSprite();
            VNUIManager.Instance.DeactiveDialogue();
        }

        public void UpdateState(VNNode node, Action onComplete)
        {
            Debug.Log($"{this} Update");
        }
    }
}