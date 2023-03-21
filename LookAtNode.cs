using MyBox;
using UnityEngine;
using XNode;


namespace ConversationMatrixTool
{
    [CreateNodeMenuAttribute("Conversation Matrix Tool/Functional/LookAt")]
    public class LookAtNode : BaseNode
    {
        public Character character;

        [Input(backingValue = ShowBackingValue.Never)]
        public Connection input;

        [Tooltip("set to true if you want to reset previous settings")]
        public bool resetLook; // stare into the void
        [ConditionalField("resetLook", true)] 
        public bool lookAtPlayerCamera = true;

        [ConditionalField("lookAtPlayerCamera", true)]
        public Character target;

        [Output(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override)]
        public Connection output;

        private void Reset()
        {
            name = "Look At";
            type = NodeType.LookAt;
        }

        public override void NextNode()
        {
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