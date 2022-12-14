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
    public Light CarEmergencyLightsLeft, CarEmergencyLightsRight;
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
        _carDashboardUI = UIManager.Instance.VehicleDriverUI;

        RandomNumber = GetRandomInt(100,999+1);
        GameManager.Instance.usedValues.Add(RandomNumber);

        RandomName = GetRandomstring();
        GameManager.Instance.usedNamesValues.Add(RandomName);

        if (IsNatan)
            GameManager.Instance.NatanCarList.Add(_photonView);
        else
            GameManager.Instance.AmbulanceCarList.Add(_photonView);
     }

     private void OnDestroy() //When Car is Destroyed Delete from list for using data again later.
     {
         GameManager.Instance.usedNamesValues.Remove(RandomName);
         GameManager.Instance.usedValues.Remove(RandomNumber);
        if (IsNatan)
            GameManager.Instance.NatanCarList.Remove(_photonView);
        else
            GameManager.Instance.AmbulanceCarList.Remove(_photonView);
    }

    private void Update()
    {
        //CheckIfDriveable();
        //GetInput();
        //CheckIsMovingBackwards();
    }

    private void FixedUpdate()
    {
        _carRb.AddForce(Vector3.down * 9);
    }

    public void GetInput()
    {
        if (_isDrivable)
        {
            _horizontalInput = Input.GetAxis("Horizontal");
            _verticalInput = Input.GetAxis("Vertical");
            _isBreaking = Input.GetKey(KeyCode.Space);
        }
    }

    public void HandleMotor()
    {
        float moveSpeed;

        if (_verticalInput > 0)
        {
            moveSpeed = _forwardSpeed;
        }
        else if (_verticalInput < 0)
        {
            moveSpeed = -_reverseSpeed;
        }
        else
        {
            moveSpeed = 0;
        }

        Vector3 moveVector = transform.forward * moveSpeed;
        moveVector.y = 0;

        _carRb.AddForce(moveVector);
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
        if (_isDrivable && Input.GetKey(KeyCode.S))
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
        _photonView.RPC("ToggleHeadlightsRPC", RpcTarget.AllViaServer);
    }

    public void ToggleSiren()
    {
        _photonView.RPC("ToggleSirenRPC", RpcTarget.AllViaServer);
    }

    public void CheckIfDriveable()
    {
        foreach (CarDoorCollision item in CarDoorCollisions)
        {
            if (item.SeatNumber == 0 && item.IsSeatOccupied)
            {
                _isDrivable = true;
                _carRb.isKinematic = false;
                //_carDashboardUI.SetActive(true);
            }
            else if (item.SeatNumber == 0 && !item.IsSeatOccupied)
            {
                _isDrivable = false;
                _carRb.isKinematic = true;
                //_carDashboardUI.SetActive(false);
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

    #region PunRPC
    [PunRPC]
    private void ToggleHeadlightsRPC()
    {
        for (int i = 0; i < GameManager.Instance.AmbulanceCarList.Count; i++)
        {
            if (GameManager.Instance.AmbulanceCarList[i].IsMine)
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
            else
            {
                for (int j = 0; j < GameManager.Instance.NatanCarList.Count; j++)
                {
                    if (GameManager.Instance.NatanCarList[i].IsMine)
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
                }
            }   
        }
    }

    [PunRPC]
    private void ToggleSirenRPC()
    {
        for (int i = 0; i < GameManager.Instance.AmbulanceCarList.Count; i++)
        {
            if (GameManager.Instance.AmbulanceCarList[i].IsMine)
            {
                if (GameManager.Instance.AmbulanceCarList[i].IsMine)
                {
                    if (CarSirenOn)
                    {
                        CarEmergencyLightsLeft.enabled = false;
                        CarEmergencyLightsRight.enabled = false;
                        CarSirenOn = false;
                        CarSirenAudioSource.Stop();
                    }
                    else
                    {
                        CarEmergencyLightsLeft.enabled = true;
                        CarEmergencyLightsRight.enabled = true;
                        CarSirenOn = true;
                        CarSirenAudioSource.Play();
                    }
                }
            }
            else
            {
                for (int j = 0; j < GameManager.Instance.NatanCarList.Count; j++)
                {
                    if (GameManager.Instance.NatanCarList[i].IsMine)
                    {
                        if (CarSirenOn)
                        {
                            CarEmergencyLightsLeft.enabled = false;
                            CarEmergencyLightsRight.enabled = false;
                            CarSirenOn = false;
                            CarSirenAudioSource.Stop();
                        }
                        else
                        {
                            CarEmergencyLightsLeft.enabled = true;
                            CarEmergencyLightsRight.enabled = true;
                            CarSirenOn = true;
                            CarSirenAudioSource.Play();
                        }
                    }
                }
            }
        }
    }
    #endregion
}
