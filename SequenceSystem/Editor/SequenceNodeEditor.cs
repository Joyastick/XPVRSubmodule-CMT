<<<<<<< Updated upstream
=======
/*
>>>>>>> Stashed changes
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XNode;

namespace ConversationMatrixTool
{
    [CustomEditor(typeof(SequenceNode))]
    public class SequenceNodeEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DrawDefaultInspector();
            SequenceNode node = (SequenceNode)target;
            if (node == null) return;
            if (!node.sequence) return;
            node.title = node.sequence.title;
        }
    }
<<<<<<< Updated upstream
}
=======
}
*/
>>>>>>> Stashed changes
