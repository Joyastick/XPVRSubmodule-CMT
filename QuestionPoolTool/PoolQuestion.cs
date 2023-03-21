using System;
using MyBox;
using UnityEngine;

namespace ConversationMatrixTool
{
    [Serializable]
    public class PoolQuestion
    {
        public string question;
        public string answer;
        public bool isConstant = true;
        [ConditionalField("isConstant", true)] public string tag;
        [HideInInspector] public bool isRevealed;
    }
}