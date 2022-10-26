using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmbulancePermissions : MonoBehaviour
{
    [Header("Permissions")]
    [SerializeField] private bool _useMedicPermissions;
    [SerializeField] private bool _useSeniorMedicPermissions, _useParamedicDocPermissions;
    [SerializeField] private List<Button> _currentActionPermissions, _cprActionPermissions, _medicActionPermissions, _seniorMedicActionPermissions, _paramedicDocActionPermissions;
    [SerializeField] private Button _callNatanActionBtn, _aspirinActionBtn, _epipenAdultActionBtn, _infusionKitActionBtn, _connectDefibrilationActionBtn, _defibrilationAmbulanceActionBtn;

    private void InitializeMedicPermissions()
    {
        _medicActionPermissions.Add(_callNatanActionBtn);
        _medicActionPermissions.Add(_aspirinActionBtn);
        _medicActionPermissions.Add(_epipenAdultActionBtn);

        foreach (Button action in _medicActionPermissions)
        {
            action.interactable = true;
        }

        _currentActionPermissions = _medicActionPermissions;
    }
    private void InitializeSeniorMedicPermissions()
    {
        _seniorMedicActionPermissions.Add(_callNatanActionBtn);
        _seniorMedicActionPermissions.Add(_aspirinActionBtn);
        _seniorMedicActionPermissions.Add(_epipenAdultActionBtn);
        _seniorMedicActionPermissions.Add(_infusionKitActionBtn);

        _seniorMedicActionPermissions.Add(_defibrilationAmbulanceActionBtn);

        foreach (Button action in _seniorMedicActionPermissions)
        {
            action.interactable = true;
        }

        _currentActionPermissions = _seniorMedicActionPermissions;
    }
    private void InitializeParamedicDocPermissions()
    {
        _paramedicDocActionPermissions.Add(_callNatanActionBtn);
        _paramedicDocActionPermissions.Add(_aspirinActionBtn);
        _paramedicDocActionPermissions.Add(_epipenAdultActionBtn);

        _paramedicDocActionPermissions.Add(_defibrilationAmbulanceActionBtn);

        foreach (Button action in _paramedicDocActionPermissions)
        {
            action.interactable = true;
        }

        _currentActionPermissions = _paramedicDocActionPermissions;
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
    public void SetActions()
    {
        if (_currentActionPermissions == _paramedicDocActionPermissions)
        {
            _currentActionPermissions.Clear();
            _currentActionPermissions = _seniorMedicActionPermissions;
        }
        foreach (Button actionBtn in _currentActionPermissions)
        {
            actionBtn.interactable = true;
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

        foreach (Button action in _paramedicDocActionPermissions)
        {
            if (action.interactable)
                action.interactable = false;
        }
    }
}
