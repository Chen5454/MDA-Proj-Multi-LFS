using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PatientType { Old, Grown, Kid, }
public enum MonitorSprites { HeartMonitor, ECG }

[System.Serializable]
public class NewPatientData
{
    [Header("Patient Informaion")]
    public PatientType PatientType;
    public string Name;
    public string SureName;
    public int Id, Age;
    public string Gender;
    public string PhoneNumber;
    public string MedicalCompany, AddressLocation, Complaint;

    [Header("Measurments")]
    [SerializeField] private PatientMeasurements _patientMeasurement;

    [Header("MonitorGraphTexture")]
    public List<Sprite> MonitorSpriteList;

    public NewPatientData() { }

    public NewPatientData(NewPatientData newPatientDataFromSO)
    {
        PatientType = newPatientDataFromSO.PatientType;
        Name = newPatientDataFromSO.Name;
        SureName = newPatientDataFromSO.SureName;
        Id = newPatientDataFromSO.Id;
        Age = newPatientDataFromSO.Age;
        PhoneNumber = newPatientDataFromSO.PhoneNumber;
        MedicalCompany = newPatientDataFromSO.MedicalCompany;
        AddressLocation = newPatientDataFromSO.AddressLocation;
        Complaint = newPatientDataFromSO.Complaint;
        _patientMeasurement = newPatientDataFromSO._patientMeasurement;
        MonitorSpriteList = newPatientDataFromSO.MonitorSpriteList;
    }

    public void Initialize(string name, string sureName, int id, int age, string gender, string phoneNum, string medicalCompany, string adress, string complaint, string[] measurements)
    {
        Name = name;
        SureName = sureName;
        Id = id;
        Age = age;
        Gender = gender;
        PhoneNumber = phoneNum;
        MedicalCompany = medicalCompany;
        AddressLocation = adress;
        Complaint = complaint;

        _patientMeasurement.Initialize(measurements);
    }
    public string GetMeasurement(int x) => _patientMeasurement.GetMeasurement((Measurements)x);
    public string GetMeasurement(Measurements x) => _patientMeasurement.GetMeasurement(x);
    public void SetPatientMeasurement(string[] x) => _patientMeasurement.SetMeasurementValues(x);
}
