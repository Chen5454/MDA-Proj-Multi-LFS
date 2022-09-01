using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Measurements
{
    BPM, PainLevel, RespiratoryRate, CincinnatiLevel, BloodSugar, BloodPressure, OxygenSaturation,
    ETCO2, AdditionalMuscles, Breathing, BreathingSounds, Speakability, Consciousness, Pupils, SkinState
}

[System.Serializable]
public class PatientMeasurements
{
    public string[] MeasurementValues = new string[System.Enum.GetValues(typeof(Measurements)).Length];

    public void Initialize(string[] measurementsArray)
    {
        measurementsArray.CopyTo(MeasurementValues, 0);
    }
    public string GetMeasurement(Measurements x) => MeasurementValues[(int)x];
    public void SetMeasurementValues(string[] newValues)
    {
        for (int i = 0; i < MeasurementValues.Length; i++)
        {
            if (newValues[i] != null && newValues[i] != "")
            {
                MeasurementValues[i] = newValues[i];
            }
        }
    }
}
