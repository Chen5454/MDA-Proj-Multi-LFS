using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PatientCreationSpace;

public class QuestionPanel : MonoBehaviour
{
    [SerializeField]
    GameObject questionPrefab, triggeredQuestionPrefab;
    [SerializeField]
    GameObject answerPrefab;
    [SerializeField]
    Transform parent;
    [SerializeField]
    UnityEngine.UI.ScrollRect scrollRect;

    /// <summary>
    /// Questions available for player choosing
    /// </summary>
    List<Question> questionsBank;
    /// <summary>
    /// The current timeline of questions, answers AND the questions available to ask (i.e. questionBank)
    /// </summary>
    List<GameObject> questionsAndAnswers;
    List<GameObject> bankQuestions;
    public void SetMe(NewPatientData newPatientData)
    {
        if (questionsAndAnswers == null)
        {
            questionsAndAnswers = new List<GameObject>();
        }
        else
        {
            foreach (GameObject questionAndAnswer in questionsAndAnswers)
            {
                Destroy(questionAndAnswer);
            }

            questionsAndAnswers.Clear();
        }
            
        if (bankQuestions == null)
        {
            bankQuestions = new List<GameObject>();
        }
        else
        {
            foreach (GameObject question in bankQuestions)
            {
                Destroy(question);
            }

            bankQuestions.Clear();
        }

        questionsBank = newPatientData.FullTreatmentSequence.GetQuestions();
        foreach (var q in questionsBank)
        {
            GameObject go = Instantiate(questionPrefab, parent);
            go.GetComponentInChildren<TMPro.TMP_Text>().text = q.questionText;
            go.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => AskQuestion(q));
            go.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => newPatientData.AnswerSheet.AttemptTreatment(q));
            //TBF TBD ALON - Here Answers should be loaded if relevant to treatmentsequence -
            //what answeres should be loaded to questions that are either not yet relevant or no longer relevant?
            bankQuestions.Add(go);
            //questionsAndAnswers.Add(go);
        }
    }
    void AddBankQuestions()
    {
        foreach (var item in bankQuestions)
        {
            //questionsAndAnswers.Add(item);
            item.SetActive(true);
            item.transform.SetAsLastSibling();
        }
    }
    void RemoveBankQuestions()
    {
        foreach (GameObject item in bankQuestions)
        {
            //questionsAndAnswers.Remove(item);
            item.SetActive(false);
        }
    }
    public void AskQuestion(Question q)
    {
        RemoveBankQuestions();
        GameObject go = Instantiate(triggeredQuestionPrefab, parent);
        go.GetComponentInChildren<TMPro.TMP_Text>().text = q.questionText;
        questionsAndAnswers.Add(go);
        //AddBankQuestions();
    }
    public void RecieveAnswer(string answerText)
    {
        RemoveBankQuestions();
        GameObject go = Instantiate(answerPrefab, parent);
        go.GetComponentInChildren<TMPro.TMP_Text>().text = answerText;

        questionsAndAnswers.Add(go);
        AddBankQuestions();
        Invoke(nameof(ScrollAfterAsking), 0.3f);
    }
    public void ScrollAfterAsking()
    {
        scrollRect.normalizedPosition = new Vector2(0, 0);
    }
}
