using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PatientCreationSpace
{

    [System.Serializable]
    public class Question : Treatment
    {
        //these get set OnEnable to ensure data is kept and not wiped
        [SerializeField]
        string questionContent;
        [SerializeField]
        string answerContent;

        //[SerializeField]
        public string questionText;
        //[SerializeField]
        public string answerText;
        /// <summary>
        /// Inits questions.
        /// Should be called when creating a new question
        /// </summary>
        /// <param name="newID">for the Treatment Set() - TBF needs to be pulled from the last used ID, and add 1</param>
        /// <param name="newAnswer">specific patient reply</param>
        public Question SetQuestion(string newID, string newQuestion, string newAnswer)
        {
            questionText = newQuestion;
            answerText = newAnswer;
            base.Set(newID, GetType().ToString().Substring(21)); ////Ignores the initial namespace and period ("PatientCreationSpace.") - 21 chars
            return this;
        }
        public override object Result()
        {
            return answerText;
        }
        public override string DisplayStringAsPartOfSequence()
        {
            return $"שאלה:{questionText} \n תשובה:{answerText}";
        }
        public override string TreatmentDisplayNameAsPartOfDatabase()
        {
            return questionText;
        }
    }

}