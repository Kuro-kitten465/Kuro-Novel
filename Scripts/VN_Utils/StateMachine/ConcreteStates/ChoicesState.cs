using System;
using KuroNovel.DataNode;
using KuroNovel.Manager;
using UnityEngine;

namespace KuroNovel.Utils
{
    public class ChoicesState : IStateMachine
    {
        public void EnterState(VNNode node, Action onComplete)
        {
            Debug.Log($"{this} Enter");
            var n = node as ChoicesNode;
            /*VNUIManager.Instance.ShowChoices(n, selectedChoice =>
            {
                    onComplete.Invoke();
            });*/
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