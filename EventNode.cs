using UnityEngine;
using XNode;

namespace ConversationMatrixTool
{
    [CreateNodeMenu("Conversation Matrix Tool/Functional/Event")]
    [NodeTint("#726A95")]
    [NodeWidth(256)]
    public class EventNode : BaseNode
    {
        [Input(backingValue = ShowBackingValue.Never)]
        public Connection input;
        [Tooltip("CMT event with a matching key will be invoked when this node is processed")]
        public string eventKey;
        [Output(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override)]
        public Connection output;

        private void Reset()
        {
            name = "Event";
            type = NodeType.Event;
        }

        public override void NextNode()
        {
            ((ConversationMatrixGraph)graph).InvokeEvent(eventKey);
            NodePort port = GetOutputPort("output");
            if (port == null) return;
            for (int i = 0; i < port.ConnectionCount; i++)
            {
                NodePort connection = port.GetConnection(i);
                (connection.node as BaseNode)?.Assign();
            }
        }

        public override void Assign()
        {
            ((ConversationMatrixGraph)graph).AssignNode(this);
            NextNode(); //auto jump to the next node
        }
    }
}