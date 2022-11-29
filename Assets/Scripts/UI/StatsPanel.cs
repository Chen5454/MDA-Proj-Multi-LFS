using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatsPanel : MonoBehaviour
{
    [SerializeField]
    List<TMP_Text> textBoxs;

    NewPatientData connectedPatient;
    public void SetMe(NewPatientData newPatientData)
    {
        if (textBoxs.Count != System.Enum.GetValues(typeof(Measurements)).Length)
        {
            Debug.LogError("Different counts of measurements and measurement-fields!");
            return;
        }

        connectedPatient = newPatientData;

        ////test
        //SetAllFields();
    }

    //private void SetAllFields()
    //{
    //    for (int i = 0; i < textBoxs.Count; i++)
    //    {
    //        UpdateField(i);
    //    }
    //}

    public void UpdateMeasurement(int i)
    {
        textBoxs[i].text = $"{(Measurements)i}: {connectedPatient.GetMeasurement(i)}";
    }
    public void UpdateMeasurement(Measurements m)
    {
        textBoxs[(int)m].text = $"{m}: {connectedPatient.GetMeasurement((int)m)}";
    }

    private void SetFieldBlank(int i)
    {
        textBoxs[i].text = $"{(Measurements)i}: ";
    }
    //[ContextMenu("Set box text")]
    //public void SetBoxText()
    //{
    //    for (int i = 0; i < textBoxs.Count; i++)
    //    {
    //        SetFieldBlank(i);
    //    }
    //}
}
