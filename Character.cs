using System;
using MyBox;
using UnityEngine;

namespace ConversationMatrixTool
{
    [Serializable]
    public class Character : ScriptableObject
    {
        [Tooltip("name of the character - not used for anything specific - just for identification purposes")]
        public string characterName;
        [Tooltip("a colour is assigned to each character at creation but you can change it here")]
        public Color colour;
        [Tooltip("this is how we identify and associate character scriptable objects with character prefabs")]
        [ReadOnly()]public string guid;
        [Tooltip("animator to animate the character - all animation nodes will use this animator to animate this character")]
        public RuntimeAnimatorController animator;
    }
}