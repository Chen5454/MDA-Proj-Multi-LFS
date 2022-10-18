using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace PatientCreationSpace
{

    public static class QuestionCreator
    {
        //TBF - create a static class to hold all paths
        //static readonly string scriptableObjects_FolderPath = "Assets/Scriptables/Questions/";

        public static Question CreateQuestion(string newID, string newQuestion, string newAnswer)
        {

            //Question q = SO_Creator<Question>.CreateT(newID, $"{PatientCreator.patientID}/Questions/");
            Question q = new Question();

            q.SetQuestion(newID, newQuestion, newAnswer);

            //TBF add new questions to database!

            return q;
        }
    }

}