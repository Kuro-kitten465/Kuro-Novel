using UnityEngine;
using KuroNovel.DataNode;
using KuroNovel.Manager;

namespace KuroNovel.StateMachine
{
    public class VNNodeState : IVNState
    {
        private VNNode node;
        private VNStateMachine stateMachine;

        public VNNodeState(VNNode node, VNStateMachine stateMachine)
        {
            this.node = node;
            this.stateMachine = stateMachine;
        }

        public void Enter()
        {
            Debug.Log($"Entering Node: {node.NodeName}");

            if (node is DialogueNode dialogueNode)
            {
                VNUIManager.Instance.ShowDialogue(dialogueNode, () => stateMachine.NextNode());
            }
            else if (node is ChoicesNode choicesNode)
            {
                // Display choice node
                VNUIManager.Instance.ShowChoices(choicesNode, selectedChoice =>
                {
                    stateMachine.NextNode();
                });
            }
            else if (node is BackgroundNode backgroundNode)
            {
                VNUIManager.Instance.ShowBackground(backgroundNode, () => stateMachine.NextNode());
            }
            else if (node is SpriteNode spriteNode)
            {
                VNUIManager.Instance.ShowSprite(spriteNode, () => stateMachine.NextNode());
            }
            else
            {
                // Handle other node types here
                stateMachine.NextNode();
            }
        }

        public void Update()
        {
            if (node is DialogueNode)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                    VNUIManager.Instance.TextEnded = true;
            }
        }

        public void Exit()
        {
            Debug.Log($"Exiting Node: {node.NodeName}");
        }
    }
}