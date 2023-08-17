#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ConversationMatrixTool.Editor
{
    [CustomEditor(typeof(CharacterHook))]
    public class CharacterHookEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            CharacterHook myScript = (CharacterHook)target;
            if (GUILayout.Button("Generate Character SO"))
                myScript.GenerateCharacterScriptableObject();
            DrawDefaultInspector();
        }
    }
}
#endif