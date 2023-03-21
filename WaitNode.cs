namespace ConversationMatrixTool
{
    [CreateNodeMenu("Conversation Matrix Tool/Functional/Wait")]
    //[NodeWidth(300)]
    public class WaitNode : BaseNode
    {
        [Input(backingValue = ShowBackingValue.Never)]
        public Connection input;

        //if this is true, the wait can be cancelled via the graph player
        public bool cancellable = true;
        //duration of the wait
        public float seconds;
        [Output(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override)]
        public Connection output;

        
        private void Reset()
        {
            name = "Wait";
            type = NodeType.Wait;
        }
        
        //go to next node
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

        //this method assigns the node to the graph player as the new current node
        public override void Assign()
        {
            ((ConversationMatrixGraph) graph).AssignNode(this);
        }
    }
}