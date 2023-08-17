using MyBox;
using UnityEngine;
using XNode;

namespace ConversationMatrixTool
{
    [CreateNodeMenu("Conversation Matrix Tool/Dialogue/Sequence")]
    [NodeWidthAttribute(300)]
    public class SequenceNode : BaseNode
    {
        [Input(backingValue = ShowBackingValue.Never)]
        public Connection input;

        [Tooltip("Sequence to play")] public SequenceSO sequence;
        [ReadOnly()] public string title;

        [Output(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override)]
        public Connection output;

        private void Reset()
        {
            name = "Sequence";
            type = NodeType.Sequence;
            if (sequence != null) title = sequence.title;
        }

        public override void NextNode()
        {
            //play sequence and wait while it is playing
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
        }
    }
}