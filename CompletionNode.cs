using UnityEngine;

namespace ConversationMatrixTool
{
    [CreateNodeMenu("Conversation Matrix Tool/Functional/End")]
    [NodeTint("#726A95")]
    public class CompletionNode : BaseNode
    {
        [Tooltip("CMT event with a matching key will be invoked at end")]
        public string finalEventKey;
        [Input(backingValue = ShowBackingValue.Never)]
        public Connection input;

        private void Reset()
        {
            name = "End";
            type = NodeType.End;
        }

        public override void NextNode()
        {
            // this is the end my friend
        }

        public override void Assign()
        {
            ((ConversationMatrixGraph)graph).AssignNode(this);
        }
    }
}