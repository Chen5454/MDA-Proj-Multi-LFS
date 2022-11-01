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
    List<Question> questions;

    List<GameObject> questionsAndAnswers;
   public void SetMe(NewPatientData newPatientData)
    {
        if (questionsAndAnswers == null)
            questionsAndAnswers = new List<GameObject>();

        questions = newPatientData.FullTreatmentSequence.GetQuestions();
        foreach (var q in questions)
        {
            GameObject go = Instantiate(questionPrefab, parent);
            go.GetComponentInChildren<TMPro.TMP_Text>().text = q.questionText;

            //TBF TBD ALON - Here Answers should be loaded if relevant to treatmentsequence -
            //what answeres should be loaded to questions that are either not yet relevant or no longer relevant?

            questionsAndAnswers.Add(go);
        }
    }
}
