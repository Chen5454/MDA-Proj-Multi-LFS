using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

public class AmbulancePermissions : MonoBehaviour
{
    [Header("Permissions")]
    [SerializeField] private bool _useMedicPermissions;
    [SerializeField] private bool _useSeniorMedicPermissions, _useParamedicDocPermissions;
    [SerializeField] private List<Button> _cprActionPermissions, _medicActionPermissions, _seniorMedicActionPermissions, _paramedicDocActionPermissions;
    [SerializeField] private Button _callNatanActionBtn, /*_aspirinActionBtn, _epipenAdultActionBtn, _infusionKitActionBtn, */_qTKitActionBtn, _adultQTKitActionBtn, _intubationKitAdultActionBtn, _intubationKitKidsActionBtn, _zondaKitActionBtn, _respiratorActionBtn, _kateterBujiActionBtn, _peepActionBtn, _breslauScaleActionBtn, _bigKidsActionBtn, _inhalationMaskActionBtn, _oxygenIndicatorsActionBtn, _capnoTubusActionBtn, _capnoNezaliActionBtn, _connectDefibrilationActionBtn, _defibrilationAmbulanceActionBtn, _syncedFlipActionBtn, _pacingActionBtn;

    private void Start()
    {
        //InitializeMedicPermissions();
        //InitializeSeniorMedicPermissions();
        //InitializeParamedicDocPermissions();
    }

    private void InitializeMedicPermissions()
    {
        _medicActionPermissions.Add(_callNatanActionBtn);
    }
    private void InitializeSeniorMedicPermissions()
    {
        _seniorMedicActionPermissions.Add(_callNatanActionBtn);
        _seniorMedicActionPermissions.Add(_defibrilationAmbulanceActionBtn);
    }
    private void InitializeParamedicDocPermissions()
    {
        _paramedicDocActionPermissions.Add(_callNatanActionBtn);

        _paramedicDocActionPermissions.Add(_adultQTKitActionBtn);
        _paramedicDocActionPermissions.Add(_intubationKitAdultActionBtn);
        _paramedicDocActionPermissions.Add(_intubationKitKidsActionBtn);
        _paramedicDocActionPermissions.Add(_zondaKitActionBtn);
        _paramedicDocActionPermissions.Add(_respiratorActionBtn);
        _paramedicDocActionPermissions.Add(_kateterBujiActionBtn);
        _paramedicDocActionPermissions.Add(_peepActionBtn);
        _paramedicDocActionPermissions.Add(_breslauScaleActionBtn);
        _paramedicDocActionPermissions.Add(_bigKidsActionBtn);
        _paramedicDocActionPermissions.Add(_inhalationMaskActionBtn);
        _paramedicDocActionPermissions.Add(_oxygenIndicatorsActionBtn);
        _paramedicDocActionPermissions.Add(_capnoTubusActionBtn);
        _paramedicDocActionPermissions.Add(_capnoTubusActionBtn);
        _paramedicDocActionPermissions.Add(_capnoNezaliActionBtn);
        _paramedicDocActionPermissions.Add(_connectDefibrilationActionBtn);
        _paramedicDocActionPermissions.Add(_syncedFlipActionBtn);
        _paramedicDocActionPermissions.Add(_pacingActionBtn);
    }
    public List<Button> InitializePermissions(Roles role)
    {
        _useMedicPermissions = false;
        _useSeniorMedicPermissions = false;
        _useParamedicDocPermissions = false;

        if (role == Roles.Medic)
        {
            InitializeMedicPermissions();
            _useMedicPermissions = true;
            return _medicActionPermissions;
        }
        else if (role == Roles.SeniorMedic)
        {
            InitializeSeniorMedicPermissions();
            _useSeniorMedicPermissions = true;
            return _seniorMedicActionPermissions;
        }
        else if (role == Roles.Paramedic || role == Roles.Doctor)
        {
            InitializeParamedicDocPermissions();
            _useParamedicDocPermissions = true;
            return _paramedicDocActionPermissions;
        }
        else
        {
            return _cprActionPermissions;
        }
    }
    public void SetActions(List<Button> permissionList)
    {
        if (permissionList == _paramedicDocActionPermissions)
        {
            permissionList = _seniorMedicActionPermissions;
        }
        foreach (Button actionBtn in _paramedicDocActionPermissions)
        {
            if (!actionBtn)
                continue;
            else
                actionBtn.enabled = false;
        }
        foreach (Button actionBtn in permissionList)
        {
            if (!actionBtn)
                continue;
            else
                actionBtn.enabled = true;
        }
    }
    public void RemovePermissions(Roles role)
    {
        if (role == Roles.Medic)
        {
            _medicActionPermissions.Clear();
        }
        else if (role == Roles.SeniorMedic)
        {
            _seniorMedicActionPermissions.Clear();
        }
        else if (role == Roles.Paramedic || role == Roles.Doctor)
        {
            _paramedicDocActionPermissions.Clear();
        }
        else
        {
            return;
        }
    }
}
