using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class NavigationManager : MonoBehaviour
{
    private PhotonView _playerPhotonView;
    private PlayerController _playerController;
    private NavMeshAgent _agent;
    private LineRenderer _lineRenderer;
    private bool _reachedDestination;

    [SerializeField] private EvacuationManager _evacuationManager;
    [SerializeField] private List<GameObject> listRoomEnums;
    [SerializeField] private Transform _destination;
    [SerializeField] private GameObject _destinationMarkerPrefab;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 0;
    }

    void Update()
    {
        if (!_playerPhotonView)
        {
            for (int i = 0; i < ActionsManager.Instance.AllPlayersPhotonViews.Count; i++)
            {
                if (ActionsManager.Instance.AllPlayersPhotonViews[i].IsMine )
                {
                    _playerPhotonView = ActionsManager.Instance.AllPlayersPhotonViews[i];
                    _playerController = _playerPhotonView.GetComponent<PlayerController>();
                    break;
                }
            }
        }
        else
        {
            if (_playerController.CurrentCarController)
            {
                transform.position = _playerController.CurrentCarController.transform.position;
            }
            else
            {
                transform.position = _playerPhotonView.transform.position + (_playerPhotonView.transform.forward * 2);
            }
        }

        //if(_lineRenderer.enabled)
        //     _playerController.CurrentCarController.isBusy = true;
        //else
        //    _playerController.CurrentCarController.isBusy = false;


        StopGPSNav();
    }

    public void StartEvacuationGPSNav()
    {
        Debug.Log(UIManager.Instance._dropDown.value); // Gives me the Enum value

        int enumRoom = UIManager.Instance._dropDown.value;

        for (int i = 0; i < listRoomEnums.Count; i++)
        {

            if (i == enumRoom)
            {
                _destinationMarkerPrefab.SetActive(true);
                _destinationMarkerPrefab.transform.position =
                    listRoomEnums[i].transform.position + new Vector3(0f, 4f, 0f);
                _agent.SetDestination(listRoomEnums[i].transform.position);
                _agent.isStopped = true;
                _reachedDestination = false;
            }
        }
    }

    // need fixing - navigation always will go to last incident currently playing
    public void StartIncidentGPSNav()
    {
        int _incidentsCount = 0;
        _incidentsCount = GameManager.Instance.CurrentIncidentsTransforms.Count;

        _destinationMarkerPrefab.transform.position =
         GameManager.Instance.CurrentIncidentsTransforms[_incidentsCount -1].position;
        _agent.SetDestination(GameManager.Instance.CurrentIncidentsTransforms[_incidentsCount -1].position);
        _agent.isStopped = true;
        _reachedDestination = false;
        DrawPath();
    }

    public void StopGPSNav()
    {
        if (Vector3.Distance(_agent.destination, transform.position) <= _agent.stoppingDistance)
        {
            _destinationMarkerPrefab.SetActive(false);
            _reachedDestination = true;

        }
        else if (_agent.hasPath)
        {
            DrawPath();
        }
    }

    private void DrawPath()
    {
        if (!_reachedDestination)
        {
            _playerController.CurrentCarController.IsInPinuy = true;
            _lineRenderer.enabled = true;
            _lineRenderer.positionCount = _agent.path.corners.Length;
            _lineRenderer.SetPosition(0, transform.position);

            if (_agent.path.corners.Length < 2)
            {
                return;
            }

            for (int i = 1; i < _agent.path.corners.Length; i++)
            {
                var tempCornerList = _agent.path.corners;
                Vector3 pointPos = new Vector3(tempCornerList[i].x, tempCornerList[i].y, tempCornerList[i].z);
                _lineRenderer.SetPosition(i, pointPos);
            }
        }
        else
        {
            _lineRenderer.enabled = false;
        }
    }


    public void CarInEvac()
    {

    }
}
