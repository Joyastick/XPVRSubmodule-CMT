using UnityEngine;
using XNode;

namespace ConversationMatrixTool
{
    [CreateNodeMenu("Conversation Matrix Tool/Animation/Trigger")]
    [NodeTint("#726A95")]
    public class AnimationTriggerNode : AnimationNodeBase
    {
        [Input(backingValue = ShowBackingValue.Never)]
        public Connection input;

        [Output(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override)]
        public Connection output;

        private void Reset()
        {
            name = "Animation Trigger";
            type = NodeType.Animation;
            animationType = AnimatorControllerParameterType.Trigger;
        }

        public override void NextNode()
        {
            ProcessRequest();
            NodePort port = GetOutputPort("output");
            if (port == null) return;
            for (int i = 0; i < port.ConnectionCount; i++)
            {
                NodePort connection = port.GetConnection(i);
                (connection.node as BaseNode)?.Assign();
            }
        }

        private void ProcessRequest()
        {
            //Debug.Log("Selected anim: " + selectedAnim);
            if (selectedAnim < 0) return;
            if (parameters == null) return;
            if (selectedAnim < parameters.Length)
                if (hasExitTime)
                    ((ConversationMatrixGraph)graph).TriggerAnimation(parameters[selectedAnim].key, exitTime);
                else
                    ((ConversationMatrixGraph)graph).TriggerAnimation(parameters[selectedAnim].key);
        }

        public override void Assign()
        {
            ((ConversationMatrixGraph)graph).AssignNode(this);
            NextNode(); //auto jump to the next node
        }
    }
}