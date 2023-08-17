#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ConversationMatrixTool.Editor
{
    [CustomEditor(typeof(ConversationMatrixGraphPlayer))]
    public class ConversationMatrixGraphPlayerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            ConversationMatrixGraphPlayer myScript = (ConversationMatrixGraphPlayer)target;
            if (GUILayout.Button("Scan for Characters in Scene"))
            {
                myScript.ScanForCharactersInScene();
            }
            if (GUILayout.Button("Next Node"))
            {
                myScript.NextNode();
            }
            
            if (GUILayout.Button("Save Conversation as Text"))
            {
                myScript.WriteConvoToDisk();
            }
            DrawDefaultInspector();
        }
    }
}
#endif