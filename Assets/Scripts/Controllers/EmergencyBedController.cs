using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;
using TMPro;
using static UnityEngine.Rendering.DebugUI;

public class EmergencyBedController : MonoBehaviourPunCallbacks, IPunObservable
{
    
    [Header("ParentVehicle")]
    [field: SerializeField] private VehicleController _parentVehicle;
    public VehicleController ParentVehicle => _parentVehicle;

    public SmoothSyncMovement _SmoothSync;

    [Header("Player & Patient")]
    [SerializeField] private GameObject _patient;
    private Patient _patientScript;
    [SerializeField] public GameObject _player;

    [Header("Emergency Bed States")]

    [SerializeField] private GameObject _emergencyBedOpen;
    [SerializeField] private GameObject _emergencyBedClosed, _emergencyBed;
    [SerializeField] private GameObject _emergencyBedModelParent;


    [Header("UI")]
    [field: SerializeField] private GameObject _emergencyBedUI;
    public GameObject EmergencyBedUI => _emergencyBedUI;
    [SerializeField] private GameObject PatientMenuParentUI;
    [SerializeField] private GameObject JoinPatientParentUI;
    [SerializeField] private GameObject TagMiunParentUI;



    [SerializeField] private TextMeshProUGUI _takeReturnText;
    [SerializeField] private TextMeshProUGUI _followUnfollowText, _placeRemovePatientText;
    [SerializeField] private string _takeText, _returnText, _followText, _unfollowText, _placeText, _removeText;

    [Header("Positions")]
    [SerializeField] private Transform _playerHoldPos;
    [SerializeField] private Transform _patientPosOnBed, _patientPosOffBed, _emergencyBedPositionInsideVehicle, _emergencyBedPositionOutsideVehicle;

    [Header("Booleans")]
    public bool IsPatientOnBed;
    [SerializeField] private bool _isBedOut, _inCar = true;
    public bool  _isFollowingPlayer = false;
    public bool insideCar = true;
    [field: SerializeField] private bool _isBedClosed = true;
    public bool IsBedClosed { get => _isBedClosed; set { ChangeIsBedClosed(value); } }

    [Header("Layers")]
    [SerializeField] private int _interactableLayerNum;
    [SerializeField] private int _defaultLayerNum;

    private PhotonView _photonView;
    public OwnershipTransfer _transfer;
    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        #region PatientUI
        PatientMenuParentUI = UIManager.Instance.PatientMenu;
        JoinPatientParentUI = UIManager.Instance.JoinPatientPopUp;
        TagMiunParentUI = UIManager.Instance.TagMiunMenu;
        GameManager.Instance.AllBeds.Add(this._photonView);
        #endregion
    }

    void Start()
    {
        _emergencyBedModelParent.layer = (int)LayerMasks.Default;

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

        PatientReadyToEvac();

        if (ParentVehicle.IsDestroy)
        {
            _photonView.RPC("DestroyBedOnReset", RpcTarget.AllBufferedViaServer);
        }
    }

    [PunRPC]
    void DestroyBedOnReset()
    {
        Destroy(this.gameObject);
    }


    public Player GetPlayerOwner()
    {
        for (int i = 0; i < ActionsManager.Instance.AllPlayersPhotonViews.Count; i++)
        {
            PlayerController desiredPlayer = ActionsManager.Instance.AllPlayersPhotonViews[i].GetComponent<PlayerController>();

            if (desiredPlayer._photonView.Owner == _photonView.Owner)
            {
                Player Player = desiredPlayer.GetComponent<PhotonView>().Owner;
                Debug.Log("This Owner is : "+ Player.NickName);
                return Player;
            }
        }
        return null;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _player = other.gameObject;
            _emergencyBedModelParent.layer = (int)LayerMasks.Interactable;
        }

        if (other.CompareTag("Patient"))
        {
            if (!IsPatientOnBed)
            {
                _patient = other.gameObject;
                _patientScript = _patient.GetComponent<Patient>();

               

            }
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
            _emergencyBedModelParent.layer = (int)LayerMasks.Default;

        }

        if (other.CompareTag("Patient"))
        {
            if (!IsPatientOnBed)
            {
                _patient = null;
            }
        }

        if (other.CompareTag("Evac"))
        {
            if (_patient != null)
            {
                _patient.GetComponent<BoxCollider>().enabled = false;
            }
           
        }
    }

    public void AlwaysChecking()
    {
        if (_inCar)
            IsBedClosed = true;
        else if (!_inCar)
            IsBedClosed = false;

        if (_patient)
        {
            if (IsPatientOnBed)
                _patient.GetComponentInChildren<MakeItAButton>().gameObject.layer = (int)LayerMasks.Default;
            else
                _patient.GetComponentInChildren<MakeItAButton>().gameObject.layer = (int)LayerMasks.Interactable;
        }



        FollowPlayer();
      //  InteractiveLayerToggles();
    }

    public void InteractiveLayerToggles()
    {
        if (PatientMenuParentUI.activeInHierarchy || JoinPatientParentUI.activeInHierarchy|| TagMiunParentUI.activeInHierarchy)
        {
            _emergencyBedModelParent.layer = (int)LayerMasks.Default;
        }
        else
        {
            _emergencyBedModelParent.layer = (int)LayerMasks.Interactable;

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

        else if (!_isFollowingPlayer && !_emergencyBedUI.activeInHierarchy)
        {
            _transfer.BedPickUp();
            _emergencyBedUI.SetActive(true);
            UIManager.Instance.CurrentActionBarParent.SetActive(false);
        }
    }

    public void PatientReadyToEvac()
    {
        if (_inCar && IsPatientOnBed)
        {
            ParentVehicle.IsPatientIn = true;
        }
        else
        {
            ParentVehicle.IsPatientIn = false;
        }

      //  bool isInCar = _inCar && IsPatientOnBed ? ParentVehicle.IsPatientIn : ParentVehicle.IsPatientIn;
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

    public void TakeOutReturnBedToggle()
    {
        if (_inCar && !_isBedOut)
        {
            _inCar = false;
            _isBedOut = true;
            _emergencyBedUI.SetActive(false);
            gameObject.transform.SetPositionAndRotation(_emergencyBedPositionOutsideVehicle.position, _emergencyBedPositionOutsideVehicle.rotation);

            // bed needs to be [in car], [just outside car] ,[with player]
            _photonView.RPC("SetBedParentRPC", RpcTarget.AllBufferedViaServer, PhotonNetwork.NickName, false, true, false);

            FollowPlayerToggle();
            _takeReturnText.text = _returnText;
            _photonView.RPC("SynchBedON", RpcTarget.AllBufferedViaServer);
            transform.Rotate(transform.eulerAngles.x, transform.eulerAngles.y, 0);

        }
        else if (!_inCar && _isBedOut)
        {
            if (_isFollowingPlayer)
                FollowPlayerToggle();

            _inCar = true;
            _isBedOut = false;
            _emergencyBedUI.SetActive(false);

            // bed needs to be [in car], [just outside car] ,[with player]
            _photonView.RPC("SetBedParentRPC", RpcTarget.AllBufferedViaServer, PhotonNetwork.NickName, true, false, false);
            // gameObject.transform.SetPositionAndRotation(_emergencyBedPositionInsideVehicle.position, _emergencyBedPositionInsideVehicle.rotation);
            _photonView.RPC("SetPositionAndRotation_RPC", RpcTarget.AllBufferedViaServer, _emergencyBedPositionInsideVehicle.position, _emergencyBedPositionInsideVehicle.rotation);

            _takeReturnText.text = _takeText;
            _photonView.RPC("SynchBedOFF", RpcTarget.AllBufferedViaServer);


        }
    }


    [PunRPC]
    public void SetPositionAndRotation_RPC(Vector3 pos, Quaternion rot)
    {
        gameObject.transform.SetPositionAndRotation(pos,rot );

    }

    [PunRPC]
    public void SynchBedOFF()
    {
        _SmoothSync.enabled =false ;

    }

    [PunRPC]
    public void SynchBedON()
    {
        _SmoothSync.enabled = true;
      
    }

    private void FollowPlayer()
    {
        if (_player != null)
        {
            if (_isFollowingPlayer)
            {
                _player.transform.position = _playerHoldPos.position;
                _player.transform.LookAt(transform.position);

                // bed needs to be [in car], [just outside car] ,[with player]
                _photonView.RPC("SetBedParentRPC", RpcTarget.AllBufferedViaServer, PhotonNetwork.NickName, false, true, true);
                _followUnfollowText.text = _unfollowText;
            }
            else if (!_isFollowingPlayer)
            {
                //_isFacingTrolley = false;
                // bed needs to be [in car], [just outside car] ,[with player]
                if (!_inCar)
                    _photonView.RPC("SetBedParentRPC", RpcTarget.AllBufferedViaServer, PhotonNetwork.NickName, false, false, false);

                _followUnfollowText.text = _followText;
            }
        }
    }
    public void FollowPlayerToggle()
    {
        if (_player != null && _isBedOut)
        {
            if (_isFollowingPlayer)
            {
                _isFollowingPlayer = false;
                //_photonView.RPC("SetBedFollowPlayer", RpcTarget.AllBufferedViaServer, PhotonNetwork.NickName);
                _followUnfollowText.text = _followText;
                UIManager.Instance.CurrentActionBarParent.SetActive(true);
            }
            else if (!_isFollowingPlayer)
            {
                _isFollowingPlayer = true;
                _player.transform.position = _playerHoldPos.position;
                _player.transform.LookAt(transform.position);
                //_photonView.RPC("SetBedFollowPlayer", RpcTarget.AllBufferedViaServer, PhotonNetwork.NickName);
                _followUnfollowText.text = _unfollowText;
                UIManager.Instance.CurrentActionBarParent.SetActive(false);
            }
            _emergencyBedUI.SetActive(false);
        }
    }

    public void PutRemovePatient()
    {
        if (_patient != null && !IsPatientOnBed)
        {
            _photonView.RPC("PutOnBed", RpcTarget.AllBufferedViaServer,_patient.GetComponent<PhotonView>().ViewID);

        }
        else if (_patient != null && IsPatientOnBed && !_inCar)
        {
            _photonView.RPC("RemoveFromBed", RpcTarget.AllBufferedViaServer, _patient.GetComponent<PhotonView>().ViewID);
            _patient.GetComponentInChildren<MakeItAButton>().gameObject.layer = (int) LayerMasks.Default;
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
            stream.SendNext(_isBedClosed);
            stream.SendNext(IsPatientOnBed);
            stream.SendNext(_isFollowingPlayer);
            stream.SendNext(_inCar);
            stream.SendNext(_isBedOut);
           // stream.SendNext(_patientPosOnBed.position);
        }
        else
        {
            IsBedClosed = (bool)stream.ReceiveNext();
            IsPatientOnBed = (bool)stream.ReceiveNext();
            _isFollowingPlayer = (bool)stream.ReceiveNext();
            _inCar = (bool)stream.ReceiveNext();
            _isBedOut = (bool)stream.ReceiveNext();
          //  _patientPosOnBed.position = (Vector3)stream.ReceiveNext();
        }
    }

    [PunRPC]
    void PutOnBed(int ID)
    {
        PhotonView currentPatientView = GameManager.Instance.GetPatientPhotonViewByIDView(ID);

        currentPatientView.GetComponentInChildren<MakeItAButton>().gameObject.layer = (int)LayerMasks.Default;
        currentPatientView.GetComponent<BoxCollider>().enabled = false;
        currentPatientView.transform.SetPositionAndRotation(_patientPosOnBed.position, _patientPosOnBed.rotation); // parent
        currentPatientView.transform.SetParent(_patientPosOnBed);// parent
        _placeRemovePatientText.text = _removeText;
        IsPatientOnBed = true;
        currentPatientView.GetComponent<Patient>().SmoothMovement.enabled = false;
    }

    [PunRPC]
    void RemoveFromBed(int ID)
    {
        PhotonView currentPatientView = GameManager.Instance.GetPatientPhotonViewByIDView(ID);

        currentPatientView.GetComponentInChildren<MakeItAButton>().gameObject.layer = (int)LayerMasks.Interactable;
        currentPatientView.GetComponent<BoxCollider>().enabled = true;
        currentPatientView.transform.position = _patientPosOffBed.position;// parent
        currentPatientView.transform.SetParent(null);// parent
        _placeRemovePatientText.text = _placeText;
        IsPatientOnBed = false;
        currentPatientView.GetComponent<Patient>().SmoothMovement.enabled = true;
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
    private void SetBedFollowPlayer(string currentPlayer)
    {
        PhotonView currentPlayerView = ActionsManager.Instance.GetPlayerPhotonViewByNickName(currentPlayer);

        if (_isFollowingPlayer)
        {
            gameObject.transform.SetParent(currentPlayerView.transform);
        }
        else if (!_isFollowingPlayer)
        {
            gameObject.transform.SetParent(null);
        }
    }

    [PunRPC]
    private void SetBedParentRPC(string currentPlayer, bool shouldBeInCar, bool shouldBeOut, bool shouldFollowPlayer)
    {
        PhotonView currentPlayerView = ActionsManager.Instance.GetPlayerPhotonViewByNickName(currentPlayer);

        if (shouldBeInCar && !shouldBeOut && !shouldFollowPlayer)
        {
            transform.SetParent(_emergencyBedPositionInsideVehicle);

            if (_patientScript)
            {
                if (IsPatientOnBed)
                {
                    DisablePatientInteractions(false);

                }
                else
                {
                    DisablePatientInteractions(true);

                }
            }
        }
        else if (!shouldBeInCar && shouldBeOut && !shouldFollowPlayer)
        {
            gameObject.transform.SetParent(_emergencyBedPositionOutsideVehicle);

            if (_patientScript)
            {

                DisablePatientInteractions(false);
            }
        }
        else if (!shouldBeInCar && !shouldBeOut && !shouldFollowPlayer)
        {
            gameObject.transform.SetParent(null);

            if (_patientScript)
            {

                DisablePatientInteractions(true);
            }
        }
        else if (!shouldBeInCar && shouldBeOut && shouldFollowPlayer)
        {
            gameObject.transform.SetParent(currentPlayerView.transform);

            if (_patientScript)
            {

                DisablePatientInteractions(false);
            }
        }
    }

    private void DisablePatientInteractions(bool shouldDisable)
    {
        _patientScript.PatientModelCollider.enabled = shouldDisable;

        if (!shouldDisable)
            _patientScript.WorldCanvas.SetActive(false);
    }
}