using UnityEditor;
using UnityEngine;

namespace ConversationMatrixTool.Editor
{
    [CustomEditor(typeof(CMT_UI_Button))]
    public class CMT_UI_ButtonEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            CMT_UI_Button myScript = (CMT_UI_Button)target;
            DrawDefaultInspector();
            if (myScript.useExistingButton)
            GUILayout.Label("Events on the existing button will be invoked on press!");
        }
    }
}