using UnityEditor;
using UnityEngine;

namespace ConversationMatrixTool.Editor
{
    [CustomPropertyDrawer(typeof(Character))]
    public class CharacterDrawer : PropertyDrawer
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
            Character currentCharInfo = property.objectReferenceValue as Character;
            if (currentCharInfo != null) buttonLabel = currentCharInfo.name;
            if (GUI.Button(buttonRect, buttonLabel))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("None"), currentCharInfo == null, () => SelectMatInfo(property, null));
                string[] guids = AssetDatabase.FindAssets("t:Character");
                for (int i = 0; i < guids.Length; i++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                    Character charInfo = AssetDatabase.LoadAssetAtPath(path, typeof(Character)) as Character;
                    if (charInfo != null)
                    {
                        GUIContent content = new GUIContent(charInfo.name);
                        string[] nameParts = charInfo.name.Split(' ');
                        if (nameParts.Length > 1)
                            content.text = nameParts[0] + "/" + charInfo.name.Substring(nameParts[0].Length + 1);
                        menu.AddItem(content, charInfo == currentCharInfo, () => SelectMatInfo(property, charInfo));
                    }
                }

                menu.ShowAsContext();
            }

            position.x += buttonRect.width + 4;
            position.width -= buttonRect.width + 4;
            EditorGUI.ObjectField(position, property, typeof(Character), GUIContent.none);

            if (EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        private void SelectMatInfo(SerializedProperty property, Character charInfo)
        {
            property.objectReferenceValue = charInfo;
            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
        }
    }
}