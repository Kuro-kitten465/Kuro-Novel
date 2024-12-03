using System;
using KuroNovel.DataNode;
using KuroNovel.Manager;
using UnityEngine;

namespace KuroNovel.Utils
{
    public class SpriteState : IStateMachine
    {
        public void EnterState(VNNode node, Action onComplete)
        {
            Debug.Log($"{this} Enter");
            var n = node as SpriteNode;
            VNUIManager.Instance.ShowSprite(n, onComplete);
        }

        public void ExitState(VNNode node, Action onComplete)
        {
            Debug.Log($"{this} Exit");
        }

        public void UpdateState(VNNode node, Action onComplete)
        {
            Debug.Log($"{this} Update");
        }
    }
}