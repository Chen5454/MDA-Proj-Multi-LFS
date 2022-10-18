using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;
using static UnityEngine.Rendering.DebugUI;

public class EmergencyBedController : MonoBehaviourPunCallbacks, IPunObservable
{
    /*
    [Header("ParentVehicle")]
    [field: SerializeField] private VehicleController _parentVehicle;
    public VehicleController ParentVehicle => _parentVehicle;

    [Header("Player & Patient")]
    [SerializeField] private GameObject _patient;
    [SerializeField] private GameObject _player;

    [Header("Emergency Bed States")]
    [SerializeField] private GameObject _emergencyBedOpen;
    [SerializeField] private GameObject _emergencyBedClosed, _emergencyBed;

    [Header("UI")]
    [field: SerializeField] private GameObject _emergencyBedUI;
    public GameObject EmergencyBedUI => _emergencyBedUI;

    [SerializeField] private TextMeshProUGUI _takeReturnText;
    [SerializeField] private TextMeshProUGUI _followUnfollowText, _placeRemovePatientText;
    [SerializeField] private string _takeText, _returnText, _followText, _unfollowText, _placeText, _removeText;

    [Header("Positions")]
    [SerializeField] private Transform _playerHoldPos;
    [SerializeField] private Transform _patientPosOnBed, _patientPosOffBed, _emergencyBedPositionInsideVehicle, _emergencyBedPositionOutsideVehicle;

    [Header("Booleans")]
    public bool IsPatientOnBed;
    [SerializeField] private bool _takeOutBed, _isBedClosed, _isFollowingPlayer, _inCar;

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
        }
    }
    
    public void ShowInteractionsToggle()
    {
        if (_emergencyBedUI.activeInHierarchy && _photonView.IsMine)
        {
            _emergencyBedUI.SetActive(false);
            UIManager.Instance.CurrentActionBarParent.SetActive(true);
        }
        else if (!_emergencyBedUI.activeInHierarchy && _photonView.IsMine)
        {
            _emergencyBedUI.SetActive(true);
            UIManager.Instance.CurrentActionBarParent.SetActive(false);
        }

        else if(!_isFollowingPlayer && !_emergencyBedUI.activeInHierarchy)
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
            _photonView.RPC("SetThisInactive", RpcTarget.AllBufferedViaServer);
        }
        else if (!_isBedClosed)
        {
            _isBedClosed = true;
            _photonView.RPC("SetThisActive", RpcTarget.AllBufferedViaServer);
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
                EmergencyBedUI.SetActive(false);

                //_isFacingTrolley = false;
                _photonView.RPC("SetBedParent", RpcTarget.AllBufferedViaServer, PhotonNetwork.NickName);
                _followUnfollowText.text = _followText;
            }
            else if (!_isFollowingPlayer)
            {
                _isFollowingPlayer = true;
                UIManager.Instance.CurrentActionBarParent.SetActive(false);
                EmergencyBedUI.SetActive(true);

                _player.transform.position = _playerHoldPos.position;
                _player.transform.LookAt(transform.position);
                _photonView.RPC("SetBedParent", RpcTarget.AllBufferedViaServer, PhotonNetwork.NickName);
                _followUnfollowText.text = _unfollowText;
            }
            _emergencyBedUI.SetActive(false);
        }
    }
    
    public void PutRemovePatient()
    {
        if (_patient != null && !IsPatientOnBed)
        {
             _photonView.RPC("PutOnBed", RpcTarget.AllBufferedViaServer);
        }
        else if (_patient != null && IsPatientOnBed && !_inCar)
        {
            _photonView.RPC("RemoveFromBed", RpcTarget.AllBufferedViaServer);
        }
    }
    
    public void TakeOutReturnBedToggle()
    {
        if (_inCar && !_takeOutBed)
        {
            _takeOutBed = true;
            _inCar = false;
            _emergencyBedUI.SetActive(true);
            transform.SetPositionAndRotation(_emergencyBedPositionOutsideVehicle.position, _emergencyBedPositionOutsideVehicle.rotation);
            FollowPlayerToggle();
            _takeReturnText.text = _returnText;
        }
        else if (_inCar && _takeOutBed)
        {
            _takeOutBed = false;
            _emergencyBedUI.SetActive(false);
        }
        else if (!_inCar && _takeOutBed)
        {
            _takeOutBed = false;
            _inCar = true;
            _isFollowingPlayer = false;
            transform.position = _emergencyBedPositionInsideVehicle.position;
            transform.rotation = _emergencyBedPositionInsideVehicle.rotation;
            transform.SetParent(_emergencyBedPositionInsideVehicle);
            _takeReturnText.text = _takeText;
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
            if (!IsPatientOnBed)
            {
                _patient = other.gameObject;
                _patient.layer = (int)LayerMasks.Default;
            }
        }

        //if (other.CompareTag("Car"))
        //{
        //    _inCar = true;
        //}

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
            if (!IsPatientOnBed)
            {
                _patient.layer = (int)LayerMasks.Interactable;
                _patient = null;
            }
        }
        //if (other.CompareTag("Car"))
        //{
        //    _inCar = false;
        //}
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
            // stream.SendNext(transform.position);
            //stream.SendNext(transform.rotation);
            stream.SendNext(_emergencyBedPositionInsideVehicle.position);
            stream.SendNext(_emergencyBedPositionOutsideVehicle.position);
            stream.SendNext(_isBedClosed);
            stream.SendNext(IsPatientOnBed);
            stream.SendNext(_isFollowingPlayer);
            stream.SendNext(_inCar);
            stream.SendNext(_takeOutBed);
            stream.SendNext(_patientPosOnBed.position);
        }
        else
        {
            // transform.position = (Vector3)stream.ReceiveNext();
            // transform.rotation = (Quaternion)stream.ReceiveNext();
            _emergencyBedPositionInsideVehicle.position = (Vector3)stream.ReceiveNext();
            _emergencyBedPositionOutsideVehicle.position = (Vector3)stream.ReceiveNext();
            _isBedClosed = (bool)stream.ReceiveNext();
            IsPatientOnBed = (bool)stream.ReceiveNext();
            _isFollowingPlayer = (bool)stream.ReceiveNext();
            _inCar = (bool)stream.ReceiveNext();
            _takeOutBed = (bool)stream.ReceiveNext();
            _patientPosOnBed.position = (Vector3)stream.ReceiveNext();
        }

    }


    [PunRPC]
    void PutOnBed()
    {
        _patient.layer = (int)LayerMasks.Default;
        _patient.GetComponent<BoxCollider>().enabled = false;
        _patient.transform.SetPositionAndRotation(_patientPosOnBed.position, _patientPosOnBed.rotation); // parent
        _patient.transform.SetParent(this.transform);// parent
        _placeRemovePatientText.text = _removeText;
        IsPatientOnBed = true;
    }

    [PunRPC]
    void RemoveFromBed()
    {
        _patient.layer = (int)LayerMasks.Interactable;
        _patient.GetComponent<BoxCollider>().enabled = true;
        _patient.transform.position = _patientPosOffBed.position;// parent
        _patient.transform.SetParent(null);// parent
        _placeRemovePatientText.text = _placeText;
        IsPatientOnBed = false;
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

    [PunRPC]
    private void SetBedParent(string currentPlayer)
    {
        PhotonView currentPlayerView = ActionsManager.Instance.GetPlayerPhotonViewByNickName(currentPlayer);


        if (_isFollowingPlayer)
            gameObject.transform.SetParent(currentPlayerView.transform);

        else if (!_isFollowingPlayer)
            gameObject.transform.SetParent(null);

    }

}
*/
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    [Header("ParentVehicle")]
    [field: SerializeField] private VehicleController _parentVehicle;
    public VehicleController ParentVehicle => _parentVehicle;

    [Header("Player & Patient")]
    [SerializeField] private GameObject _patient;
    [SerializeField] private GameObject _player;

    [Header("Emergency Bed States")]
    [SerializeField] private GameObject _emergencyBedOpen;
    [SerializeField] private GameObject _emergencyBedClosed, _emergencyBed;

    [Header("UI")]
    [field: SerializeField] private GameObject _emergencyBedUI;
    public GameObject EmergencyBedUI => _emergencyBedUI;

    [SerializeField] private TextMeshProUGUI _takeReturnText;
    [SerializeField] private TextMeshProUGUI _followUnfollowText, _placeRemovePatientText;
    [SerializeField] private string _takeText, _returnText, _followText, _unfollowText, _placeText, _removeText;

    [Header("Positions")]
    [SerializeField] private Transform _playerHoldPos;
    [SerializeField] private Transform _patientPosOnBed, _patientPosOffBed, _emergencyBedPositionInsideVehicle, _emergencyBedPositionOutsideVehicle;

    [Header("Booleans")]
    public bool IsPatientOnBed;
    [SerializeField] private bool _isBedOut, _isFollowingPlayer = false, _inCar = true;
    [field: SerializeField] private bool _isBedClosed = true;
    public bool IsBedClosed { get => _isBedClosed; set { ChangeIsBedClosed(value); } }

    [Header("Layers")]
    [SerializeField] private int _interactableLayerNum;
    [SerializeField] private int _defaultLayerNum;

    private PhotonView _photonView;

    //[SerializeField] private PhotonView _photonView;
    public OwnershipTransfer _transfer;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }

    void Start()
    {
        ChangeIsBedClosed(true);

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
            IsBedClosed = true;
        }
        else if (!_inCar)
        {
            //_emergencyBed.GetComponent<BoxCollider>().isTrigger = false;
            IsBedClosed = false;
        }

        // Fold
        //FoldUnfold();

        // Follow Player
        FollowPlayer();

        // Take Out Bed
        //TakeOutReturnBed();
    }

    public void ShowInteractionsToggle()
    {
        if (_emergencyBedUI.activeInHierarchy && _photonView.IsMine)
        {
            _emergencyBedUI.SetActive(false);
            UIManager.Instance.CurrentActionBarParent.SetActive(true);
        }
        else if (!_emergencyBedUI.activeInHierarchy && _photonView.IsMine)
        {
            _emergencyBedUI.SetActive(true);
            UIManager.Instance.CurrentActionBarParent.SetActive(false);
        }

        else if (!_isFollowingPlayer && !_emergencyBedUI.activeInHierarchy)
        {
            _transfer.BedPickUp();
            _emergencyBedUI.SetActive(true);
            UIManager.Instance.CurrentActionBarParent.SetActive(false);
        }
    }

    private void ChangeIsBedClosed(bool value)
    {
        if (_isBedClosed != value)
        {
            _isBedClosed = value;

            if (value)
                _photonView.RPC("SetBedClosedRPC", RpcTarget.AllBufferedViaServer);
            else
                _photonView.RPC("SetBedOpenRPC", RpcTarget.AllBufferedViaServer);
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
    //private void FoldUnfold()
    //{
    //    if (_isBedClosed)
    //    {
    //        _photonView.RPC("SetThisActive", RpcTarget.AllBufferedViaServer);
    //
    //    }
    //    else if (!_isBedClosed)
    //    {
    //        _photonView.RPC("SetThisInactive", RpcTarget.AllBufferedViaServer);
    //
    //
    //    }
    //}
    public void FollowPlayerToggle()
    {
        if (_player != null && _isBedOut)
        {
            if (_isFollowingPlayer)
            {
                _isFollowingPlayer = false;
                _photonView.RPC("SetBedParent", RpcTarget.AllBufferedViaServer, PhotonNetwork.NickName);
                _followUnfollowText.text = _followText;
                UIManager.Instance.CurrentActionBarParent.SetActive(true);
            }
            else if (!_isFollowingPlayer)
            {
                _isFollowingPlayer = true;
                _player.transform.position = _playerHoldPos.position;
                _player.transform.LookAt(transform.position);
                _photonView.RPC("SetBedParent", RpcTarget.AllBufferedViaServer, PhotonNetwork.NickName);
                _followUnfollowText.text = _unfollowText;
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
                _photonView.RPC("SetBedParent", RpcTarget.AllBufferedViaServer, PhotonNetwork.NickName);
                _followUnfollowText.text = _unfollowText;
            }
            else if (!_isFollowingPlayer)
            {
                //_isFacingTrolley = false;
                _photonView.RPC("SetBedParent", RpcTarget.AllBufferedViaServer, PhotonNetwork.NickName);
                _followUnfollowText.text = _followText;
            }
        }
    }

    public void PutRemovePatient()
    {
        if (_patient != null && !IsPatientOnBed)
        {
            _photonView.RPC("PutOnBed", RpcTarget.AllBufferedViaServer);

        }
        else if (_patient != null && IsPatientOnBed && !_inCar)
        {
            _photonView.RPC("RemoveFromBed", RpcTarget.AllBufferedViaServer);

        }
    }

    public void TakeOutReturnBedToggle()
    {
        if (_inCar && !_isBedOut)
        {
            _emergencyBedUI.SetActive(true);
            transform.SetPositionAndRotation(_emergencyBedPositionOutsideVehicle.position, _emergencyBedPositionOutsideVehicle.rotation);
            transform.SetParent(_emergencyBedPositionOutsideVehicle);

            _inCar = false;
            _isBedOut = true;

            _takeReturnText.text = _returnText;
            FollowPlayerToggle();
        }
        else if (!_inCar && _isBedOut)
        {
            if (_isFollowingPlayer)
                FollowPlayerToggle();

            _emergencyBedUI.SetActive(false);
            transform.SetPositionAndRotation(_emergencyBedPositionInsideVehicle.position, _emergencyBedPositionInsideVehicle.rotation);
            transform.SetParent(_emergencyBedPositionInsideVehicle);

            _inCar = true;
            _isBedOut = false;

            _takeReturnText.text = _takeText;
        }
    }

    private void TakeOutReturnBed()
    {
        if (_inCar && !_isBedOut)
        {
            if (_isFollowingPlayer)
                FollowPlayerToggle();

            transform.SetPositionAndRotation(_emergencyBedPositionInsideVehicle.position, _emergencyBedPositionInsideVehicle.rotation);
            transform.SetParent(_emergencyBedPositionInsideVehicle);
            _takeReturnText.text = _takeText;
        }
        else if (!_inCar && _isBedOut)
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
            if (!IsPatientOnBed)
            {
                _patient = other.gameObject;
                _patient.layer = (int)LayerMasks.Default;
            }
        }

        //if (other.CompareTag("Car"))
        //{
        //    _inCar = true;
        //}

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
            if (!IsPatientOnBed)
            {
                _patient.layer = (int)LayerMasks.Interactable;
                _patient = null;
            }
        }
        //if (other.CompareTag("Car"))
        //{
        //    _inCar = false;
        //}
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
            // stream.SendNext(transform.position);
            //stream.SendNext(transform.rotation);
            stream.SendNext(_emergencyBedPositionInsideVehicle.position);
            stream.SendNext(_emergencyBedPositionOutsideVehicle.position);
            stream.SendNext(_isBedClosed);
            stream.SendNext(IsPatientOnBed);
            stream.SendNext(_isFollowingPlayer);
            stream.SendNext(_inCar);
            stream.SendNext(_isBedOut);
            stream.SendNext(_patientPosOnBed.position);
        }
        else
        {
            // transform.position = (Vector3)stream.ReceiveNext();
            // transform.rotation = (Quaternion)stream.ReceiveNext();
            _emergencyBedPositionInsideVehicle.position = (Vector3)stream.ReceiveNext();
            _emergencyBedPositionOutsideVehicle.position = (Vector3)stream.ReceiveNext();
            IsBedClosed = (bool)stream.ReceiveNext();
            IsPatientOnBed = (bool)stream.ReceiveNext();
            _isFollowingPlayer = (bool)stream.ReceiveNext();
            _inCar = (bool)stream.ReceiveNext();
            _isBedOut = (bool)stream.ReceiveNext();
            _patientPosOnBed.position = (Vector3)stream.ReceiveNext();
        }

    }


    [PunRPC]
    void PutOnBed()
    {
        _patient.layer = (int)LayerMasks.Default;
        _patient.GetComponent<BoxCollider>().enabled = false;
        _patient.transform.SetPositionAndRotation(_patientPosOnBed.position, _patientPosOnBed.rotation); // parent
        _patient.transform.SetParent(this.transform);// parent
        _placeRemovePatientText.text = _removeText;
        IsPatientOnBed = true;
    }

    [PunRPC]
    void RemoveFromBed()
    {
        _patient.layer = (int)LayerMasks.Interactable;
        _patient.GetComponent<BoxCollider>().enabled = true;
        _patient.transform.position = _patientPosOffBed.position;// parent
        _patient.transform.SetParent(null);// parent
        _placeRemovePatientText.text = _placeText;
        IsPatientOnBed = false;
    }

    [PunRPC]
    void SetBedOpenRPC()
    {
        _emergencyBedOpen.SetActive(true);
        _emergencyBedClosed.SetActive(false);
    }

    [PunRPC]
    void SetBedClosedRPC()
    {
        _emergencyBedOpen.SetActive(false);
        _emergencyBedClosed.SetActive(true);
    }

    [PunRPC]
    private void SetBedParent(string currentPlayer)
    {
        PhotonView currentPlayerView = ActionsManager.Instance.GetPlayerPhotonViewByNickName(currentPlayer);


        if (_isFollowingPlayer)
            gameObject.transform.SetParent(currentPlayerView.transform);

        else if (!_isFollowingPlayer)
            gameObject.transform.SetParent(null);

    }
}