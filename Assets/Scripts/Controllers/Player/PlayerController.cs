using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine.Assertions.Must;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    #region Photon

    [Header("Photon")] [SerializeField] public PhotonView _photonView; // should be private
    [SerializeField] public PhotonView PhotonView => _photonView;

    #endregion

    #region Data

    [Header("Data")] public PlayerData PlayerData;
    public Material LineMaterial;
    public GameObject Vest;
    public MeshFilter VestMeshFilter;

    #endregion

    #region Cameras

    [Header("Cameras")] private Camera _currentCamera;

    [SerializeField] private GameObject _MiniMaCamera;

    //[SerializeField] private Transform _firstPersonCameraTransform;
    [SerializeField] private Transform _thirdPersonCameraTransform;
    [SerializeField] private Camera _playerCamera, _vehicleCamera, _pikud10Camera;
    public Camera CurrentCamera => _currentCamera;
    public Camera Pikud10Camera => _pikud10Camera;

    #endregion

    #region Audio

    [SerializeField] private AudioListener _audioListener;

    #endregion

    #region Animations

    // [Header("Animation")]
    //[SerializeField] private Animator _playerAnimator;

    private PlayerAnimationManager _anim;

    #endregion

    #region Controllers Behaviours

    [Header("Controllers")] [SerializeField]
    public CharacterController _characterController;

    private CarControllerSimple _currentCarController;

    public CarControllerSimple CurrentCarController
    {
        get => _currentCarController;
        set => _currentCarController = value;
    }

    public VehicleController _currentVehicleController;

    public VehicleController CurrentVehicleController
    {
        get => _currentVehicleController;
        set => _currentVehicleController = value;
    }

    [SerializeField] private Vector2 _mouseSensitivity = new Vector2(60f, 40f);
    [SerializeField] private float _turnSpeed = 90f, _runningSpeed = 11f, _flyingSpeed = 16f;
    [SerializeField] private float _flyUpwardsSpeed = 9f;
    private float _stateSpeed;

    public bool _isInVehicle;

    public bool IsInVehicle
    {
        get => _isInVehicle;
        set => _isInVehicle = value;
    }

    private bool _isDriving;

    public bool IsDriving
    {
        get => _isDriving;
        set => _isDriving = value;
    }

    private bool _isPassanger;

    public bool IsPassanger
    {
        get => _isPassanger;
        set => _isPassanger = value;
    }

    private bool _isMiddleSit;

    public bool IsMiddleSit
    {
        get => _isMiddleSit;
        set => _isMiddleSit = value;
    }

    private bool _isLeftBackSit;

    public bool IsLeftBackSit
    {
        get => _isLeftBackSit;
        set => _isLeftBackSit = value;
    }

    private bool _isRightBackSit;

    public bool IsRightBackSit
    {
        get => _isRightBackSit;
        set => _isRightBackSit = value;
    }


    public Vector2 _input;
    public float _walkingSpeed = 6f, actualSpeed;

    [SerializeField] private float _focusedCanvasDistance;
    [SerializeField] private int _worldCanvasLayer;

    #endregion

    #region Physics

    [Header("Physics")] [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Transform _groundCheckTransform;
    [SerializeField] private float _groundCheckRadius = 0.5f;
    private bool _isGrounded;


    #endregion

    private PlayerController thisScript;

    public GameObject _tastingForPremisionWorks;
    public GameObject _tastingForPremisionError;
    [SerializeField] private NameTagDisplay playerNameTag;

    #region Colliders

    [Header("Colliders")] public GameObject CarCollider;

    #endregion

    #region State Machine

    private delegate void State();

    private State _stateAction;

    #endregion

    #region Monobehaviour Callbacks

    private void Awake()
    {
        UIManager.Instance.ResetCrewRoom.onClick.AddListener(delegate { CrewLeaderResetIncident(); });
        UIManager.Instance.TeleportBtn.onClick.AddListener(delegate { TelepotrtToCenterBtn(); });

        _anim = GetComponent<PlayerAnimationManager>();


        PlayerData = gameObject.AddComponent<PlayerData>();
        _currentCamera = _playerCamera;
        _playerCamera.tag = "MainCamera";
        //  PlayerData.IsInstructor = false;

        if (VivoxManager.Instance.Lobby.ConnectAsInstructor.isOn && photonView.IsMine)
        {
            PlayerData.IsInstructor = true;
            PhotonNetwork.SetMasterClient(_photonView.Owner);
        }

        thisScript = GetComponent<PlayerController>();
        playerNameTag = GetComponentInChildren<NameTagDisplay>();
    }

    private void Start()
    {


        if (_photonView.IsMine)
        {
            PhotonView.Get(this).RPC("AddPlayerToList", RpcTarget.All, PhotonView.Get(this).ViewID);
            FreeMouse(true);
            _stateAction = UseTankIdleState;
            _MiniMaCamera.SetActive(true);
            _characterController.enabled = true;


        }
        else
        {
            _MiniMaCamera.SetActive(false);
            _characterController.enabled = false;
            Destroy(_audioListener);
            thisScript.enabled = false;
            // Destroy(this);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.LogError(ActionsManager.Instance.AllPlayersPhotonViews.Count);
        }

        if (_photonView.IsMine)
        {
            _stateAction.Invoke();
        }
    }

    #endregion

    #region Collisions & Triggers

    private void OnTriggerEnter(Collider other)
    {
        if (_photonView.IsMine)
        {
            if (other.gameObject.CompareTag("Car") && !IsInVehicle)
            {
                VehicleController vehicleController = other.gameObject.GetComponent<VehicleController>();

                foreach (GameObject door in vehicleController.AllDoors)
                {
                    door.layer = (int) LayerMasks.Interactable;
                }
            }

            if (!other.gameObject.TryGetComponent(out Patient possiblePatient))
                return;

            PlayerData.CurrentPatientNearby = possiblePatient;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_photonView.IsMine)
        {
            if (other.gameObject.CompareTag("Car"))
            {
                VehicleController vehicleController = other.gameObject.GetComponent<VehicleController>();

                foreach (GameObject door in vehicleController.AllDoors)
                {
                    door.layer = (int) LayerMasks.Default;
                }
            }

            if (!other.gameObject.TryGetComponent(out Patient possiblePatient))
                return;

            PlayerData.CurrentPatientNearby = null;
        }
    }

    #endregion

    #region States

    private void UseTankIdleState()
    {
        if (_photonView.IsMine)
        {
            //Debug.Log("Current State: Idle");
            _anim.IdleStateAnimation();

            GetInputAxis();

            if (_isInVehicle)
            {
                _stateAction = UseDrivingState;
            }

            if (_input != Vector2.zero)
            {
                FreeMouse(true);
                _stateAction = UseTankWalkingState;
            }

            if (UIManager.Instance.EventSystem.currentSelectedGameObject)
            {
                if (UIManager.Instance.EventSystem.currentSelectedGameObject.layer == _worldCanvasLayer)
                {
                    _stateAction = UseUIState;
                }
            }


            if (Input.GetKeyDown(KeyCode.G))
            {
                _stateAction = UseFlyingIdleState;
            }

            RotateBodyWithMouse();
        }
    }

    private void UseTankWalkingState()
    {
        if (_photonView.IsMine)
        {
            // Debug.Log("Current State: Walking");

            GetInputAxis();

            if (_isInVehicle)
            {
                _stateAction = UseDrivingState;
            }

            if (_input == Vector2.zero)
            {
                FreeMouse(true);
                _stateAction = UseTankIdleState;
            }

            if (UIManager.Instance.EventSystem.currentSelectedGameObject)
            {
                if (UIManager.Instance.EventSystem.currentSelectedGameObject.layer == _worldCanvasLayer)
                {
                    _stateAction = UseUIState;
                }
            }


            if (Input.GetKeyDown(KeyCode.G))
            {
                _stateAction = UseFlyingMovingState;
            }

            RotateBodyWithMouse();
            UseTankRotate();
            UseTankMovement();
        }
    }

    private void UseFlyingIdleState()
    {
        if (_photonView.IsMine)
        {
            //Debug.Log("Current State: FlyingIdle");

            GetInputAxis();

            if (_isInVehicle)
            {
                _stateAction = UseDrivingState;
            }

            if (_input != Vector2.zero)
            {
                FreeMouse(false);
                _stateAction = UseFlyingMovingState;
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                _stateAction = UseTankIdleState;
            }

            if (Input.GetKey(KeyCode.E))
            {
                transform.position += new Vector3(0, _flyUpwardsSpeed * Time.deltaTime, 0);
            }

            if (Input.GetKey(KeyCode.Q))
            {
                transform.position -= new Vector3(0, _flyUpwardsSpeed * Time.deltaTime, 0);
            }

            RotateBodyWithMouse();
        }
    }

    private void UseFlyingMovingState()
    {
        if (_photonView.IsMine)
        {
            //  Debug.Log("Current State: FlyingMoving");

            GetInputAxis();

            if (_input == Vector2.zero)
            {
                _stateAction = UseFlyingIdleState;
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                _stateAction = UseTankIdleState;
            }

            if (Input.GetKey(KeyCode.E))
            {
                transform.position += new Vector3(0, _flyUpwardsSpeed * Time.deltaTime, 0);
            }

            if (Input.GetKey(KeyCode.Q))
            {
                transform.position -= new Vector3(0, _flyUpwardsSpeed * Time.deltaTime, 0);
            }

            RotateBodyWithMouse();
            UseTankRotate();
            UseFlyingMovement();
        }
    }

    private void UseDrivingState()
    {
        if (_photonView.IsMine)
        {
            //   Debug.Log("Current State: Driving");
            _anim.IsSittedAnimation();

            if (!_isInVehicle)
            {
                _characterController.enabled = true;
                _vehicleCamera.enabled = false;
                _vehicleCamera.tag = "Untagged";
                _playerCamera.gameObject.SetActive(true);
                _playerCamera.enabled = true;
                _playerCamera.tag = "MainCamera";
                _currentCamera = _playerCamera;
                _vehicleCamera.GetComponent<VehicleCameraFollow>().Target = null;
                //_vehicleCamera = null;
                _stateAction = UseTankIdleState;
                return;
            }


            if (Input.GetKey(KeyCode.F))
            {
                Transform vehicleTransform = transform.parent.parent.parent;

                vehicleTransform.rotation = Quaternion.Lerp(vehicleTransform.rotation,
                    new Quaternion(0, vehicleTransform.rotation.y, 0, vehicleTransform.rotation.w), 0.1f);
            }

            if (_playerCamera.enabled)
            {
                _characterController.enabled = false;
                _playerCamera.enabled = false;
                _playerCamera.tag = "Untagged";
                _playerCamera.gameObject.SetActive(false);
                //_vehicleCamera = _currentVehicleController.VehicleCamera;
                _vehicleCamera.enabled = true;
                _vehicleCamera.tag = "MainCamera";
                _currentCamera = _vehicleCamera;
                VehicleCameraFollow cameraFollow = _vehicleCamera.GetComponent<VehicleCameraFollow>();
                cameraFollow.Target = _currentVehicleController.CameraFollowTransform;
                _isGrounded = _characterController.isGrounded;
            }
            //_currentCarController.CheckIfDriveable();
            //_currentCarController.GetInput();
            //_currentCarController.CheckIsMovingBackwards();
        }
    }

    private void UseTreatingState()
    {
        if (_photonView.IsMine)
        {
            //RotateCameraWithMouse();
        }
    }

    private void UseUIState()
    {
        if (_photonView.IsMine)
        {
            RotateBodyWithMouse();
        }
    }

    public void UnstuckFromUI()
    {
        _stateAction = UseTankIdleState;
    }

    public void TelepotrtToCenterBtn()
    {
        this.gameObject.transform.position = GameManager.Instance.PlayerTPPos.transform.position;
    }

    public void CrewLeaderResetIncident()
    {
        if (PlayerData.IsCrewLeader)
        {

            List<PlayerController> crewMembers = FindObjectsOfType<PlayerController>()
                .Where(player => player.PlayerData.CrewIndex == PlayerData.CrewIndex && player != this).ToList();
            foreach (PlayerController player in crewMembers)
            {
                // Reset the data for each player in the crew
                player.ResetPlayerData();
            }

            _photonView.RPC("ExitPlayersFromCar_RPC", RpcTarget.All, PlayerData.CrewIndex);


            if (_photonView.IsMine)
                _photonView.RPC("CrewLeaderResetIncident_RPC", RpcTarget.AllViaServer, PlayerData.CrewIndex);
        }

        if (_photonView.IsMine)
        {
            _photonView.RPC("FindPlayerOwner", GetPatientOwner(), GetPatientPhotonView());
            _photonView.RPC("FindPlayerOwner", GetCarOwner(), GetCarPhotonView());

        }

        UIManager.Instance.ResetCrewRoom.gameObject.SetActive(false);

    }


    public int GetBedPhotonView()
    {
        for (int i = 0; i < GameManager.Instance.AllBeds.Count; i++)
        {
            EmergencyBedController desiredBed = GameManager.Instance.AllBeds[i].GetComponent<EmergencyBedController>();

            if (PlayerData.CrewIndex == desiredBed.ParentVehicle._ownedCrewNumber)
            {

                int bedIndex = desiredBed.GetComponent<PhotonView>().ViewID;
                return bedIndex;
            }
        }

        return 0;
    }

    public int GetCarPhotonView()
    {
        for (int i = 0; i < GameManager.Instance.AmbulanceCarList.Count; i++)
        {
            VehicleController desiredCar = GameManager.Instance.AmbulanceCarList[i].GetComponent<VehicleController>();

            if (PlayerData.CrewIndex == desiredCar._ownedCrewNumber)
            {

                int carIndex = desiredCar.GetComponent<PhotonView>().ViewID;
                Debug.Log(carIndex);
                return carIndex;
            }
        }


        for (int i = 0; i < GameManager.Instance.NatanCarList.Count; i++)
        {
            VehicleController desiredCar = GameManager.Instance.NatanCarList[i].GetComponent<VehicleController>();

            if (PlayerData.CrewIndex == desiredCar._ownedCrewNumber)
            {
                int carIndex = desiredCar.GetComponent<PhotonView>().ViewID;
                Debug.Log(carIndex);
                return carIndex;
            }
        }

        Debug.Log("Return nothing");
        return 0;
    }

    public int GetPatientPhotonView()
    {
        for (int i = 0; i < GameManager.Instance.AllPatients.Count; i++)
        {
            Patient desiredPatient = GameManager.Instance.AllPatients[i].GetComponent<Patient>();

            if (PlayerData.CrewIndex == desiredPatient._ownedCrewNumber)
            {
                int PatientIndex = desiredPatient.GetComponent<PhotonView>().ViewID;
                Debug.Log(PatientIndex);
                return PatientIndex;
            }
        }

        Debug.Log("Return nothing");
        return 0;
    }

    public Player GetCarOwner()
    {
        for (int i = 0; i < GameManager.Instance.AmbulanceCarList.Count; i++)
        {
            VehicleController desiredCar = GameManager.Instance.AmbulanceCarList[i].GetComponent<VehicleController>();

            if (PlayerData.CrewIndex == desiredCar._ownedCrewNumber)
            {
                Player carIndex = desiredCar.GetComponent<PhotonView>().Controller;
                return carIndex;
            }
        }


        for (int i = 0; i < GameManager.Instance.NatanCarList.Count; i++)
        {
            VehicleController desiredCar = GameManager.Instance.NatanCarList[i].GetComponent<VehicleController>();

            if (PlayerData.CrewIndex == desiredCar._ownedCrewNumber)
            {
                Player carIndex = desiredCar.GetComponent<PhotonView>().Controller;
                return carIndex;
            }
        }

        return null;
    }

    public Player GetPatientOwner()
    {
        for (int i = 0; i < GameManager.Instance.AllPatients.Count; i++)
        {
            Patient desiredPatient = GameManager.Instance.AllPatients[i].GetComponent<Patient>();

            if (PlayerData.CrewIndex == desiredPatient._ownedCrewNumber)
            {
                Player PaitentIndex = desiredPatient.GetComponent<PhotonView>().Controller;
                return PaitentIndex;
            }
        }

        return null;
    }

    public Player GetBedOwner()
    {
        for (int i = 0; i < GameManager.Instance.AllBeds.Count; i++)
        {
            EmergencyBedController desiredBed = GameManager.Instance.AllBeds[i].GetComponent<EmergencyBedController>();

            if (PlayerData.CrewIndex == desiredBed.ParentVehicle._ownedCrewNumber)
            {
                Player BedIndex = desiredBed.GetComponent<PhotonView>().Owner;
                return BedIndex;
            }
        }

        return null;
    }


    public Player GetRandomCrewTeam()
    {

        for (int i = 0; i < ActionsManager.Instance.AllPlayersPhotonViews.Count; i++)
        {
            PlayerData desiredPlayer = ActionsManager.Instance.AllPlayersPhotonViews[i].GetComponent<PlayerData>();

            if (!desiredPlayer.photonView.IsMine)
            {
                if (desiredPlayer.CrewIndex == PlayerData.CrewIndex)
                {
                    Player PlayerIndex = desiredPlayer.GetComponent<PhotonView>().Owner;
                    Debug.Log("Switching Owning to diffrent team player");
                    return PlayerIndex;
                }
                else
                {
                    return null;
                }
            }

        }

        return null;
    }

    #endregion

    #region Private Methods

    private void GetInputAxis()
    {
        _input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    private void UseTankMovement()
    {
        Vector3 moveDirerction;
        actualSpeed = Input.GetKey(KeyCode.LeftShift) ? _runningSpeed : _walkingSpeed;
        moveDirerction = actualSpeed * _input.y * transform.forward;
        _anim.MoveStateAnimation();
        // moves the character in diagonal direction
        _characterController.Move(moveDirerction * Time.deltaTime - Vector3.up * 0.1f);
    }

    private void UseFlyingMovement()
    {
        float yPosition = transform.position.y;
        Vector3 moveDirerction;
        moveDirerction = (Input.GetKey(KeyCode.LeftShift) ? _flyingSpeed * 2 : _flyingSpeed) * _input.y *
                         transform.forward;

        // moves the character in diagonal direction
        _characterController.Move(moveDirerction * Time.deltaTime - Vector3.up * 0.1f);
        transform.position = new Vector3(transform.position.x, yPosition, transform.position.z);
    }

    private void UseTankRotate()
    {
        _anim.RotateAnimation();
        transform.Rotate(0, _input.x * _turnSpeed * Time.deltaTime, 0);

        //if (Input.GetMouseButton(1))
        //    return;
    }

    private void FreeMouse(bool value)
    {
        Cursor.visible = value;
        Cursor.lockState = value ? CursorLockMode.Confined : CursorLockMode.Locked;
    }

    private void RotateBodyWithMouse()
    {
        if (Input.GetMouseButton(1))
        {
            Vector2 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), -Input.GetAxisRaw("Mouse Y"));

            transform.Rotate(Vector3.up * mouseInput.x * _mouseSensitivity.x * Time.deltaTime);
            _currentCamera.transform.Rotate(_mouseSensitivity.y * mouseInput.y * Time.deltaTime * Vector3.right);
        }
    }

    private void FreeMouseWithAlt()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            FreeMouse(!Cursor.visible);
        }
    }

    #endregion

    #region Public Methods

    public void ChangeToTreatingState(bool isTreating)
    {
        if (isTreating)
        {
            _stateAction = UseTreatingState;
        }
        else
        {
            _stateAction = UseTankIdleState;
            _currentCamera.transform.localRotation = new Quaternion(0.130525976f, 0, 0, 0.991444886f);
        }
    }

    public void ChangeToUseUIState(bool isFocusingOnUI)
    {
        if (isFocusingOnUI)
        {
            _stateAction = UseUIState;
        }
        else
        {
            _stateAction = UseTankIdleState;
        }
    }

    #endregion

    #region PunRPC

    [PunRPC]
    private void ChangeCharControllerStateRPC()
    {
        if (_characterController.enabled)
        {
            _characterController.enabled = false;
        }
        else
        {
            _characterController.enabled = true;
        }
    }

    [PunRPC]
    private void SetUserVehicleController()
    {
        PlayerData.LastVehicleController = _currentVehicleController;
    }



    public void SetUserVestRPC(int roleIndex)
    {
        if (!PlayerData.IsDataInitialized || PlayerData.UserRole != (Roles) roleIndex)
        {
            VestMeshFilter.mesh = ActionsManager.Instance.Vests[roleIndex];
            PlayerData.UserRole = (Roles) roleIndex;

            if (!Vest.activeInHierarchy)
                Vest.SetActive(true);

            PlayerData.IsDataInitialized = true;
        }
    }



    [PunRPC]
    public void CrewLeaderResetIncident_RPC(int crewIndex)
    {
        List<PlayerController> crewMembers = FindObjectsOfType<PlayerController>()
            .Where(player => player.PlayerData.CrewIndex == crewIndex).ToList();
        foreach (PlayerController player in crewMembers)
        {
            player.ResetPlayerData();
        }
    }

    [PunRPC]
    public void ExitPlayersFromCar_RPC(int crewIndex)
    {
        ExitPlayersFromCar(crewIndex);
    }

    public void ExitPlayersFromCar(int crewIndex)
    {
        // Get a list of all the players in the crew
        List<PlayerController> playersToExit = FindObjectsOfType<PlayerController>()
            .Where(player => player.PlayerData.CrewIndex == PlayerData.CrewIndex && player.IsInVehicle).ToList();

        // Iterate through the list of players
        foreach (PlayerController player in playersToExit)
        {
            // Check if the player is in a vehicle
            if (player.IsInVehicle && player.CurrentVehicleController != null)
            {
                // Get the VehicleInteraction component of the current vehicle
                VehicleInteraction vehicleInteraction =
                    player.CurrentVehicleController.GetComponent<VehicleInteraction>();

                // Call the ExitVehicle method of the VehicleInteraction component
                if (vehicleInteraction != null)
                {
                    // Exit the vehicle
                    vehicleInteraction.ExitVehicle();
                }
            }
        }
    }

    public void ResetPlayerData()
    {
        // Reset the player's data here
        playerNameTag.crown.SetActive(false);
        playerNameTag.text.color = Color.white;
        PlayerData.CrewColor = Color.white;
        PlayerData.CrewIndex = 0;
        PlayerData.UserRole = Roles.None;
        PlayerData.IsCrewLeader = false;
        Vest.SetActive(false);
        PlayerData.IsDataInitialized = false;
        if (UIManager.Instance.PatientInfoParent.activeInHierarchy)
        {
            UIManager.Instance.PatientInfoParent.SetActive(false);
        }
    }

    [PunRPC]
    public void FindPlayerOwner(int objectIndex)
    {

        Debug.Log("objectIndex: " + objectIndex);

        // Check if the objectIndex is valid
        if (objectIndex <= 0)
        {
            Debug.LogError("Invalid objectIndex: " + objectIndex);
            return;
        }

        // Try to get the PhotonView component using the objectIndex
        PhotonView goPhotonView = PhotonNetwork.GetPhotonView(objectIndex);
        if (goPhotonView == null)
        {
            Debug.LogError("PhotonView is null for objectIndex: " + objectIndex);
            return;
        }

        // Get the game object associated with the PhotonView component
        GameObject go = goPhotonView.gameObject;
        Debug.Log("go: " + go);

        // Get the owner of the PhotonView component
        Photon.Realtime.Player goPhotonview = goPhotonView.Owner;

        // Iterate through the list of players
        for (int i = 0; i < ActionsManager.Instance.AllPlayersPhotonViews.Count; i++)
        {
            // Get the PhotonView component for the player
            PhotonView desiredPlayer = ActionsManager.Instance.AllPlayersPhotonViews[i].GetComponent<PhotonView>();

            // Check if the owner of the PhotonView component matches the owner of the goPhotonView component
            if (desiredPlayer.Owner == goPhotonview)
            {
                // Destroy the game object if the owners match
                PhotonNetwork.Destroy(go);
                break;
            }
        }


    }

    #endregion

    #region Gizmos

    private void OnDrawGizmosSelected()
    {
        if (_photonView.IsMine)
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (_isGrounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(_groundCheckTransform.position, _groundCheckRadius);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_isInVehicle);
            stream.SendNext(_isDriving);
            stream.SendNext(_isPassanger);
            stream.SendNext(_isMiddleSit);
            stream.SendNext(_isLeftBackSit);
            stream.SendNext(_isRightBackSit);
            // stream.SendNext(Vest.activeSelf);
            //  stream.SendNext(PlayerData.UserRole); // no need


        }
        else
        {
            _isInVehicle = (bool) stream.ReceiveNext();
            _isDriving = (bool) stream.ReceiveNext();
            _isPassanger = (bool) stream.ReceiveNext();
            _isMiddleSit = (bool) stream.ReceiveNext();
            _isLeftBackSit = (bool) stream.ReceiveNext();
            _isRightBackSit = (bool) stream.ReceiveNext();
            // Vest.SetActive((bool)stream.ReceiveNext());
            //  PlayerData.UserRole  = (Roles)stream.ReceiveNext(); // no need




        }

    }

    #endregion

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        // Debug.LogError(newPlayer + "OnPlayerEnteredRoom");

        _photonView.RPC("UpdatePlayerData", newPlayer, PlayerData.CrewIndex, (int) PlayerData.UserRole,
            PlayerData.IsCrewLeader, PlayerData.IsDataInitialized, PlayerData.CrewColor.r, PlayerData.CrewColor.g,
            PlayerData.CrewColor.b);

        _photonView.RPC("AddPlayerToList", newPlayer, PhotonView.Get(this).ViewID);

    }

    [PunRPC]
    public void UpdatePlayerData(int crewIndex, int roleIndex, bool isCrewLeader, bool isDataInitialized, float r,
        float g, float b)
    {
        PlayerData.CrewIndex = crewIndex;
        PlayerData.UserRole = (Roles) roleIndex;
        PlayerData.IsCrewLeader = isCrewLeader;
        PlayerData.IsDataInitialized = isDataInitialized;
        //Set his Vest to the right Role
        VestMeshFilter.mesh = ActionsManager.Instance.Vests[roleIndex];
        if (!Vest.activeInHierarchy)
            Vest.SetActive(true);

        PlayerData.CrewColor = new Color(r, g, b);
        playerNameTag.text.color = PlayerData.CrewColor;
    }


    [PunRPC]
    void AddPlayerToList(int playerViewId)
    {
        PhotonView playerView = PhotonView.Find(playerViewId);
        if (!ActionsManager.Instance.AllPlayersPhotonViews.Contains(playerView))
            ActionsManager.Instance.AllPlayersPhotonViews.Add(playerView);
    }

}







