using UnityEngine;
using KuroNovel.DataNode;

namespace KuroNovel.StateMachine
{
    public interface IVNState
    {
        public void Enter();
        public void Update();
        public void Exit();
    }

    public class VNStateMachine 
    {
        private VNSequence sequence;
        private int currentNodeIndex = 0;
        private IVNState currentState;

        public VNStateMachine(VNSequence sequence)
        {
            this.sequence = sequence;
        }

        public void Start()
        {
            if (sequence.Nodes.Count > 0)
            {
                SetState(new VNNodeState(sequence.Nodes[currentNodeIndex], this));
            }
        }

        public void SetState(IVNState newState)
        {
            currentState?.Exit();
            currentState = newState;
            currentState.Enter();
        }

        public void NextNode()
        {
            currentNodeIndex++;
            if (currentNodeIndex < sequence.Nodes.Count)
            {
                SetState(new VNNodeState(sequence.Nodes[currentNodeIndex], this));
            }
            else
            {
                Debug.Log("VN Sequence Completed!");
            }
        }

        public void Update()
        {
            currentState?.Update();
        }
    }
}
