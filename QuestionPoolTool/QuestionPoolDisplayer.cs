using System;
using System.Collections.Generic;
using ConversationMatrixTool;
using MyBox;
using UnityEngine;

public class QuestionPoolDisplayer : MonoBehaviour
{
    public int questionCount;
    public GameObject questionPrefab;
    public Transform questionParent;
    public float startHeight;
    public ConversationMatrixGraphPlayer cmgPlayer;
    [SerializeField] private RectTransform[] questions;
    public float answerOpenSize = 300f, answerClosedSize = 150f;

    private void Start()
    {
        GrabQuestions();
    }

    public void ActivateUIQuestions(int count)
    {
        foreach (var question in questions)
            question.gameObject.SetActive(question.GetComponent<UiQuestion>().index < count);
    }

    [ButtonMethod()]
    private void Setup()
    {
        if (!questionPrefab) return;
        questions = new RectTransform[questionCount];
        for (int i = 0; i < questionCount; i++)
        {
            if (questionParent == null) questionParent = transform;
            if (questionPrefab)
            {
                var temp = Instantiate(questionPrefab, questionParent);
                temp.GetComponent<UiQuestion>().SetIndex(i);
                temp.GetComponent<UiQuestion>().cmgPlayer = GetCMG();
                questions[i] = temp.GetComponent<RectTransform>();
            }
        }

        ReDrawQuestions();
    }

    [ButtonMethod()]
    public void ReDrawQuestions()
    {
        if (questions.IsNullOrEmpty()) GrabQuestions();
        var y = startHeight;
        for (int i = 0; i < questions.Length; i++)
        {
            var q = questions[i].GetComponent<UiQuestion>();
            if (q != null)
            {
                questions[i].anchoredPosition =
                    new Vector3(0, y);
                y -= q.isOpen ? answerOpenSize : answerClosedSize;
            }
        }

        var rect = questionParent.GetComponent<RectTransform>();
        if (rect == null) return;
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, -y);
    }

    [ButtonMethod()]
    public void Clear()
    {
        if (questions.IsNullOrEmpty()) GrabQuestions();
        for (int i = 0; i < questions.Length; i++)
            if (Application.isPlaying)
                Destroy(questions[i].gameObject);
            else
                DestroyImmediate(questions[i].gameObject);
        questions = Array.Empty<RectTransform>();
    }

    private ConversationMatrixGraphPlayer GetCMG()
    {
        if (cmgPlayer == null) cmgPlayer = GetComponent<ConversationMatrixGraphPlayer>();
        if (cmgPlayer == null) cmgPlayer = GetComponentInChildren<ConversationMatrixGraphPlayer>();
        if (cmgPlayer == null) cmgPlayer = FindObjectOfType<ConversationMatrixGraphPlayer>();
        return cmgPlayer;
    }

    [ButtonMethod()]
    private void GrabQuestions()
    {
        questions = new RectTransform[questionCount];
        var _questions = FindObjectsOfType<UiQuestion>();
        for (int i = 0; i < questionCount; i++)
            questions[_questions[i].index] = _questions[i].GetComponent<RectTransform>();
    }


    public QuestionPool qp;

    [ButtonMethod()]
    public void TestDisplayQP()
    {
        DisplayQuestions(qp.questions);
    }

    public void DisplayQuestions(List<PoolQuestion> poolQuestions)
    {
        var len = poolQuestions.Count;
        ActivateUIQuestions(len);

        for (int i = 0; i < len; i++)
        {
            questions[i].GetComponent<UiQuestion>().question.text = poolQuestions[i].question;
            questions[i].GetComponent<UiQuestion>().answer.text = poolQuestions[i].answer;
        }
    }
}