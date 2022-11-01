using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatsPanel : MonoBehaviour
{
    [SerializeField]
    List<TMP_Text> textBoxs;

    public void SetMe(NewPatientData newPatientData)
    {
        for (int i = 0; i < textBoxs.Count; i++)
        {
            textBoxs[i].text = $"{(Measurements)i}: {newPatientData.GetMeasurement(i)}";
        }
    }
}
