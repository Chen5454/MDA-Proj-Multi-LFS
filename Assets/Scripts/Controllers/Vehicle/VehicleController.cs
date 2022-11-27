using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Reflection;
using TMPro;

public enum VehicleSit { Driver, Passanger, Middle, LeftBack, RightBack }

public class VehicleController : MonoBehaviour, IPunObservable, IPunInstantiateMagicCallback
{
    private PhotonView _photonView;
    public PhotonView PhotonView { get => _photonView; set => value = _photonView; }

    public Vector2 _input;
    public float _currentSteerAngle, _currentbreakForce;
    private bool _isBreaking;

    [Header("Vehicle Components")]
    [SerializeField] private float _motorForce;
    [SerializeField] private float _breakForce, _maxSteerAngle, _maxLean;
    [SerializeField] private Transform _frontLeftWheelTransform, _frontRightWheeTransform, _rearLeftWheelTransform, _rearRightWheelTransform, _centerOfMass;
    [SerializeField] private WheelCollider _frontLeftWheelCollider, _frontRightWheelCollider, _rearLeftWheelCollider, _rearRightWheelCollider;
    [SerializeField] private Rigidbody _rb;

    [SerializeField] private GameObject VehiclePos;


    public Camera VehicleCamera;
    public Animator LeftBackDoorAnimator, RightBackDoorAnimator;
    public Transform CameraFollowTransform;
    public Transform DriverSit, PassangerSit, MiddleSit, LeftBackSit, RightBackSit;
    public Transform DriverExit, PassangerExit, MiddleExit;
    public List<GameObject> CollidingPlayers;
    public PlayerController CurrentDriverController;
    public GameObject CarHeadLights, CarSiren;
    public Light CarEmergencyLightsLeft, CarEmergencyLightsRight;
    public AudioSource CarSirenAudioSource;
    public GameObject[] AllDoors;
    public OwnershipTransfer Transfer;

    [Header("Vehicle Conditionals")]
    public bool IsNatan;
    public bool IsCarHeadLightsOn, IsCarSirenOn, IsDriverIn, IsPassangerIn, IsMiddleIn, IsLeftBackIn, IsRightBackIn, IsBusy;
    public bool IsBackDoorsOpen;
    public bool IsPatientIn;
    public bool IsInMovement;

    [Header("Vehicle Data")]
    public int OwnerCrew, RandomNumber;
    public string RandomName;
    public int _ownedCrewNumber;

    [Header("Vehicle UI")]
    private GameObject _carDashboardUI;
    public List <TMP_Text> _CarNameTxt;
    public List<TMP_Text> _CarNumberTxt;

    #region Monobehaviour Callbacks

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        _carDashboardUI = UIManager.Instance.VehicleDriverUI;
    }
    private void Start()
    {
        _rb.centerOfMass = _centerOfMass.localPosition;
        _rb.isKinematic = true;

        RandomNumber = GetRandomInt(100, 999 + 1);
        GameManager.Instance.usedValues.Add(RandomNumber);

        RandomName = GetRandomstring();
        GameManager.Instance.usedNamesValues.Add(RandomName);



        if (IsNatan)
            GameManager.Instance.NatanCarList.Add(_photonView);
        else
            GameManager.Instance.AmbulanceCarList.Add(_photonView);

        AssginsTextToVehicle();

        var BusCreator = _photonView.CreatorActorNr;


    }
    private void Update()
    {
        GetInput();
    }
    private void FixedUpdate()
    {
        if (!CurrentDriverController)
            return;

        if (CurrentDriverController.IsDriving)
        {
            HandleMotor();
            HandleSteering();
            UpdateWheels();
        }
    }
    private void OnDestroy()
    {
        GameManager.Instance.usedNamesValues.Remove(RandomName);
        GameManager.Instance.usedValues.Remove(RandomNumber);

        if (IsNatan)
            GameManager.Instance.NatanCarList.Remove(_photonView);
        else
            GameManager.Instance.AmbulanceCarList.Remove(_photonView);


        if (VehiclePos != null)
        {
            VehiclePos.GetComponent<VehicleChecker>().IsPosOccupied = false;
        }
    }
    #endregion

    #region Trigger Colliders
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CollidingPlayers.Add(other.gameObject);
        }

        if (other.CompareTag("VehiclePos"))
        {
            VehiclePos = other.gameObject;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CollidingPlayers.Remove(other.gameObject);
        }
        if (other.CompareTag("VehiclePos"))
        {
            VehiclePos = null;
        }
    }
    #endregion

    #region Getters
    private int GetRandomInt(int min, int max)
    {
        int val = UnityEngine.Random.Range(min, max);
        while (GameManager.Instance.usedValues.Contains(val))
        {
            val = UnityEngine.Random.Range(min, max);
        }
        return val;
    }
    private string GetRandomstring()
    {
        int val = UnityEngine.Random.Range(0, GameManager.Instance.usedNamesValues.Count);

        while (GameManager.Instance.usedNamesValues.Contains(val.ToString()))
        {
            val = UnityEngine.Random.Range(0, GameManager.Instance.usedNamesValues.Count);
        }

        return GameManager.Instance.usedNamesValues[val];
    }
    #endregion

    #region Vehicle Movement
    private void GetInput()
    {
        _input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        _currentbreakForce = _input.y == 0 ? _breakForce / 2 : 0;

        #region Is In Movemenet

        if (_input.y != 0 || _input.x != 0)
        {
            IsInMovement = true;
        }
        else
        {
            IsInMovement = false;

        }

        #endregion

        _isBreaking = Input.GetKey(KeyCode.Space);

        if (_isBreaking)
            _currentbreakForce = _breakForce;
    }
    private void HandleMotor()
    {
        _rearLeftWheelCollider.motorTorque = _input.y * _motorForce;
        _rearRightWheelCollider.motorTorque = _input.y * _motorForce;

        if (_isBreaking)
        {
            _rearLeftWheelCollider.motorTorque = 0;
            _rearRightWheelCollider.motorTorque = 0;
            ApplyBreaking();
            return;
        }



        ApplyBreaking();
    }
    private void ApplyBreaking()
    {
        _frontRightWheelCollider.brakeTorque = _currentbreakForce;
        _frontLeftWheelCollider.brakeTorque = _currentbreakForce;
        _rearLeftWheelCollider.brakeTorque = _currentbreakForce;
        _rearRightWheelCollider.brakeTorque = _currentbreakForce;
    }
    private void HandleSteering()
    {
        _currentSteerAngle = _maxSteerAngle * _input.x;
        _frontLeftWheelCollider.steerAngle = _currentSteerAngle;
        _frontRightWheelCollider.steerAngle = _currentSteerAngle;
    }
    private void UpdateWheels()
    {
        UpdateSingleWheel(_frontLeftWheelCollider, _frontLeftWheelTransform);
        UpdateSingleWheel(_frontRightWheelCollider, _frontRightWheeTransform);
        UpdateSingleWheel(_rearRightWheelCollider, _rearRightWheelTransform);
        UpdateSingleWheel(_rearLeftWheelCollider, _rearLeftWheelTransform);
    }
    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }
    #endregion

    public void ChangeKinematicState()
    {
        if (_rb.isKinematic)
            _rb.isKinematic = false;
        else
            _rb.isKinematic = true;
    }

    public void AssginsTextToVehicle()
    {
        foreach (var CarNameTxt in _CarNameTxt)
        {
            CarNameTxt.text = RandomName;
        }
        foreach (var CarNumberTxt in _CarNumberTxt)
        {
            CarNumberTxt.text = RandomNumber.ToString();
        }
    }

    #region PunRPC
    [PunRPC]
    private void ToggleHeadlightsRPC()
    {
        if (IsCarHeadLightsOn)
        {
            CarHeadLights.SetActive(false);
            CarSiren.GetComponent<Animator>().enabled = false;
            CarEmergencyLightsLeft.enabled = false;
            CarEmergencyLightsRight.enabled = false;
            IsCarHeadLightsOn = false;
        }
        else
        {
            CarHeadLights.SetActive(true);
            CarSiren.GetComponent<Animator>().enabled = true;
            CarEmergencyLightsLeft.enabled = true;
            CarEmergencyLightsRight.enabled = true;
            IsCarHeadLightsOn = true;
        }
    }

    [PunRPC]
    private void ToggleSirenRPC()
    {
        if (IsCarSirenOn)
        {
            CarSirenAudioSource.Stop();
            IsCarSirenOn = false;
        }
        else
        {
            CarSirenAudioSource.Play();
            IsCarSirenOn = true;
        }
    }

    [PunRPC]
    private void ChangeSit(int playerViewIndex, int sitEnum, bool isEnteringVehicle)
    {
        PhotonView photonView = ActionsManager.Instance.AllPlayersPhotonViews[playerViewIndex];
        PlayerController playerController = photonView.GetComponent<PlayerController>();

        if (isEnteringVehicle)
        {
            VehicleSit desiredVehicleSit = (VehicleSit)sitEnum;

            switch (desiredVehicleSit)
            {
                case VehicleSit.Driver:
                    playerController._characterController.enabled = false;
                    photonView.transform.SetParent(DriverSit);
                    break;
                case VehicleSit.Passanger:
                    playerController._characterController.enabled = false;
                    photonView.transform.SetParent(PassangerSit);
                    break;
                case VehicleSit.Middle:
                    playerController._characterController.enabled = false;
                    photonView.transform.SetParent(MiddleSit);
                    break;
                case VehicleSit.LeftBack:
                    playerController._characterController.enabled = false;
                    photonView.transform.SetParent(LeftBackSit);
                    break;
                case VehicleSit.RightBack:
                    playerController._characterController.enabled = false;
                    photonView.transform.SetParent(RightBackSit);
                    break;
                default:
                    break;
            }

            photonView.transform.localPosition = Vector3.zero;
            photonView.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
        }
        else
        {
            photonView.transform.SetParent(transform.root.parent);
            photonView.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
        }
    }

    [PunRPC]
    public void OpenCloseBackDoorRPC()
    {
        if (IsBackDoorsOpen)
        {
            LeftBackDoorAnimator.SetBool("IsDoorOpen", false);
            RightBackDoorAnimator.SetBool("IsDoorOpen", false);
            IsBackDoorsOpen = false;
            RightBackDoorAnimator.Play("Close Back Doors");
        }
        else
        {
            LeftBackDoorAnimator.SetBool("IsDoorOpen", true);
            RightBackDoorAnimator.SetBool("IsDoorOpen", true);
            IsBackDoorsOpen = true;
        }
    }
    #endregion
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] instantiationData = info.photonView.InstantiationData;
        _ownedCrewNumber = (int)instantiationData[0];
        Debug.Log("Room Number is " + _ownedCrewNumber);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //  stream.SendNext(transform.position);
            stream.SendNext(IsBusy);
            stream.SendNext(IsDriverIn);
            stream.SendNext(IsPassangerIn);
            stream.SendNext(IsMiddleIn);
            stream.SendNext(IsLeftBackIn);
            stream.SendNext(IsRightBackIn);
            stream.SendNext(IsPatientIn);
            stream.SendNext(IsInMovement);

            foreach (var CarNameTxt in _CarNameTxt)
            {

                stream.SendNext(CarNameTxt.text);

            }
            foreach (var CarNumberTxt in _CarNumberTxt)
            {
                stream.SendNext(CarNumberTxt.text);
            }
        }
        else
        {
            // transform.position = (Vector3)stream.ReceiveNext();
            IsBusy = (bool)stream.ReceiveNext();
            IsDriverIn = (bool)stream.ReceiveNext();
            IsPassangerIn = (bool)stream.ReceiveNext();
            IsMiddleIn = (bool)stream.ReceiveNext();
            IsLeftBackIn = (bool)stream.ReceiveNext();
            IsRightBackIn = (bool)stream.ReceiveNext();
            IsPatientIn = (bool)stream.ReceiveNext();
            IsInMovement = (bool)stream.ReceiveNext();


            foreach (var CarNameTxt in _CarNameTxt)
            {

                CarNameTxt.text = (string)stream.ReceiveNext();

            }
            foreach (var CarNumberTxt in _CarNumberTxt)
            {
                CarNumberTxt.text = (string)stream.ReceiveNext();
            }
        }
    }
}
