using XNode;

namespace ConversationMatrixTool
{
    [CreateNodeMenu("Conversation Matrix Tool/Functional/Start")]
    [NodeTint("#726A95")]
    public class StartNode : BaseNode
    {
        [Output(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override)]
        public Connection output;

        private void Reset()
        {
            name = "Start";
            type = NodeType.Start;
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
        }
    }
}