using UnityEngine;

namespace ConversationMatrixTool
{
    [CreateNodeMenu("Conversation Matrix Tool/Functional/Condition")]
    [NodeTint("#726A95")]
    [NodeWidth(300)]
    public class ConditionNode : BaseNode
    {
        [Input(backingValue = ShowBackingValue.Never)]
        public Connection input;
        [Tooltip("condition with a matching name will be checked when this node is processed")]
        public string conditionKey;
        [Tooltip("if condition is met, this output is used")]
        [Output] public Connection pass;
        [Tooltip("if condition fails, this output is used")]
        [Output] public Connection fail;

        private bool _success;

        private void Reset()
        {
            name = "Condition";
            type = NodeType.Condition;
        }

        public override void NextNode()
        {
            // check checkThis
            var port = GetOutputPort(((ConversationMatrixGraph)graph).CheckCondition(conditionKey) ? "pass" : "fail");
            if (port == null) return;
            for (var i = 0; i < port.ConnectionCount; i++)
            {
                var connection = port.GetConnection(i);
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