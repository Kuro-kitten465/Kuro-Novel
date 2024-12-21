using UnityEngine;
using KuroNovel.Utils;
using System.Collections.Generic;
using System;
using KuroNovel.DataNode;

namespace KuroNovel.Manager
{
    public class VNStateMachine : MonoSingleton<VNStateMachine>
    {
        [SerializeField] VNSequence m_Sequence;
        private StateMachine m_StateMachine;
        private Dictionary<Enum, IStateMachine> m_States = new Dictionary<Enum, IStateMachine>();
        private bool m_Running = false;

        public override void Awake()
        {
            base.Awake();

            m_States.Add(VNNodeType.Dialogue, new DialogueState());
            m_States.Add(VNNodeType.Choices, new ChoicesState());
            m_States.Add(VNNodeType.Sprite, new SpriteState());
            m_States.Add(VNNodeType.Background, new BackgroundState());
        }

        int m_CurrentStateIndex = 0;

        private void OnGUI()
        {
            if (!m_Running)
            if (GUILayout.Button("Start"))
            {
                m_StateMachine = new StateMachine(m_States, m_Sequence, OnComplete);
                m_StateMachine.ChangeState(m_Sequence.Nodes[m_CurrentStateIndex]);
                m_Running = true;
            }
            
            if (m_Running)
            if (GUILayout.Button("Next State"))
            {
                m_CurrentStateIndex++;
                if (m_CurrentStateIndex >= m_Sequence.Nodes.Count || m_CurrentStateIndex < 0)
                {
                    m_CurrentStateIndex = 0;
                    VNUIManager.Instance.DeactiveBackground();
                }

                m_StateMachine.ChangeState(m_Sequence.Nodes[m_CurrentStateIndex]);
            }
        }

        private void OnComplete()
        {

        }

        /*private void Update()
        {
            isNull = m_Sequence == null ? true : false;
            isNull1 = m_StateMachine == null ? true : false;

            if (m_StateMachine != null)
                if (m_StateMachine.CurrentState != null &&
                    m_StateMachine.CurrentState == m_States[VNNodeType.Dialogue])
                    m_StateMachine?.OnState(m_Sequence.Nodes[m_CurrentStateIndex]);
        }*/
    }
}
