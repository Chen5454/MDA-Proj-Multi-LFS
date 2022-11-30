using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class NavigationManager : MonoBehaviour
{
    [SerializeField] private PhotonView _photonView;

    private PhotonView _playerPhotonView;
    private PlayerController _playerController;
    private PlayerData _playerData;
    private NavMeshAgent _agent;
    private LineRenderer _lineRenderer;
    private bool _reachedDestination;

    [SerializeField] private List<GameObject> listRoomEnums;
    [SerializeField] private List<Transform> _destinationHospitals;
    [SerializeField] private GameObject _destinationMarkerPrefab;
    [SerializeField] private float stoppingDistance;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 0;
        stoppingDistance = 14f;
        _agent.stoppingDistance = stoppingDistance;
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
                    _playerData = _playerPhotonView.GetComponent<PlayerData>();
                    break;
                }
            }
        }
        else
        {
            if (_playerController.CurrentVehicleController)
            {
                transform.position = _playerController.CurrentVehicleController.transform.position;
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

    public void StartEvacuationGPSNavButton()
    {
        if(_playerController.CurrentVehicleController.IsPatientIn)
        StartEvacuationGPSNav();
    }


    public void StartEvacuationGPSNav() // this
    {
        //Debug.Log(UIManager.Instance._dropDown.value); // Gives me the Enum value

        //int enumRoom = UIManager.Instance._dropDown.value;

        //for (int i = 0; i < listRoomEnums.Count; i++)
        //{
        //    if (i == enumRoom)
        //    {
        //        _destinationMarkerPrefab.SetActive(true);
        //        _destinationMarkerPrefab.transform.position = listRoomEnums[i].transform.position + new Vector3(0f, 4f, 0f);
        //        _agent.SetDestination(listRoomEnums[i].transform.position);
        //        _agent.isStopped = true;
        //        _reachedDestination = false;

        //        _photonView.RPC("ShowEvacNavRPC", RpcTarget.Others, _playerData.CrewIndex, i);
        //    }
        //}

    
        Transform closestTarget = null;
        float closestTargetDistance = float.MaxValue;
        NavMeshPath Path = new NavMeshPath();
        for (int i = 0; i < _destinationHospitals.Count; i++)
        {
            if (_destinationHospitals[i] == null)
            {
                continue;
            }

            if (NavMesh.CalculatePath(transform.position, _destinationHospitals[i].position, _agent.areaMask, Path))
            {
                float distance = Vector3.Distance(transform.position, Path.corners[0]);
                for (int j = 1; j < Path.corners.Length; j++)
                {
                    distance += Vector3.Distance(Path.corners[j - 1], Path.corners[j]);
                }

                if (distance < closestTargetDistance)
                {
                    closestTargetDistance = distance;
                    closestTarget = _destinationHospitals[i];
                }
            }
        }

        if (closestTarget != null)
        {
         _destinationMarkerPrefab.SetActive(true);
         _destinationMarkerPrefab.transform.position = closestTarget.transform.position + new Vector3(0f, 4f, 0f);
         _agent.isStopped = true;
         _reachedDestination = false;
         _agent.SetDestination(closestTarget.position); 
         _photonView.RPC("ShowEvacNavRPC", RpcTarget.Others, _playerData.CrewIndex, closestTarget.position);

        }

    }

    // need fixing - navigation always will go to last incident currently playing
    public void StartIncidentGPSNav()
    {
        if (_playerController.IsDriving)
        {
            int _incidentsCount = 0;
            _incidentsCount = GameManager.Instance.CurrentIncidentsTransforms.Count;

            try
            {
                _destinationMarkerPrefab.transform.position =
                GameManager.Instance.CurrentIncidentsTransforms[_incidentsCount - 1].position;
                _agent.SetDestination(GameManager.Instance.CurrentIncidentsTransforms[_incidentsCount - 1].position);
                _agent.isStopped = true;
                _reachedDestination = false;

                _photonView.RPC("ShowIncidentNavRPC", RpcTarget.Others, _playerData.CrewIndex, _incidentsCount - 1);
            }
            catch (System.ArgumentOutOfRangeException)
            {

                return;
            }
            //_destinationMarkerPrefab.transform.position =
            //    GameManager.Instance.CurrentIncidentsTransforms[_incidentsCount - 1].position;
            //_agent.SetDestination(GameManager.Instance.CurrentIncidentsTransforms[_incidentsCount - 1].position);
            //_agent.isStopped = true;
            //_reachedDestination = false;
            //
            //_photonView.RPC("ShowIncidentNavRPC", RpcTarget.Others, _playerData.CrewIndex, _incidentsCount - 1);
        }
        else
        {
            Debug.Log("Only the driver can set to navigation.");
        }
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
            if (_playerController && _playerController.CurrentVehicleController)
                _playerController.CurrentVehicleController.IsBusy  = true;

            _lineRenderer.enabled = true;
            _lineRenderer.positionCount = _agent.path.corners.Length;
            _lineRenderer.SetPosition(0, transform.position);

            if (_agent.path.corners.Length < 2)
                return;

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


    [PunRPC]
    private void ShowIncidentNavRPC(int crewIndex, int incidentCount)
    {
        if (_playerData.CrewIndex == crewIndex)
        {
            _destinationMarkerPrefab.transform.position = GameManager.Instance.CurrentIncidentsTransforms[incidentCount].position;
            _agent.SetDestination(GameManager.Instance.CurrentIncidentsTransforms[incidentCount].position);
            _agent.isStopped = true;
            _reachedDestination = false;
        }
    }

    [PunRPC]
    private void ShowEvacNavRPC(int crewIndex,Vector3 target)
    {
        if (_playerData.CrewIndex == crewIndex)
        {
            _destinationMarkerPrefab.SetActive(true);
            _destinationMarkerPrefab.transform.position = target + new Vector3(0f, 4f, 0f);
            _agent.SetDestination(target);
            _agent.isStopped = true;
            _reachedDestination = false;
        }
    }
}
