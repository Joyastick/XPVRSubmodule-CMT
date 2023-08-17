<<<<<<< Updated upstream
=======
/*
>>>>>>> Stashed changes
using UnityEditor;
using UnityEngine;

namespace ConversationMatrixTool
{
    [CustomPropertyDrawer(typeof(SequenceSO))]
    public class SequenceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();

            // Store old indent level and set it to 0, the PrefixLabel takes care of it

            position = EditorGUI.PrefixLabel(position, label);

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            Rect buttonRect = position;
            buttonRect.width = 80;

            string buttonLabel = "Select";
            SequenceSO currentSequenceInfo = property.objectReferenceValue as SequenceSO;
            if (currentSequenceInfo != null) buttonLabel = currentSequenceInfo.name;
            if (GUI.Button(buttonRect, buttonLabel))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("None"), currentSequenceInfo == null, () => SelectMatInfo(property, null));
                string[] guids = AssetDatabase.FindAssets("t:SequenceSO");
                for (int i = 0; i < guids.Length; i++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                    SequenceSO sequenceInfo = AssetDatabase.LoadAssetAtPath(path, typeof(SequenceSO)) as SequenceSO;
                    if (sequenceInfo != null)
                    {
                        GUIContent content = new GUIContent(sequenceInfo.name);
<<<<<<< Updated upstream
                        /* string[] nameParts = sequenceInfo.name.Split(' ');
                        if (nameParts.Length > 1)
                            content.text = nameParts[0] + "/" + sequenceInfo.name.Substring(nameParts[0].Length + 1);*/
                        content.text = sequenceInfo.title;
                        menu.AddItem(content, sequenceInfo == currentSequenceInfo, () => SelectMatInfo(property, sequenceInfo));
=======
                        // string[] nameParts = sequenceInfo.name.Split(' ');
                        // if (nameParts.Length > 1)
                        //    content.text = nameParts[0] + "/" + sequenceInfo.name.Substring(nameParts[0].Length + 1);
                        // content.text = sequenceInfo.title;
                        // menu.AddItem(content, sequenceInfo == currentSequenceInfo, () => SelectMatInfo(property, sequenceInfo));
>>>>>>> Stashed changes
                    }
                }

                menu.ShowAsContext();
            }

            position.x += buttonRect.width + 4;
            position.width -= buttonRect.width + 4;
            EditorGUI.ObjectField(position, property, typeof(SequenceSO), GUIContent.none);

            if (EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        private void SelectMatInfo(SerializedProperty property, SequenceSO sequenceInfo)
        {
            property.objectReferenceValue = sequenceInfo;
            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
        }
    }
<<<<<<< Updated upstream
}
=======
}
    */
>>>>>>> Stashed changes
