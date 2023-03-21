using UnityEngine;
using XNodeEditor;

namespace ConversationMatrixTool.Editor
{
    [CustomNodeEditor(typeof(StatementNode))]
    public class StatementNodeEditor : NodeEditor
    {
        public override Color GetTint() //changes colour of the statement node based on the character colour
        {
            var col = base.GetTint();
            StatementNode node = target as StatementNode;

            if (node != null)
                if (node.character != null)
                    col = node.character.colour;

            col.a = 1;
            return col;
        }
    }
}