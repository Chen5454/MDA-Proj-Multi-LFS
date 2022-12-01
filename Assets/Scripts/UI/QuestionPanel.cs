using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PatientCreationSpace;

public class QuestionPanel : MonoBehaviour
{
    [SerializeField]
    GameObject questionPrefab;
    [SerializeField]
    GameObject answerPrefab;
    [SerializeField]
    Transform parent;

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
            questionsAndAnswers = new List<GameObject>();
         if (bankQuestions == null)
            bankQuestions = new List<GameObject>();

        questionsBank = newPatientData.FullTreatmentSequence.GetQuestions();
        foreach (var q in questionsBank)
        {
            GameObject go = Instantiate(questionPrefab, parent);
            go.GetComponentInChildren<TMPro.TMP_Text>().text = q.questionText;
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
        foreach (var item in bankQuestions)
        {
            //questionsAndAnswers.Remove(item);
            item.SetActive(false);
        }
    }
    public void AskQuestion(Question q)
    {
        RemoveBankQuestions();
        GameObject go = Instantiate(questionPrefab, parent);
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
    }
}
