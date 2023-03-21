using UnityEngine;
using XNode;

namespace ConversationMatrixTool
{
    [CreateNodeMenu("Conversation Matrix Tool/Animation/Float")]
    [NodeTint("#726A95")]
    public class AnimationFloatNode : AnimationNodeBase
    {
        [Input(backingValue = ShowBackingValue.Never)]
        public Connection input;
        [Tooltip("This float value will be applied to the selected animator parameter")]
        public float floatValue;

        [Output(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override)]
        public Connection output;

        private void Reset()
        {
            name = "Animation Float";
            type = NodeType.Animation;
            animationType = AnimatorControllerParameterType.Float;
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
                ((ConversationMatrixGraph)graph).FloatAnimation(parameters[selectedAnim].key, floatValue, exitTime);
            else
                ((ConversationMatrixGraph)graph).FloatAnimation(parameters[selectedAnim].key, floatValue);
        }

        public override void Assign()
        {
            ((ConversationMatrixGraph)graph).AssignNode(this);
            NextNode(); //auto jump to the next node
        }
    }
}