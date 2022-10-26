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
    [SerializeField] private Button _callNatanActionBtn, _connectDefibrilationActionBtn, _defibrilationAmbulanceActionBtn;

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
        _paramedicDocActionPermissions.Add(_connectDefibrilationActionBtn);
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
    public void RemovePermissions()
    {
        //if (_medicActionPermissions[0])
        //{
        //    foreach (Button actionBtn in _medicActionPermissions)
        //        actionBtn.enabled = false;
        //
        //    _medicActionPermissions.Clear();
        //}
        //
        //if (_seniorMedicActionPermissions[0])
        //{
        //    foreach (Button actionBtn in _seniorMedicActionPermissions)
        //        actionBtn.enabled = false;
        //
        //    _seniorMedicActionPermissions.Clear();
        //}
        //
        //if (_paramedicDocActionPermissions[0])
        //{
        //    foreach (Button actionBtn in _paramedicDocActionPermissions)
        //        actionBtn.enabled = false;
        //
        //    _paramedicDocActionPermissions.Clear();
        //}

        _medicActionPermissions.Clear();
        _seniorMedicActionPermissions.Clear();
        _paramedicDocActionPermissions.Clear();
    }
}
