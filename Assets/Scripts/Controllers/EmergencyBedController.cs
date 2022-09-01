using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;

public class EmergencyBedController : MonoBehaviourPunCallbacks,IPunObservable
{
    [Header("Player & Patient")]
    [SerializeField] private GameObject _patient;
    [SerializeField] private GameObject _player;

    [Header("Emergency Bed States")]
    [SerializeField] private GameObject _emergencyBedOpen;
    [SerializeField] private GameObject _emergencyBedClosed, _emergencyBed;

    [Header("UI")]
    [SerializeField] private GameObject _emergencyBedUI;
    [SerializeField] private TextMeshProUGUI _takeReturnText;
    [SerializeField] private TextMeshProUGUI _followUnfollowText, _placeRemovePatientText;
    [SerializeField] private string _takeText, _returnText, _followText, _unfollowText, _placeText, _removeText;

    [Header("Positions")]
    [SerializeField] private Transform _playerHoldPos;
    [SerializeField] private Transform _patientPosOnBed, _patientPosOffBed, _emergencyBedPositionInsideVehicle, _emergencyBedPositionOutsideVehicle;

    [Header("Booleans")]
    [SerializeField] private bool _takeOutBed;
    [SerializeField] private bool _isBedClosed, _isPatientOnBed, _isFollowingPlayer, _inCar;


    private PhotonView _photonView;

    //[SerializeField] private PhotonView _photonView;
    public OwnershipTransfer _transfer;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }

    void Start()
    {
        if (photonView.IsMine)
        {
            _emergencyBedUI.SetActive(false);
        }
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            AlwaysChecking();

        }
    }

    public void AlwaysChecking()
    {
        // In Car
        if (_inCar)
        {
            //_emergencyBed.GetComponent<BoxCollider>().isTrigger = true;
            _isBedClosed = true;
        }
        else if (!_inCar)
        {
            //_emergencyBed.GetComponent<BoxCollider>().isTrigger = false;
            _isBedClosed = false;
        }
    
        // Fold
        FoldUnfold();
    
        // Follow Player
        FollowPlayer();
    
        // Take Out Bed
        TakeOutReturnBed();
    }
    
    public void ShowInteractionsToggle()
    {
        if (_emergencyBedUI.activeInHierarchy)
        {
            _emergencyBedUI.SetActive(false);
            UIManager.Instance.CurrentActionBarParent.SetActive(true);
        }
        else
        {
            _transfer.BedPickUp();
            _emergencyBedUI.SetActive(true);
            UIManager.Instance.CurrentActionBarParent.SetActive(false);
        }
    }
    
    public void FoldUnfoldToggle()
    {
        if (_isBedClosed)
        {
            _isBedClosed = false;
        }
        else if (!_isBedClosed)
        {
            _isBedClosed = true;
        }
    }
    
    private void FoldUnfold()
    {
        if (_isBedClosed)
        {
            _photonView.RPC("SetThisActive", RpcTarget.AllBufferedViaServer);

        }
        else if (!_isBedClosed)
        {
            _photonView.RPC("SetThisInactive", RpcTarget.AllBufferedViaServer);


        }
    }
    
    public void FollowPlayerToggle()
    {
        if (_player != null && _takeOutBed)
        {
            if (_isFollowingPlayer)
            {
                _isFollowingPlayer = false;
                UIManager.Instance.CurrentActionBarParent.SetActive(true);
            }
            else if (!_isFollowingPlayer)
            {
                _isFollowingPlayer = true;
                UIManager.Instance.CurrentActionBarParent.SetActive(false);
            }
            _emergencyBedUI.SetActive(false);
        }
    }
    
    private void FollowPlayer()
    {
        if (_player != null)
        {
            if (_isFollowingPlayer)
            {
                _player.transform.position = _playerHoldPos.position;
                _player.transform.LookAt(transform.position);
                gameObject.transform.SetParent(_player.transform);
                _followUnfollowText.text = _unfollowText;
            }
            else if (!_isFollowingPlayer)
            {
                //_isFacingTrolley = false;
                gameObject.transform.SetParent(null);
                _followUnfollowText.text = _followText;
            }
        }
    }
    
    public void PutRemovePatient()
    {
        if (_patient != null && !_isPatientOnBed)
        {
             _photonView.RPC("PutOnBed", RpcTarget.AllBufferedViaServer);

        }
        else if (_patient != null && _isPatientOnBed && !_inCar)
        {
            _photonView.RPC("RemoveFromBed", RpcTarget.AllBufferedViaServer);

        }
    }
    
    public void TakeOutReturnBedToggle()
    {
        if (_inCar && !_takeOutBed)
        {
            _takeOutBed = true;
            _emergencyBedUI.SetActive(true);
            transform.SetPositionAndRotation(_emergencyBedPositionOutsideVehicle.position, _emergencyBedPositionOutsideVehicle.rotation);
            FollowPlayerToggle();
        }
        else if (_inCar && _takeOutBed)
        {
            _takeOutBed = false;
            _emergencyBedUI.SetActive(false);
        }
        else if (!_inCar && _takeOutBed)
        {
            _takeOutBed = false;
            transform.position = _emergencyBedPositionInsideVehicle.position;
            transform.rotation = _emergencyBedPositionInsideVehicle.rotation;
            transform.SetParent(_emergencyBedPositionInsideVehicle);
        }
    }
    
    private void TakeOutReturnBed()
    {
        if (_inCar && !_takeOutBed)
        {
            _isFollowingPlayer = false;
            transform.position = _emergencyBedPositionInsideVehicle.position;
            transform.rotation = _emergencyBedPositionInsideVehicle.rotation;
            transform.SetParent(_emergencyBedPositionInsideVehicle);
            _takeReturnText.text = _takeText;
        }
        else if (_inCar && _takeOutBed)
        {
            _takeReturnText.text = _returnText;
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            _player = other.gameObject;

        }

        if (other.CompareTag("Patient"))
        {
            if (!_isPatientOnBed)
            {
                _patient = other.gameObject;
            }
        }

        if (other.CompareTag("Car"))
        {
            _inCar = true;
        }

        if (other.CompareTag("Evac"))
        {
                _patient.GetComponent<BoxCollider>().enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _player = null;
            //InteractionsBar.SetActive(false);
        }
        if (other.CompareTag("Patient"))
        {
            if (!_isPatientOnBed)
            {
                _patient = null;
            }
        }
        if (other.CompareTag("Car"))
        {
            _inCar = false;
        }
        if (other.CompareTag("Evac"))
        {
            _patient.GetComponent<BoxCollider>().enabled = false;
        }
    }


    public void ReturnBackBack()
    {
        if (!_patient.gameObject.activeInHierarchy)
        {
            transform.position = _emergencyBedPositionInsideVehicle.position;
            transform.rotation = _emergencyBedPositionInsideVehicle.rotation;
            transform.SetParent(_emergencyBedPositionInsideVehicle);
        }

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(_emergencyBedPositionInsideVehicle.position);
            stream.SendNext(_emergencyBedPositionOutsideVehicle.position);
            stream.SendNext(_isBedClosed);
            stream.SendNext(_isPatientOnBed);
            stream.SendNext(_isFollowingPlayer);
            stream.SendNext(_inCar);
            stream.SendNext(_takeOutBed);
            stream.SendNext(_patientPosOnBed.position);
        }
        else
        {
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
            _emergencyBedPositionInsideVehicle.position = (Vector3)stream.ReceiveNext();
            _emergencyBedPositionOutsideVehicle.position = (Vector3)stream.ReceiveNext();
            _isBedClosed = (bool)stream.ReceiveNext();
            _isPatientOnBed = (bool)stream.ReceiveNext();
            _isFollowingPlayer = (bool)stream.ReceiveNext();
            _inCar = (bool)stream.ReceiveNext();
            _takeOutBed = (bool)stream.ReceiveNext();
            _patientPosOnBed.position = (Vector3)stream.ReceiveNext();
        }

    }


    [PunRPC]
    void PutOnBed()
    {
        _isPatientOnBed = true;
        _patient.GetComponent<BoxCollider>().enabled = false;
        _patient.transform.SetPositionAndRotation(_patientPosOnBed.position, _patientPosOnBed.rotation); // parent
        _patient.transform.SetParent(this.transform);// parent
        _placeRemovePatientText.text = _removeText;
        _emergencyBedUI.SetActive(false);
    }

    [PunRPC]
    void RemoveFromBed()
    {
        _isPatientOnBed = false;
        _patient.GetComponent<BoxCollider>().enabled = true;
        _patient.transform.position = _patientPosOffBed.position;// parent
        _patient.transform.SetParent(null);// parent
        _placeRemovePatientText.text = _placeText;
        _emergencyBedUI.SetActive(false);
    }

    [PunRPC]
    void SetThisActive()
    {

        _emergencyBedOpen.SetActive(false);
        _emergencyBedClosed.SetActive(true);
    }

    [PunRPC]
    void SetThisInactive()
    {
        _emergencyBedOpen.SetActive(true);
        _emergencyBedClosed.SetActive(false);
    }



}
