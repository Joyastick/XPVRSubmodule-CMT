using System.Collections.Generic;
using MyBox;
using XNode;
#if UNITY_EDITOR
using XNodeEditor;
#endif

namespace ConversationMatrixTool
{
    [CreateNodeMenuAttribute("Conversation Matrix Tool/Dialogue/QuestionPool")]
    //[NodeTintAttribute("#726A95")]
    [NodeWidthAttribute(666)]
    public class QuestionPoolNode : BaseNode
    {
        [Input] public Connection input;
        public QuestionPool pool;

        [Output(dynamicPortList = true, backingValue = ShowBackingValue.Never,
            typeConstraint = TypeConstraint.None,
            connectionType = ConnectionType.Override)]
        public List<PoolQuestion> output;

        private int questionIndex;

        private const int maxOptions = 10;

        private void Reset()
        {
            name = "QuestionPool";
            type = NodeType.QuestionPool;
            GetQuestions();
        }

        public void AskQuestion(int i)
        {
            questionIndex = i;
            NextNode();
        }

        private new void OnValidate()
        {
            base.OnValidate();
            GetQuestions();
        }

        public void GetQuestions()
        {
            if (output == null)
                output = new List<PoolQuestion>();
            else
                output.Clear();

            ClearDynamicPorts();
#if UNITY_EDITOR
            NodeEditorGUILayout.ClearReorderableListCache();
#endif
            if (pool != null && !pool.questions.IsNullOrEmpty())
            {
                var len = pool.questions.Count;
                for (int i = 0; i < len; i++)
                    output.Add(pool.questions[i]);
            }

            UpdatePorts();
#if UNITY_EDITOR
            if (NodeEditorWindow.current)
                NodeEditorWindow.current.Repaint();
#endif
        }

        public override void NextNode()
        {
            NodePort port = null;
            if (output.Count <= questionIndex) return;
            port = GetOutputPort("output " + questionIndex);
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
}