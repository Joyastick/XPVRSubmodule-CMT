using System;
using MyBox;
using UnityEngine;
using XNode;

namespace ConversationMatrixTool
{
    [CreateNodeMenu("Conversation Matrix Tool/Dialogue/Statement")]
    [NodeTint("#726A95")]
    [NodeWidth(333)]
    public class StatementNode : BaseNode
    {
        [Tooltip("Character that makes the statement")]
        public Character character;

        [Input(backingValue = ShowBackingValue.Never)]
        public Connection input;

        [Tooltip("Set this to true if you want to skip to the next node after the statement is complete")]
        public bool autoSkip = true;

        
        [TextArea(6, 66)] 
        [Tooltip("statement text")]
        public string text;

        [Tooltip("Statement audio recording")]
        public AudioClip audio;

        [Output(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override)]
        public Connection output;

        private void Reset()
        {
            name = "Statement";
            type = NodeType.Statement;
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

        public string NextNodeTriggerInfo()
        {
            NodePort port = GetOutputPort("output");
            if (port == null) return null;
            NodePort connection = port.GetConnection(0);
            if (connection.node is AnimationTriggerNode)
            {
                AnimationTriggerNode temp;
                temp = connection.node as AnimationTriggerNode;
                return temp.parameters[temp.selectedAnim].key;
            }

            return "Idle";
        }

        public override void Assign()
        {
            ((ConversationMatrixGraph)graph).AssignNode(this);
        }
    }
}