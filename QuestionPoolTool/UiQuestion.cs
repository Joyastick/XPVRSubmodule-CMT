using ConversationMatrixTool;
using MyBox;
using UnityEngine;
using UnityEngine.UI;

public class UiQuestion : MonoBehaviour
{
    public int index;
    public Text qNo, aNo, question, answer;
    public ConversationMatrixGraphPlayer cmgPlayer;
    public bool isOpen;
    public GameObject answerPanel;
    public QuestionPoolDisplayer QPD;
    public void Ask()
    {
        GetCMG().Answer(index);
        answer.enabled = true;
    }

    [ButtonMethod()]
    public void Toggle()
    {
        isOpen = !isOpen;
        answerPanel.SetActive(isOpen);
        GetQPD().ReDrawQuestions();
    }

    public void SetIndex(int _index)
    {
        index = _index;
        qNo.text = "Q" + (index + 1);
        aNo.text = "A" + (index + 1);
    }

    private QuestionPoolDisplayer GetQPD()
    {
        if (QPD == null) QPD = GetComponent<QuestionPoolDisplayer>();
        if (QPD == null) QPD = GetComponentInChildren<QuestionPoolDisplayer>();
        if (QPD == null) QPD = FindObjectOfType<QuestionPoolDisplayer>();
        return QPD;
    }
    
    private ConversationMatrixGraphPlayer GetCMG()
    {
        if (cmgPlayer == null) cmgPlayer = GetComponent<ConversationMatrixGraphPlayer>();
        if (cmgPlayer == null) cmgPlayer = GetComponentInChildren<ConversationMatrixGraphPlayer>();
        if (cmgPlayer == null) cmgPlayer = FindObjectOfType<ConversationMatrixGraphPlayer>();
        return cmgPlayer;
    }
}