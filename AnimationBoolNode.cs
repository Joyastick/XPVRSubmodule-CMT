using UnityEngine;
using XNode;

namespace ConversationMatrixTool
{
    [CreateNodeMenu("Conversation Matrix Tool/Animation/Boolean")]
    [NodeTint("#726A95")]
    public class AnimationBoolNode : AnimationNodeBase
    {
        [Input(backingValue = ShowBackingValue.Never)]
        public Connection input;

        [Tooltip("This state will be applied to the selected animator parameter")]
        public bool booleanState;

        [Output(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override)]
        public Connection output;

        private void Reset()
        {
            name = "Animation Boolean";
            type = NodeType.Animation;
            animationType = AnimatorControllerParameterType.Bool;
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
            if (hasExitTime)
                ((ConversationMatrixGraph)graph).BoolAnimation(parameters[selectedAnim].key, booleanState, exitTime);
            else
                ((ConversationMatrixGraph)graph).BoolAnimation(parameters[selectedAnim].key, booleanState);
        }

        public override void Assign()
        {
            ((ConversationMatrixGraph)graph).AssignNode(this);
            NextNode(); //auto jump to the next node
        }
    }
}