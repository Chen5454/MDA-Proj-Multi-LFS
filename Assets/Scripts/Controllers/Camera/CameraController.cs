using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CameraController : MonoBehaviour
{
    #region Photon
    [Header("Photon")]
    private PhotonView _photonView;
    #endregion

    #region Cameras
    [Header("Player Camera")]
    [SerializeField] private Camera _playerCamera;
   // [SerializeField] private Camera _pikud10Camera;
    public Camera PlayerCamera => _playerCamera;
    //public Camera Pikud10Camera => _pikud10Camera;
    #endregion

    #region Interactions & Indicators
    [Header("Interaction")]
    [SerializeField] private LayerMask _interactableLayer;
    [SerializeField] private LayerMask _selectableLayer;
    [SerializeField] private float _raycastDistance = 10f;

    private bool _sendInteractRaycast;
    public bool SetSendIteractRaycast { set => _sendInteractRaycast = value; }

    [Header("Indications")]
    [SerializeField] private GameObject _indicatorIcon;
    [SerializeField] private AudioSource _indicatorSound;
    [SerializeField] private Outline.Mode _outlineMode;
    private Outline _currentOutline;
    #endregion

    #region MonobehaviourCallbacks
    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (!_photonView.IsMine)
        {
            Destroy(_playerCamera.gameObject);
            Destroy(this);
        }
        else
        {
            _sendInteractRaycast = true;
        }
    }

    private void FixedUpdate()
    {
        if (_photonView.IsMine)
        {
            if (_sendInteractRaycast)
            {
                Interact();
            }
        }

    }
    #endregion

    #region Public Methods
    public void ToggleInteractRaycast(bool sendRaycast)
    {
        _sendInteractRaycast = sendRaycast;
    }

    public RaycastHit Interact()
    {
        Ray ray = _playerCamera.ScreenPointToRay(Input.mousePosition);

        // need to try other layer before ----

        if (Physics.Raycast(ray, out RaycastHit interactableRaycastHit, _raycastDistance, _interactableLayer))
        {
            if (_currentOutline != null && interactableRaycastHit.transform.gameObject != _currentOutline) // true if raycast hit new interactable, destroy old current outline
            {
                _currentOutline.OutlineWidth = 0;
            }

            _currentOutline = interactableRaycastHit.transform.GetComponent<Outline>();

            if (!_currentOutline) // if current is null => add outline to interactrable
            {
                _currentOutline = interactableRaycastHit.transform.gameObject.AddComponent<Outline>();
            }

            _currentOutline.OutlineWidth = 15;
            _currentOutline.OutlineMode = _outlineMode;
            //_indicatorIcon.SetActive(true);

            Debug.DrawLine(ray.origin, interactableRaycastHit.point, Color.cyan, 1f);


            if (Input.GetMouseButtonDown(0))
            {
                _indicatorSound.Play();
                interactableRaycastHit.transform.GetComponent<MakeItAButton>().EventToCall.Invoke();

                Debug.Log($"Interacted with {interactableRaycastHit.transform.name}");
            }
        }
        else
        {
            if (_currentOutline != null)
            {
                _currentOutline.OutlineWidth = 0;
            }
            //_indicatorIcon.SetActive(false);
        }

        return interactableRaycastHit;
    }

    #endregion
}
