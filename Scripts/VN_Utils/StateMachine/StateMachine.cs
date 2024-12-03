using System;
using System.Collections.Generic;
using KuroNovel.DataNode;
using UnityEngine;

namespace KuroNovel.Utils
{
    public interface IStateMachine
    {
        public void EnterState(VNNode node, Action onComplete);
        public void UpdateState(VNNode node, Action onComplete);
        public void ExitState(VNNode node, Action onComplete);
    }

    public class StateMachine
    {
        public IStateMachine CurrentState => m_currentState;

        private IStateMachine m_currentState = null;
        private Enum m_stateType;
        private Dictionary<Enum, IStateMachine> m_states = new Dictionary<Enum, IStateMachine>();
        public VNSequence m_Sequence { get; private set; }
        public Action m_Action { get; private set; }

        public StateMachine(Dictionary<Enum, IStateMachine> states, VNSequence sequence, Action onComplete)
        {
            m_states = states;
            m_Sequence = sequence;
            m_Action = onComplete;
        }

        private void TransitionState(Enum stateType, IStateMachine newState, VNNode node)
        {
            m_currentState?.ExitState(node, m_Action);
            m_currentState = newState;
            m_stateType = stateType;
            m_currentState.EnterState(node, m_Action);
        }

        public void OnState(VNNode node)
        {
            m_currentState?.UpdateState(node, m_Action);
        }

        public void ChangeState(VNNode node)
        {
            IStateMachine currentState;

            if (m_states.TryGetValue(node.NodeType, out currentState))
            {
                TransitionState(node.NodeType, currentState, node);
            }
            else
            {
                Debug.LogWarning("No handler found for event type: " + node.NodeType);
            }
        }
    }
}