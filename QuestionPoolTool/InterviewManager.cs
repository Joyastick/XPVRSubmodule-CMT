using System;
using ConversationMatrixTool;
using UnityEngine;

public class InterviewManager : MonoBehaviour
{
    public static InterviewManager instance;
    public ConversationMatrixGraphPlayer cmgPlayer;

    private void Start()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(this);
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
        if (cmgPlayer == null) FindObjectOfType<ConversationMatrixGraphPlayer>();
    }

    //Interviewee Parameters
    [SerializeField] public int interviewCount, interviewIndex;
    [SerializeField] private int[] interviewIndexes;
    public Interview[] interviews;
    private Interview currentInterview;

    public void SetCaseInterviews(int[] _interviewIndexes)
    {
        interviewIndexes = _interviewIndexes;
        interviewCount = interviews.Length;
        interviewIndex = 0;
    }

    public void StartInterviews()
    {
        RunInterview(interviewIndexes[interviewIndex]);
    }

    public void RunInterview(int _interviewIndex)
    {
        currentInterview = interviews[_interviewIndex];
        if (cmgPlayer) cmgPlayer.StartConversation(currentInterview.conversationName);
    }

    public void NextInterview()
    {
        UnloadInterviewAndLoadNextInterview();
    }

    private void UnloadInterviewAndLoadNextInterview()
    {
        UnloadInterview(currentInterview);
        interviewIndex++;
        if (interviewIndex < interviewCount)
            RunInterview(interviewIndex);
        else
            EndInterviews();
    }

    private void UnloadInterview(Interview _currentInterview)
    {
        //cmgPlayer.EndGraph();
        interviews[interviewIndexes[interviewIndex]].Complete();
        if (_currentInterview.character != null) Destroy(_currentInterview.character);
    }

    public void EndInterviews()
    {
        //return to main menu
    }
}

[Serializable]
public struct Interview
{
    public string title, conversationName;
    public GameObject character;
    public QuestionPool pool;
    public bool isComplete;

    public void Complete()
    {
        isComplete = true;
    }
}