using PatientCreationSpace;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine;

public class QuestionBlock : MonoBehaviour, BasicBlock
{

    [SerializeField]
    TMP_InputField questionText;
    [SerializeField]
    TMP_InputField replyText;

    bool _isInteractable;
    //public void ClickOnCreateNew()
    //{
    //    if (string.IsNullOrEmpty(questionText.text) || string.IsNullOrEmpty(replyText.text))
    //    {
    //        Debug.LogError("both reply and question text needs to be added");
    //        return;
    //    }
    //    treatmentSequenceEditorWindow.AddTreatmentToCollection(QuestionCreator.CreateQuestion($"{System.DateTime.Now.ToString("m-s")}", questionText.text, replyText.text));

    //    //Release other buttons-lock? tbf

    //    gameObject.SetActive(false);
    //}
    //private void OnEnable()
    //{
    //    SetInteractable(true);
    //}
    public Treatment GetTreatment()
    {
        return QuestionCreator.CreateQuestion($"{System.DateTime.Now.ToString("m-s")}", questionText.text, replyText.text);
    }
    public TreatmentGroup GetTreatmentGroup()
    {
        return null;
    }
    public bool IsInteractable()
    {
        return _isInteractable;
    }

    public void SetInteractable(bool isInteractable)
    {
        questionText.interactable = isInteractable;
        replyText.interactable = isInteractable;
        _isInteractable = isInteractable;
    }

    GameObject BasicBlock.gameObject()
    {
        return gameObject;
    }
}
