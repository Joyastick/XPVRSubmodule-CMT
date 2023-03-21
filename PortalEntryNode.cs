using UnityEngine;

namespace ConversationMatrixTool
{
    [CreateNodeMenu("Conversation Matrix Tool/Functional/Portal Entry")]
    [NodeTint("#726A95")]
    public class PortalEntryNode : PortalNodeBase
    {
        [Input(backingValue = ShowBackingValue.Never)]
        public Connection input;
        
        [Tooltip("title of the target exit portal")]
        public string key;

        //this method searches for all the portal exits in the graph and finds one with the given title
        public BaseNode FindExit(string target)
        {
            if (target == null) return null;
            if (target.Length <= 0) return null;
            foreach (var node in ((ConversationMatrixGraph)graph).nodes)
                if (((BaseNode)node).type == NodeType.Exit)
                    if (((PortalExitNode)node).key == target)
                        return (BaseNode)node;

            if (target == "Start" || target == "start" || target == "START")
                foreach (var node in ((ConversationMatrixGraph)graph).nodes)
                    if (((BaseNode)node).type == NodeType.Start)
                        return (BaseNode)node;

            return null;
        }

        private void Reset()
        {
            type = NodeType.Entry;
            name = "Portal Entry";
        }

        public override void NextNode()
        {
            FindExit(key).Assign();
        }

        public override void Assign()
        {
            ((ConversationMatrixGraph)graph).AssignNode(this);
            NextNode(); //auto jump to the next node
        }
    }
}