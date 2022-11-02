using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPunCallbacks
{
    #region Photon
    [Header("Photon")]
    [SerializeField] public PhotonView _photonView; // should be private
    [SerializeField] public PhotonView PhotonView => _photonView;
    #endregion

    #region Data
    [Header("Data")]
    public PlayerData PlayerData;
    public Material LineMaterial;
    public GameObject Vest;
    public MeshFilter VestMeshFilter;
    #endregion

    #region Cameras
    [Header("Cameras")]
    private Camera _currentCamera;
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
    [Header("Controllers")]
    [SerializeField] public CharacterController _characterController;
    
    private CarControllerSimple _currentCarController;
    public CarControllerSimple CurrentCarController { get => _currentCarController; set => _currentCarController = value; }

    private VehicleController _currentVehicleController;
    public VehicleController CurrentVehicleController { get => _currentVehicleController; set => _currentVehicleController = value; }

    [SerializeField] private Vector2 _mouseSensitivity = new Vector2(60f, 40f);
    [SerializeField] private float _turnSpeed = 90f, _runningSpeed = 11f, _flyingSpeed = 16f;
    [SerializeField] private float _flyUpwardsSpeed = 9f;
    private float _stateSpeed;

    private bool _isInVehicle;
    public bool IsInVehicle { get => _isInVehicle; set => _isInVehicle = value; }

    private bool _isDriving;
    public bool IsDriving { get => _isDriving; set => _isDriving = value; }

    private bool _isPassanger;
    public bool IsPassanger { get => _isPassanger; set => _isPassanger = value; }

    private bool _isMiddleSit;
    public bool IsMiddleSit { get => _isMiddleSit; set => _isMiddleSit = value; }

    private bool _isLeftBackSit;
    public bool IsLeftBackSit { get => _isLeftBackSit; set => _isLeftBackSit = value; }

    private bool _isRightBackSit;
    public bool IsRightBackSit { get => _isRightBackSit; set => _isRightBackSit = value; }


    public Vector2 _input;
    public float _walkingSpeed = 6f, actualSpeed;

    [SerializeField] private float _focusedCanvasDistance;
    [SerializeField] private int _worldCanvasLayer;
    #endregion

    #region Physics
    [Header("Physics")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Transform _groundCheckTransform;
    [SerializeField] private float _groundCheckRadius = 0.5f;
    private bool _isGrounded;


    #endregion

    private PlayerController thisScript;


    #region Colliders
    [Header("Colliders")]
    public GameObject CarCollider;
    #endregion

    #region State Machine
    private delegate void State();
    private State _stateAction;
    #endregion

    #region Monobehaviour Callbacks
    private void Awake()
    {
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
    }
    private void Start()
    {
        if (_photonView.IsMine)
        {
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
                    door.layer = (int)LayerMasks.Interactable;
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
                    door.layer = (int)LayerMasks.Default;
                }
            }

            if (!other.gameObject.TryGetComponent(out Patient possiblePatient))
                return;

            PlayerData.CurrentPatientNearby = null;
        }
    }
    #endregion
    
    #region States
    //private void UseFirstPersonIdleState()
    //{
    //    if (_photonView)
    //    {
    //        Debug.Log("Current State: First Person Idle");
    //
    //        GetInputAxis();
    //
    //        if (_isInVehicle)
    //        {
    //            _stateAction = UseDrivingState;
    //        }
    //
    //        if (_input != Vector2.zero)
    //        {
    //            FreeMouse(false);
    //            _stateAction = UseFirstPersonWalkingState;
    //        }
    //
    //        if (Input.GetKeyDown(KeyCode.V))
    //        {
    //            FreeMouse(true);
    //            SetFirstPersonCamera(false);
    //            _stateAction = UseTankIdleState;
    //        }
    //
    //        if (Input.GetKeyDown(KeyCode.G))
    //        {
    //            _stateAction = UseFlyingIdleState;
    //        }
    //
    //        UseFirstPersonRotate();
    //        FreeMouseWithAlt();
    //    }
    //}
    //private void UseFirstPersonWalkingState()
    //{
    //    if (_photonView.IsMine)
    //    {
    //        Debug.Log("Current State: First Person Walking");
    //
    //        GetInputAxis();
    //
    //        if (_isInVehicle)
    //        {
    //            _stateAction = UseDrivingState;
    //        }
    //
    //        if (_input == Vector2.zero)
    //        {
    //            _stateAction = UseFirstPersonIdleState;
    //        }
    //
    //        if (Input.GetKeyDown(KeyCode.V))
    //        {
    //            SetFirstPersonCamera(false);
    //
    //            if (_input == Vector2.zero)
    //            {
    //                FreeMouse(true);
    //                _stateAction = UseTankIdleState;
    //            }
    //            else
    //            {
    //                FreeMouse(true);
    //                _stateAction = UseTankWalkingState;
    //            }
    //        }
    //
    //        if (Input.GetKeyDown(KeyCode.G))
    //        {
    //            _stateAction = UseFlyingMovingState;
    //        }
    //
    //        if (Input.GetMouseButtonDown(1))
    //        {
    //            if (Cursor.visible)
    //            {
    //                FreeMouse(false);
    //            }
    //            else
    //            {
    //                FreeMouse(true);
    //            }
    //        }
    //
    //        UseFirstPersonRotate();
    //        UseFirstPersonMovement();
    //        FreeMouseWithAlt();
    //    }
    //}
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

            //if (Input.GetKeyDown(KeyCode.V))
            //{
            //    FreeMouse(false);
            //    SetFirstPersonCamera(true);
            //    _stateAction = UseFirstPersonIdleState;
            //}

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

            //if (Input.GetKeyDown(KeyCode.V))
            //{
            //    FreeMouse(false);
            //    SetFirstPersonCamera(true);
            //    _stateAction = UseFirstPersonWalkingState;
            //}

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
            Debug.Log("Current State: FlyingIdle");

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
            Debug.Log("Current State: FlyingMoving");

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
            Debug.Log("Current State: Driving");
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
                
                vehicleTransform.rotation = Quaternion.Lerp(vehicleTransform.rotation, new Quaternion(0, vehicleTransform.rotation.y, 0, vehicleTransform.rotation.w), 0.1f);
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
            if (Input.GetKeyDown(KeyCode.Escape) || !UIManager.Instance.EventSystem.currentSelectedGameObject)
            {
                _stateAction = UseTankIdleState;
            }



            RotateBodyWithMouse();

        }
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
        moveDirerction = (Input.GetKey(KeyCode.LeftShift) ? _flyingSpeed * 2 : _flyingSpeed) * _input.y * transform.forward;

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
    //private void UseFirstPersonMovement()
    //{
    //    float actualSpeed = Input.GetKey(KeyCode.LeftShift) ? _runningSpeed : _walkingSpeed;
    //    _characterController.Move(actualSpeed * _input.x * Time.deltaTime * transform.right + actualSpeed * _input.y * Time.deltaTime * transform.forward);
    //}
    //private void UseFirstPersonRotate()
    //{
    //    Vector2 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), -Input.GetAxisRaw("Mouse Y"));
    //
    //    transform.Rotate(_mouseSensitivity.x * mouseInput.x * Time.deltaTime * Vector3.up);
    //    _currentCamera.transform.Rotate(_mouseSensitivity.y * mouseInput.y * Time.deltaTime * Vector3.right);
    //}
    //private void SetFirstPersonCamera(bool value)
    //{
    //    _currentCamera.transform.position = value ? _firstPersonCameraTransform.position : _thirdPersonCameraTransform.position;
    //    _currentCamera.transform.rotation = value ? _firstPersonCameraTransform.rotation : _thirdPersonCameraTransform.rotation;
    //}
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
    #endregion




}
