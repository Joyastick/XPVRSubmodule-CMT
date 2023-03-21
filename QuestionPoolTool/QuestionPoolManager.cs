using System.Collections.Generic;
using ConversationMatrixTool;
using UnityEngine;

public class QuestionPoolManager : MonoBehaviour
{
    public static QuestionPoolManager instance;

    [SerializeField] private List<string> tags = new List<string>();

    public void Start()
    {
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    public void ProcessTag(string _tag)
    {
        if (!tags.Contains(_tag))
            tags.Add(_tag);
    }

    public void ResetTags()
    {
        tags = new List<string>();
    }
    
    public List<PoolQuestion> RevealQuestions(QuestionPool pool)
    {
        if (pool.questions == null) return null;
        var len = pool.questions.Count;
        if (len == 0) return null;
        for (int i = 0; i < len; i++)
        {
            var poolQuestion = pool.questions[i];
            poolQuestion.isRevealed = poolQuestion.isConstant ? true : CheckAvailability(poolQuestion);
            pool.questions[i] = poolQuestion;
        }

        return pool.questions;
    }

    bool CheckAvailability(PoolQuestion question)
    {
        return tags.Contains(question.tag);
    }
}