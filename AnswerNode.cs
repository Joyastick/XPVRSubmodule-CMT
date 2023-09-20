using System;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace ConversationMatrixTool
{
    [CreateNodeMenu("Conversation Matrix Tool/Dialogue/Answer")]
    [NodeTint("#726A95")]
    [NodeWidth(333)]
    public class AnswerNode : BaseNode
    {
        [Input] public Connection input;
        [Space(6.66f)] public bool copyPrevious;
        [TextArea(6, 66)] public string question;

        [Tooltip("Answers will be displayed in random order each time this node is loaded, if this is set to true.")]
        public bool randomize;

        [Output(dynamicPortList = true, backingValue = ShowBackingValue.Never,
            typeConstraint = TypeConstraint.None,
            connectionType = ConnectionType.Override)]
        public List<Answer> output = new List<Answer>();

        private int givenAnswer;

        private void Reset()
        {
            name = "Question - Answer";
            type = NodeType.Answer;
            CheckPreviousNode();
        }

        private void OnValidate()
        {
            base.OnValidate();
            CheckPreviousNode();
        }

        private void OnEnable()
        {
            base.OnEnable();
            CheckPreviousNode();
        }

        private void CheckPreviousNode()
        {
            if (!copyPrevious) return;
            name = "Answer";
            type = NodeType.Answer;
            var port = GetInputPort("input");
            if (port == null) return;
            if (port.ConnectionCount == 0) return;
            NodePort connection = port.GetConnection(0);
            if (((BaseNode)connection.node).type == NodeType.Statement)
                question = ((StatementNode)connection.node).text;
        }

        public void AnswerQuestion(int i)
        {
            givenAnswer = i;
            NextNode();
        }

        public override void NextNode()
        {
            NodePort port = null;
            if (output.Count <= givenAnswer) return;
            port = GetOutputPort("output " + givenAnswer);
//            //Debug.Log("Retrieved answer port: " + port);
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

    [Serializable]
    public class Answer
    {
        [TextArea(6, 66)] public string text;
        public bool isActive = true;

        public Answer()
        {
            isActive = true;
        }
    }
}