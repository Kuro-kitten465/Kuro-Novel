using System;
using KuroNovel.DataNode;
using KuroNovel.Manager;
using UnityEngine;

namespace KuroNovel.Utils
{
    public class DialogueState : IStateMachine
    {
        public void EnterState(VNNode node, Action onComplete)
        {
            Debug.Log($"{this} Enter");
            var n = node as DialogueNode;
            VNUIManager.Instance.ShowDialogue(n, onComplete);
        }

        public void ExitState(VNNode node, Action onComplete)
        {
            VNUIManager.Instance.DeactiveDialogue();
            Debug.Log($"{this} Exit");
        }

        public void UpdateState(VNNode node, Action onComplete)
        {
            Debug.Log($"{this} Update");
        }
    }
}