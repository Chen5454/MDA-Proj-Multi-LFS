using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PatientCreationSpace;

public enum PatientType { Old, Grown, Kid, }
public enum PatientGender { Male, Female }
public enum MonitorSprites { HeartMonitor, ECG }
public enum PatientCondition { Dead, Critical, Urgent, Nonurgent, Untagged }

[System.Serializable]
public class NewPatientData
{
    [Header("Patient Informaion")]
    public PatientType PatientType;
    public string Name;
    public string SureName;
    public int Id, Age; //these two need to strings, in my opinion TBF alon 12/10/22
    public PatientGender Gender;
    public string PhoneNumber;
    public string MedicalCompany, AddressLocation, Complaint;
    public PatientCondition Status;
    public Color StatusColor;

    public DestinationRoom _DestinationRoom;

    public string GetSaveFileName => $"{Name}_{SureName}.txt";
    public string GetTreatmentSequenceSaveFileName => $"{Name}_{SureName}_treatmentSequence.txt";

    /// <summary>
    /// true = ALS | false = BLS
    /// </summary>
    public bool isALS;
    /// <summary>
    /// false is illness
    /// </summary>
    public bool isTrauma;

    [Header("Measurments")]
    [SerializeField] private PatientMeasurements _patientMeasurement;
    public bool IsConscious; //Getter? TBF

    [Header("MonitorGraphTexture")]
    public List<Sprite> MonitorSpriteList;

    public TreatmentSequence FullTreatmentSequence;

    public AnswerSheet AnswerSheet;
    //public TreatmentSequence GetTreatmeantSequence { get => paitent_FullTreatmentSequence; }

    public NewPatientData() { }

    public void Initialize(int patientType, string name, string sureName, int id, int age, bool gender, string phoneNum, string medicalCompany, string complaint, string[] measurements, DestinationRoom room, bool isAls, bool trauma)
    {
        PatientType = (PatientType)patientType;
        Name = name;
        SureName = sureName;
        Id = id;
        Age = age;
        if (gender)
            Gender = PatientGender.Male;
        else
            Gender = PatientGender.Female;
        PhoneNumber = phoneNum;
        MedicalCompany = medicalCompany;
        //AddressLocation = adress;
        Complaint = complaint;
        _patientMeasurement = new PatientMeasurements();
        _patientMeasurement.Initialize(measurements);

        _DestinationRoom = room;

        isALS = isAls;
        isTrauma = trauma;
        FullTreatmentSequence = new TreatmentSequence();
        FullTreatmentSequence.Init();
        Status = PatientCondition.Untagged;
        StatusColor = Color.grey;
        //AnswerSheet = new AnswerSheet();
        //AnswerSheet = 
        //AnswerSheet.Set(this);
    }

    //TBC - Init with ALL required data (measrurements, basic info, models, everythig!)

    public string GetMeasurement(int x) => _patientMeasurement.GetMeasurement((Measurements)x);
    public string GetMeasurement(Measurements x) => _patientMeasurement.GetMeasurement(x);
    public void SetPatientMeasurement(string[] x) => _patientMeasurement.SetMeasurementValues(x);
}
