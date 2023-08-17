#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace ConversationMatrixTool.Editor
{
    [CustomEditor(typeof(BaseNode))]
    public class BaseNodeEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            BaseNode node = (BaseNode)target; 
            if (node.isCurrent)
            {
                GUILayout.BeginVertical("Box");
                GUILayout.Label("ACTIVE NODE");
                GUILayout.EndVertical();
            }
                
            DrawDefaultInspector();
        }
    }
}
#endif
