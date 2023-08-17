#if UNITY_EDITOR
using MyBox;
using UnityEngine;
using XNodeEditor;

namespace ConversationMatrixTool
{
    [CustomNodeEditor(typeof(AnimationNodeBase))]
    public class AnimationNodeBaseEditor : NodeEditor
    {
        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            AnimationNodeBase node = target as AnimationNodeBase;
            if (node == null)
            {
                return;
            }

            var array = node.GetParameters();

            var len = !array.IsNullOrEmpty() ? array.Length : 0;
            GUILayout.BeginVertical("Box");
            node.buttonText = node.showParameters ? "Hide Parameters" : "Show Parameters";
            node.showParameters = GUILayout.Toggle(node.showParameters, node.buttonText);
            GUILayout.Space(20);
            if (node.showParameters)
            {
                if (len == 0)
                    GUILayout.Label("No parameters");
                else
                    node.selectedAnim = GUILayout.SelectionGrid(node.selectedAnim, array, 1);
                GUILayout.Space(20);
            }

            if (node.selectedAnim >= 0 && node.selectedAnim < len)
                GUILayout.Label("Selected: " + array[node.selectedAnim]);
            //serializedObject.ApplyModifiedProperties();
            GUILayout.EndVertical();
        }

        public override Color GetTint() //changes colour of the statement node based on the character colour
        {
            var col = base.GetTint();
            AnimationNodeBase node = target as AnimationNodeBase;

            if (node != null)
                if (node.character != null)
                {
                    col = node.character.colour;
                }

            col.a = 1;
            return col;
        }
    }
}
#endif