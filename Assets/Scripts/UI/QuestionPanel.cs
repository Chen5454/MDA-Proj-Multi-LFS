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
    public void SetMe(NewPatientData newPatientData)
    {
        if (questionsAndAnswers == null)
            questionsAndAnswers = new List<GameObject>();

        questionsBank = newPatientData.FullTreatmentSequence.GetQuestions();
        foreach (var q in questionsBank)
        {
            GameObject go = Instantiate(questionPrefab, parent);
            go.GetComponentInChildren<TMPro.TMP_Text>().text = q.questionText;

            //TBF TBD ALON - Here Answers should be loaded if relevant to treatmentsequence -
            //what answeres should be loaded to questions that are either not yet relevant or no longer relevant?

            questionsAndAnswers.Add(go);
        }
    }
}
