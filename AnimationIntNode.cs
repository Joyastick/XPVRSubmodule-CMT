using UnityEngine;
using XNode;

namespace ConversationMatrixTool
{
    [CreateNodeMenu("Conversation Matrix Tool/Animation/Integer")]
    [NodeTint("#726A95")]
    public class AnimationIntNode : AnimationNodeBase
    {
        [Input(backingValue = ShowBackingValue.Never)]
        public Connection input;
        [Tooltip("This integer value will be applied to the selected animator parameter")]
        public int intValue;

        [Output(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override)]
        public Connection output;

        private void Reset()
        {
            name = "Animation Integer";
            type = NodeType.Animation;
            animationType = AnimatorControllerParameterType.Int;
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
                ((ConversationMatrixGraph)graph).IntegerAnimation(parameters[selectedAnim].key, intValue, exitTime);
            else
                ((ConversationMatrixGraph)graph).IntegerAnimation(parameters[selectedAnim].key, intValue);
        }

        public override void Assign()
        {
            ((ConversationMatrixGraph)graph).AssignNode(this);
            NextNode(); //auto jump to the next node
        }
    }
}