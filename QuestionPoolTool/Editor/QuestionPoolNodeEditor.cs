using UnityEngine;
using XNodeEditor;

namespace ConversationMatrixTool.Editor
{
    [CustomNodeEditor(typeof(QuestionPoolNode))]
    public class QuestionPoolNodeEditor : NodeEditor
    {
        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            QuestionPoolNode myScript = (QuestionPoolNode)target;
            GUILayout.BeginVertical("Box");
            if (GUILayout.Button("Refresh"))
            {
                myScript.GetQuestions();
            }
            GUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
        }
    }
}