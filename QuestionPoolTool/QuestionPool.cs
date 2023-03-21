using System;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

namespace ConversationMatrixTool
{
    [CreateAssetMenu(menuName = "Conversation Matrix/QuestionPool", order = 1)]
    [Serializable]
    public class QuestionPool : ScriptableObject
    {
        public List<PoolQuestion> questions;

        [ButtonMethod()]
        public void AddQuestion()
        {
            questions.Add(new PoolQuestion());
        }

        public int removeIndex;

        [ButtonMethod()]
        public void RemoveQuestion()
        {
            questions.Remove(questions[removeIndex]);
        }

        public bool confirmDeletion;

        [ButtonMethod()]
        public void ClearQuestions()
        {
            if (!confirmDeletion) return;
            questions = new List<PoolQuestion>();
            confirmDeletion = false;
        }
    }
}