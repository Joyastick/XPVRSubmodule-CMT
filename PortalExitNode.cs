using UnityEngine;
using XNode;

namespace ConversationMatrixTool
{
    [CreateNodeMenu("Conversation Matrix Tool/Functional/Portal Exit")]
    [NodeTint("#726A95")]
    public class PortalExitNode : PortalNodeBase
    {
        [Output(backingValue = ShowBackingValue.Never)]
        public Connection output;
        [Tooltip("title of the portal")]
        public string key;

        private void Reset()
        {
            type = NodeType.Exit;
            name = "Portal Exit";
        }

        public override void NextNode()
        {
            //grab the output port
            var port = GetOutputPort("output");
            if (port == null) return;
            for (var i = 0; i < port.ConnectionCount; i++)
            {
                //get the node that is connected to this output port
                var connection = port.GetConnection(i);
                //assign the connected node to the graph player as the new current node 
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