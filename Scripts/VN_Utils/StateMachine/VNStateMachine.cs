using UnityEngine;
using KuroNovel.Utils;
using System.Collections.Generic;
using System;
using TMPro;
using KuroNovel.DataNode;

namespace KuroNovel.Manager
{
    public class VNStateMachine : MonoSingleton<VNStateMachine>
    {
        private StateMachine m_StateMachine;
        private Dictionary<Enum, IStateMachine> m_States = new Dictionary<Enum, IStateMachine>();

        public VNSequence m_Sequence;
        [SerializeField] private TextMeshProUGUI textMeshProUGUI;

        public override void Awake()
        {
            base.Awake();

            m_States.Add(VNNodeType.Dialogue, new DialogueState());
            m_States.Add(VNNodeType.Choices, new ChoicesState());
            m_States.Add(VNNodeType.Sprite, new SpriteState());
            m_States.Add(VNNodeType.Background, new BackgroundState());
        }

        int i = 0;

        private void OnGUI()
        {
            textMeshProUGUI.text = $"sequence : {isNull} | statemac : {isNull1} | {m_Sequence.Nodes.Count}";

            if (GUILayout.Button("Start"))
            {
                m_StateMachine = new StateMachine(m_States, m_Sequence, OnComplete);
                m_StateMachine.ChangeState(m_Sequence.Nodes[i]);
            }

            if (m_StateMachine != null)
                if (m_StateMachine.CurrentState != null)
                GUILayout.Label(m_StateMachine.CurrentState.ToString());
        }

        bool isNull = false;
        bool isNull1 = false;

        private void OnComplete()
        {
            i++;
            if (i == m_Sequence.Nodes.Count) i = 0;

            m_StateMachine.ChangeState(m_Sequence.Nodes[i]);
        }

        private void Update()
        {
            isNull = m_Sequence == null ? true : false;
            isNull1 = m_StateMachine == null ? true : false;

            if (m_StateMachine != null)
                if (m_StateMachine.CurrentState != null &&
                    m_StateMachine.CurrentState == m_States[VNNodeType.Dialogue])
                    m_StateMachine?.OnState(m_Sequence.Nodes[i]);
        }
    }
}
