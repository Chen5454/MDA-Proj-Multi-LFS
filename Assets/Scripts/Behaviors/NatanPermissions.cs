using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NatanPermissions : MonoBehaviour
{
    [Header("Permissions")]
    [SerializeField] private bool _useMedicPermissions;
    [SerializeField] private bool _useSeniorMedicPermissions, _useParamedicDocPermissions;
    [SerializeField] private List<Button> _currentActionPermissions, _cprActionPermissions, _medicActionPermissions, _seniorMedicActionPermissions, _paramedicDocActionPermissions;
    [SerializeField] private Button _callAmbulanceActionBtn, _aspirinAmbuActionBtn, _epipenAdultActionBtn, _infusionKitActionBtn, _cPapActionBtn, _lmaActionBtn, _qTKitActionBtn, _adultQTKitActionBtn, _intubationKitAdultActionBtn, _intubationKitKidsActionBtn, _zondaKitActionBtn, _respiratorActionBtn, _kateterBujiActionBtn, _peepActionBtn, _breslauScaleActionBtn, _bigKidsActionBtn, _inhalationMaskActionBtn, _oxygenIndicatorsActionBtn, _capnoTubusActionBtn, _capnoNezaliActionBtn, _connectDefibrilationActionBtn, _defibrilationNatanActionBtn, _syncedFlipActionBtn, _pacingActionBtn, _pinkVenflonActionBtn, _greenVenflonActionBtn, _blueVenflonActionBtn, _orangeVenflonActionBtn, _yellowVenflonActionBtn, _aspirinActionBtn, _veinBlockerActionBtn, _loicoplastActionBtn, _injector10ActionBtn, _injector25ActionBtn, _injector5ActionBtn, _saline10ActionBtn, _saline100ActionBtn, _saline500ActionBtn, _spongetaActionBtn, _gauzePadActionBtn, _zofranActionBtn, _injector50ActionBtn, _injectorPOActionBtn, _katumaNeedleActionBtn, _nezaliFitterActionBtn, NeedleActionBtn, NitrolingualActionBtn, NarkanActionBtn, SugmadexActionBtn, _sodiumTiosolfatActionBtn, _solimedrolActionBtn, _superDropsActionBtn, _lopresorActionBtn, _magneziumActionBtn, _oralTermometerActionBtn, _ventolinActionBtn, _termadexActionBtn, _injector20ActionBtn, _fusidActionBtn, _driedPlazmaActionBtn, _panetnileActionBtn, _chanokitActionBtn, _kataminActionBtn, _calciumActionBtn, _rukoroniumActionBtn, _d5wActionBtn, _adenozinActionBtn, _adrenalineActionBtn, _optalginActionBtn, _atomidatActionBtn, _atropinActionBtn, _izokatActionBtn, _irobantActionBtn, _acamolIVActionBtn, _bCarbonetActionBtn, _highPressureStopcockActionBtn, _glucogelActionBtn, _glucose50ActionBtn, _dopaminActionBtn, _dormicomActionBtn, _dropridolActionBtn, _haperinActionBtn, _hexacfronActionBtn, _hertman;

    private void InitializeMedicPermissions()
    {
        _medicActionPermissions.Add(_callAmbulanceActionBtn);
        _medicActionPermissions.Add(_aspirinAmbuActionBtn);
        _medicActionPermissions.Add(_epipenAdultActionBtn);
        _medicActionPermissions.Add(_blueVenflonActionBtn);
        _medicActionPermissions.Add(_greenVenflonActionBtn);
        _medicActionPermissions.Add(_orangeVenflonActionBtn);
        _medicActionPermissions.Add(_pinkVenflonActionBtn);
        _medicActionPermissions.Add(_yellowVenflonActionBtn);
        _medicActionPermissions.Add(_loicoplastActionBtn);
        _medicActionPermissions.Add(_injector10ActionBtn);
        _medicActionPermissions.Add(_injector25ActionBtn);
        _medicActionPermissions.Add(_injector5ActionBtn);
        _medicActionPermissions.Add(_saline10ActionBtn);
        _medicActionPermissions.Add(_saline100ActionBtn);
        _medicActionPermissions.Add(_saline500ActionBtn);
        _medicActionPermissions.Add(_spongetaActionBtn);
        _medicActionPermissions.Add(_gauzePadActionBtn);

        foreach (Button action in _medicActionPermissions)
        {
            action.interactable = true;
        }

        _currentActionPermissions = _medicActionPermissions;
    }
    private void InitializeSeniorMedicPermissions()
    {
        _seniorMedicActionPermissions.Add(_callAmbulanceActionBtn);
        _seniorMedicActionPermissions.Add(_blueVenflonActionBtn);
        _seniorMedicActionPermissions.Add(_greenVenflonActionBtn);
        _seniorMedicActionPermissions.Add(_orangeVenflonActionBtn);
        _seniorMedicActionPermissions.Add(_pinkVenflonActionBtn);
        _seniorMedicActionPermissions.Add(_yellowVenflonActionBtn);
        _seniorMedicActionPermissions.Add(_loicoplastActionBtn);
        _seniorMedicActionPermissions.Add(_injector10ActionBtn);
        _seniorMedicActionPermissions.Add(_injector25ActionBtn);
        _seniorMedicActionPermissions.Add(_injector5ActionBtn);
        _seniorMedicActionPermissions.Add(_saline10ActionBtn);
        _seniorMedicActionPermissions.Add(_saline100ActionBtn);
        _seniorMedicActionPermissions.Add(_saline500ActionBtn);
        _seniorMedicActionPermissions.Add(_spongetaActionBtn);
        _seniorMedicActionPermissions.Add(_gauzePadActionBtn);

        _seniorMedicActionPermissions.Add(_aspirinAmbuActionBtn);
        _seniorMedicActionPermissions.Add(_epipenAdultActionBtn);
        _seniorMedicActionPermissions.Add(_infusionKitActionBtn);
        _seniorMedicActionPermissions.Add(_cPapActionBtn);
        _seniorMedicActionPermissions.Add(_lmaActionBtn);

        foreach (Button action in _seniorMedicActionPermissions)
        {
            action.interactable = true;
        }

        _currentActionPermissions = _seniorMedicActionPermissions;
    }
    private void InitializeParamedicDocPermissions()
    {
        _paramedicDocActionPermissions.Add(_callAmbulanceActionBtn);
        _paramedicDocActionPermissions.Add(_aspirinActionBtn);
        _paramedicDocActionPermissions.Add(_blueVenflonActionBtn);
        _paramedicDocActionPermissions.Add(_greenVenflonActionBtn);
        _paramedicDocActionPermissions.Add(_orangeVenflonActionBtn);
        _paramedicDocActionPermissions.Add(_pinkVenflonActionBtn);
        _paramedicDocActionPermissions.Add(_yellowVenflonActionBtn);
        _paramedicDocActionPermissions.Add(_loicoplastActionBtn);
        _paramedicDocActionPermissions.Add(_injector10ActionBtn);
        _paramedicDocActionPermissions.Add(_injector25ActionBtn);
        _paramedicDocActionPermissions.Add(_injector5ActionBtn);
        _paramedicDocActionPermissions.Add(_saline10ActionBtn);
        _paramedicDocActionPermissions.Add(_saline100ActionBtn);
        _paramedicDocActionPermissions.Add(_saline500ActionBtn);
        _paramedicDocActionPermissions.Add(_spongetaActionBtn);
        _paramedicDocActionPermissions.Add(_gauzePadActionBtn);

        _paramedicDocActionPermissions.Add(_aspirinAmbuActionBtn);
        _paramedicDocActionPermissions.Add(_epipenAdultActionBtn);
        _paramedicDocActionPermissions.Add(_infusionKitActionBtn);
        _paramedicDocActionPermissions.Add(_cPapActionBtn);
        _paramedicDocActionPermissions.Add(_lmaActionBtn);
        _paramedicDocActionPermissions.Add(_defibrilationNatanActionBtn);

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
        _paramedicDocActionPermissions.Add(_nezaliFitterActionBtn);
        _paramedicDocActionPermissions.Add(_capnoTubusActionBtn);
        _paramedicDocActionPermissions.Add(_capnoNezaliActionBtn);
        _paramedicDocActionPermissions.Add(_connectDefibrilationActionBtn);
        _paramedicDocActionPermissions.Add(_syncedFlipActionBtn);
        _paramedicDocActionPermissions.Add(_pacingActionBtn);
        _paramedicDocActionPermissions.Add(_zofranActionBtn);
        _paramedicDocActionPermissions.Add(_injector50ActionBtn);
        _paramedicDocActionPermissions.Add(_injectorPOActionBtn);
        _paramedicDocActionPermissions.Add(_katumaNeedleActionBtn);
        _paramedicDocActionPermissions.Add(_nezaliFitterActionBtn);
        _paramedicDocActionPermissions.Add(NeedleActionBtn);
        _paramedicDocActionPermissions.Add(NitrolingualActionBtn);
        _paramedicDocActionPermissions.Add(NarkanActionBtn);
        _paramedicDocActionPermissions.Add(SugmadexActionBtn);
        _paramedicDocActionPermissions.Add(_sodiumTiosolfatActionBtn);
        _paramedicDocActionPermissions.Add(_solimedrolActionBtn);
        _paramedicDocActionPermissions.Add(_superDropsActionBtn);
        _paramedicDocActionPermissions.Add(_lopresorActionBtn);
        _paramedicDocActionPermissions.Add(_magneziumActionBtn);
        _paramedicDocActionPermissions.Add(_oralTermometerActionBtn);
        _paramedicDocActionPermissions.Add(_ventolinActionBtn);
        _paramedicDocActionPermissions.Add(_termadexActionBtn);
        _paramedicDocActionPermissions.Add(_injector20ActionBtn);
        _paramedicDocActionPermissions.Add(_fusidActionBtn);
        _paramedicDocActionPermissions.Add(_driedPlazmaActionBtn);
        _paramedicDocActionPermissions.Add(_panetnileActionBtn);
        _paramedicDocActionPermissions.Add(_chanokitActionBtn);
        _paramedicDocActionPermissions.Add(_kataminActionBtn);
        _paramedicDocActionPermissions.Add(_calciumActionBtn);
        _paramedicDocActionPermissions.Add(_rukoroniumActionBtn);
        _paramedicDocActionPermissions.Add(_d5wActionBtn);
        _paramedicDocActionPermissions.Add(_adenozinActionBtn);
        _paramedicDocActionPermissions.Add(_adrenalineActionBtn);
        _paramedicDocActionPermissions.Add(_optalginActionBtn);
        _paramedicDocActionPermissions.Add(_atomidatActionBtn);
        _paramedicDocActionPermissions.Add(_atropinActionBtn);
        _paramedicDocActionPermissions.Add(_izokatActionBtn);
        _paramedicDocActionPermissions.Add(_irobantActionBtn);
        _paramedicDocActionPermissions.Add(_acamolIVActionBtn);
        _paramedicDocActionPermissions.Add(_bCarbonetActionBtn);
        _paramedicDocActionPermissions.Add(_highPressureStopcockActionBtn);
        _paramedicDocActionPermissions.Add(_glucogelActionBtn);
        _paramedicDocActionPermissions.Add(_glucose50ActionBtn);
        _paramedicDocActionPermissions.Add(_dopaminActionBtn);
        _paramedicDocActionPermissions.Add(_dormicomActionBtn);
        _paramedicDocActionPermissions.Add(_dropridolActionBtn);
        _paramedicDocActionPermissions.Add(_haperinActionBtn);
        _paramedicDocActionPermissions.Add(_hexacfronActionBtn);
        _paramedicDocActionPermissions.Add(_hertman);

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
