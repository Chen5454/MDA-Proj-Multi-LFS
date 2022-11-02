using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace PatientCreationSpace
{

    public class NewQuestionWindow : NewBlockWindow
    {
        [SerializeField]
        TMP_InputField questionText;
        [SerializeField]
        TMP_InputField replyText;



        [SerializeField]
        BlockCollectionEditor treatmentSequenceEditorWindow;
        //private void OnEnable()
        //{
        //    questionText.text = " ";
        //    replyText.text = " ";
        //}
        public override void OnDisable()
        {
            questionText.text = "";
            replyText.text = "";
            base.OnDisable();
        }
        public void ClickOnCreateNew()
        {
            if (string.IsNullOrEmpty(questionText.text) || string.IsNullOrEmpty(replyText.text))
            {
                Debug.LogError("both reply and question text needs to be added");
                return;
            }
            treatmentSequenceEditorWindow.AddTreatmentToCollection(QuestionCreator.CreateQuestion($"{System.DateTime.Now.ToString("m-s")}", questionText.text, replyText.text));

            //Release other buttons-lock? tbf

            gameObject.SetActive(false);
        }



    }

}