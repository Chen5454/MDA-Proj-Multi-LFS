using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CarControllerSimple : MonoBehaviourPunCallbacks, IPunObservable
{
    private float _verticalInput;
    private float _horizontalInput;
    private float _currentbreakForce;
    private bool _isBreaking;
    private bool _isMovingBackwards;
    [SerializeField] public bool _isDrivable;
    [SerializeField] public bool IsNatan;

    [SerializeField] private float _forwardSpeed = 20;
    [SerializeField] private float _reverseSpeed = 15;
    [SerializeField] private float _turningSpeed = 2;
    [SerializeField] private float _breakForce;
    [SerializeField] private float _centerOfMassOffset;

    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheeTransform;
    [SerializeField] private Transform rearRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform;

    private Rigidbody _carRb;

    public GameObject CarHeadLights;
    public GameObject CarCollider;
    public bool CarHeadLightsOn = false;

    public GameObject CarSiren;
    public AudioSource CarSirenAudioSource;
    public bool CarSirenOn = false;

    public List<CarDoorCollision> CarDoorCollisions;

    private GameObject _carDashboardUI;
    private PhotonView _photonView;
    public OwnershipTransfer Transfer;
    public int OwnerCrew;

    public bool IsInPinuy;
    public int RandomNumber;
    public string RandomName;

     private void Awake()
     {
         _photonView = GetComponent<PhotonView>();
     }

     private void Start()
     {  
         _carRb = GetComponent<Rigidbody>();
         _carRb.centerOfMass = new Vector3(_carRb.centerOfMass.x, _centerOfMassOffset, _carRb.centerOfMass.z);
        _carDashboardUI = UIManager.Instance.VehicleUI;

        RandomNumber = GetRandomInt(100,999+1);
        GameManager.Instance.usedValues.Add(RandomNumber);

        RandomName = GetRandomstring();
        GameManager.Instance.usedNamesValues.Add(RandomName);

        if (IsNatan)
            GameManager.Instance.NatanCarList.Add(_photonView);
        //else
        //    GameManager.Instance.AmbulanceList.Add(_photonView);
     }

     private void OnDestroy() //When Car is Destroyed Delete from list for using data again later.
     {
         GameManager.Instance.usedNamesValues.Remove(RandomName);
         GameManager.Instance.usedValues.Remove(RandomNumber);
         GameManager.Instance.NatanCarList.Remove(_photonView);
     }

    private void Update()
    {
        //CheckIfDriveable();
        //GetInput();
        //CheckIsMovingBackwards();
    }

    private void FixedUpdate()
    {
        //HandleMotor();
        //HandleSteering();
        //UpdateWheels();
    }

    public void GetInput()
    {
        if (_isDrivable)
        {
            _verticalInput = Input.GetAxis("Vertical");
            _horizontalInput = Input.GetAxis("Horizontal");
            _isBreaking = Input.GetKey(KeyCode.Space);
        }
    }

    public void HandleMotor()
    {
        float moveSpeed;

        if (_verticalInput > 0)
        {
            moveSpeed = _forwardSpeed;
            IsInPinuy = true;
        }
        else if (_verticalInput < 0)
        {
            moveSpeed = -_reverseSpeed;
            IsInPinuy = true;

        }
        else
        {
            moveSpeed = 0;
            IsInPinuy = false;

        }

        _carRb.AddForce(transform.forward * moveSpeed, ForceMode.Acceleration);
    }

    private void ApplyBreaking(float moveSpeed)
    {

    }

    public void HandleSteering()
    {
        float turnSpeed = _horizontalInput * _turningSpeed * Time.deltaTime * _carRb.velocity.magnitude;
        if (_isMovingBackwards)
        {
            turnSpeed = -turnSpeed;
        }
        transform.Rotate(0, turnSpeed, 0, Space.World);
    }

    public void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheeTransform);
        UpdateSingleWheel(rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelTransform);
    }

    private void UpdateSingleWheel(Transform wheelTransform)
    {
        if (_isMovingBackwards)
        {
            wheelTransform.Rotate(-_carRb.velocity.magnitude, 0, 0);
        }
        else
        {
            wheelTransform.Rotate(_carRb.velocity.magnitude, 0, 0);
        }
    }

    public void CheckIsMovingBackwards()
    {
        if (_carRb.angularVelocity.y > 0)
        {
            _isMovingBackwards = true;
        }
        else
        {
            _isMovingBackwards = false;
        }
    }

    public void ToggleHeadlights()
    {
        if (CarHeadLightsOn)
        {
            CarHeadLightsOn = false;
            CarHeadLights.SetActive(false);
            CarSiren.GetComponent<Animator>().enabled = false;
            //CarSirenLightLeft.SetActive(false);
            //CarSirenLightRight.SetActive(false);
        }
        else
        {
            CarHeadLightsOn = true;
            CarHeadLights.SetActive(true);
            CarSiren.GetComponent<Animator>().enabled = true;
            //CarSirenLightLeft.SetActive(true);
            //CarSirenLightRight.SetActive(true);
        }
    }

    public void ToggleSiren()
    {
        if (CarSirenOn)
        {
            CarSirenOn = false;
            CarSirenAudioSource.Stop();
        }
        else
        {
            CarSirenOn = true;
            CarSirenAudioSource.Play();
        }
    }

    public void CheckIfDriveable()
    {
        foreach (CarDoorCollision item in CarDoorCollisions)
        {
            if (item.SeatNumber == 0 && item.IsSeatOccupied)
            {
                _isDrivable = true;
                _carRb.isKinematic = false;
                _carDashboardUI.SetActive(true);
            }
            else if (item.SeatNumber == 0 && !item.IsSeatOccupied)
            {
                _isDrivable = false;
                _carRb.isKinematic = true;
                _carDashboardUI.SetActive(false);
            }
        }
    }

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

    public void ExitVehicle()
    {

    }

    public int GetRandomInt(int min, int max)
    {
        int val = UnityEngine.Random.Range(min, max);
        while (GameManager.Instance.usedValues.Contains(val))
        {
            val = UnityEngine.Random.Range(min, max);
        }
        return val;
    }

    //public string GetRandomstring()
    //{
    //    string val = UnityEngine.Random.Range(0, GameManager.Instance.usedNamesValues.Count).ToString();
    //    while (GameManager.Instance.usedNamesValues.Contains(val))
    //    {
    //        val = UnityEngine.Random.Range(0, GameManager.Instance.usedNamesValues.Count).ToString();
    //    }
    //    return val;
    //}

    public string GetRandomstring()
    {
        int val = UnityEngine.Random.Range(0, GameManager.Instance.usedNamesValues.Count);

        while (GameManager.Instance.usedNamesValues.Contains(val.ToString()))
        {
            val = UnityEngine.Random.Range(0, GameManager.Instance.usedNamesValues.Count);
        }

        return GameManager.Instance.usedNamesValues[val];
    }
}
