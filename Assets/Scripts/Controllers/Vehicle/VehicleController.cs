using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public enum VehicleSit { Driver, Passanger, Middle, LeftBack, RightBack }

public class VehicleController : MonoBehaviour
{
    private PhotonView _photonView;
    public PhotonView PhotonView { get => _photonView; set => value = _photonView; }

    private Vector2 _input;
    private float _currentSteerAngle, _currentbreakForce;
    private bool _isBreaking;

    [Header("Vehicle Components")]
    [SerializeField] private float _motorForce;
    [SerializeField] private float _breakForce, _maxSteerAngle;
    [SerializeField] private Transform _frontLeftWheelTransform, _frontRightWheeTransform, _rearLeftWheelTransform, _rearRightWheelTransform;
    [SerializeField] private WheelCollider _frontLeftWheelCollider, _frontRightWheelCollider, _rearLeftWheelCollider, _rearRightWheelCollider;
    [SerializeField] private Rigidbody _rb;

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
    public OwnershipTransfer Transfer;

    [Header("Vehicle Conditionals")]
    public bool IsNatan; 
    public bool IsCarHeadLightsOn, IsCarSirenOn, IsDriverIn, IsPassangerIn, IsMiddleIn, IsLeftBackIn, IsRightBackIn, IsInPinuy;
    public bool IsBackDoorsOpen;

    [Header("Vehicle Data")]
    public int OwnerCrew, RandomNumber;
    public string RandomName;

    [Header("Vehicle UI")]
    private GameObject _carDashboardUI;

    #region Monobehaviour Callbacks
    private void Start()
    {
        _rb.isKinematic = true;
        _photonView = GetComponent<PhotonView>();
        _carDashboardUI = UIManager.Instance.VehicleDriverUI;

        RandomNumber = GetRandomInt(100, 999 + 1);
        GameManager.Instance.usedValues.Add(RandomNumber);

        RandomName = GetRandomstring();
        GameManager.Instance.usedNamesValues.Add(RandomName);

        if (IsNatan)
            GameManager.Instance.NatanCarList.Add(_photonView);
    }
    private void FixedUpdate()
    {
        if (!CurrentDriverController)
            return;

        if (CurrentDriverController.IsDriving)
        {
            GetInput();
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
    }
    #endregion

    #region Trigger Colliders
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CollidingPlayers.Add(other.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CollidingPlayers.Remove(other.gameObject);
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
        _isBreaking = Input.GetKey(KeyCode.Space);
    }
    private void HandleMotor()
    {
        _frontLeftWheelCollider.motorTorque = _input.y * _motorForce;
        _frontRightWheelCollider.motorTorque = _input.y * _motorForce;
        _currentbreakForce = _isBreaking ? _breakForce : 0f;
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

    #region PunRPC
    [PunRPC]
    private void ToggleHeadlightsRPC()
    {
        if (IsCarHeadLightsOn)
        {
            IsCarHeadLightsOn = false;
            CarHeadLights.SetActive(false);
            CarSiren.GetComponent<Animator>().enabled = false;
            //CarSirenLightLeft.SetActive(false);
            //CarSirenLightRight.SetActive(false);
        }
        else
        {
            IsCarHeadLightsOn = true;
            CarHeadLights.SetActive(true);
            CarSiren.GetComponent<Animator>().enabled = true;
            //CarSirenLightLeft.SetActive(true);
            //CarSirenLightRight.SetActive(true);
        }
    }

    [PunRPC]
    private void ToggleSirenRPC()
    {
        if (IsCarSirenOn)
        {
            CarEmergencyLightsLeft.enabled = false;
            CarEmergencyLightsRight.enabled = false;
            IsCarSirenOn = false;
            CarSirenAudioSource.Stop();
        }
        else
        {
            CarEmergencyLightsLeft.enabled = true;
            CarEmergencyLightsRight.enabled = true;
            IsCarSirenOn = true;
            CarSirenAudioSource.Play();
        }
    }
    #endregion

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(IsInPinuy);
        }
        else
        {
            transform.position = (Vector3)stream.ReceiveNext();
            IsInPinuy = (bool)stream.ReceiveNext();
        }
    }
}
