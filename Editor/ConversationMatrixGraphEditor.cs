using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace ConversationMatrixTool.Editor
{
    [CustomNodeGraphEditor(typeof(ConversationMatrixGraph))]
    public class ConversationMatrixGraphEditor : NodeGraphEditor
    {
        private ConversationMatrixGraph graph;
        private bool _autoSave;
        private Node start, end;
        private float targetX;

        public override void OnGUI()
        {
            if (graph == null) graph = (ConversationMatrixGraph)target;

            if (start == null)
                foreach (var node in graph.nodes)
                    if (((BaseNode)node).type == NodeType.Start)
                        start = node;

            if (end == null)
                foreach (var node in graph.nodes)
                    if (((BaseNode)node).type == NodeType.End)
                        end = node;

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("SAVE", GUILayout.Width(80f)))
            {
                NodeEditorWindow.current.Save();
                AssetDatabase.SaveAssetIfDirty(graph);
            }

            if (GUILayout.Button("HOME", GUILayout.Width(80f)))
            {
                NodeEditorWindow.current.SaveChanges();
                NodeEditorWindow.current.Home();
            }

            if (GUILayout.Button("|<<", GUILayout.Width(80f)))
                graph.GoToStart();

            if (GUILayout.Button(">>|", GUILayout.Width(80f)))
                graph.GoToEnd();

            GUILayoutOption[] options = new GUILayoutOption[1];
            options[0] = GUILayout.Width(1111f);
            EditorGUILayout.Separator();
            if (end && start)
                targetX = GUILayout.HorizontalScrollbar(targetX, 6.66f, end.position.x, start.position.x, options);
            if (!NodeEditorWindow.isPanning)
                NodeEditorWindow.current.panOffset = new Vector2(targetX, NodeEditorWindow.current.panOffset.y);
            else
                targetX = NodeEditorWindow.current.panOffset.x;

            EditorGUILayout.Separator();
            //GUILayout.Box("Pan Offset: " + NodeEditorWindow.current.panOffset + " | Zoom: " + NodeEditorWindow.current.zoom);
            EditorGUILayout.EndHorizontal();
        }
    }
}