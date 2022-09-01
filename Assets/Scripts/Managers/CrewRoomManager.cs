using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.ExceptionServices;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;

public class CrewRoomManager : MonoBehaviour,IPunObservable
{
    //public GameObject RoomDoorBlocker;
    //public TextMeshProUGUI CrewMemberNameText1, CrewMemberNameText2, CrewMemberNameText3, CrewMemberNameText4;

    private PhotonView _photonView;

    public int _crewRoomIndex;
    public static int _crewRoomIndexStatic;
    public int _playersMaxCount = 4;

    public Canvas RoomCrewMenuUI;
    public List<TextMeshProUGUI> listOfUiNamesTMP;
    public List<TMP_Dropdown> CrewMemberRoleDropDownList;
    public TMP_Dropdown CrewLeaderDropDown;

    public List<PhotonView> _playersInRoomList;
    //public int _crewRoomIndex;

    private Color crewColor;
    private Vector3 _vestPos = new Vector3(0f, 0.295f, -0.015f);

    [SerializeField] private GameObject _patientMale, _patientFemale;
    [SerializeField] private string _waitMemberText;
    [SerializeField] private bool isUsed;


    private OwnershipTransfer _transfer;
    //[SerializeField] private GameObject _crewRoomDoor;

    private void Awake()
    {
        _transfer = GetComponent<OwnershipTransfer>();
        _crewRoomIndexStatic = 0;
        _photonView = GetComponent<PhotonView>();
        PopulateDropdownRoles();
        RoomCrewMenuUI.gameObject.SetActive(false);
    }

    private void Start()
    {
        _crewRoomIndexStatic++;
        _crewRoomIndex = _crewRoomIndexStatic;
    }

    private void Update()
    {
        if (_photonView.IsMine)
        {
            InteractUICrew();
        }
        else
        {
            RoomCrewMenuUI.GetComponent<CanvasGroup>().interactable = false;

        }
    }

    private bool CheckIfAlreadyInList(GameObject player)
    {
        bool playerFound = false;

        for (int i = 0; i < _playersInRoomList.Count; i++)
        {
            if (_playersInRoomList[i].ViewID == player.GetPhotonView().ViewID)
            {
                playerFound = true;
            }
        }

        return playerFound;
    }

    private void InteractUICrew()
    {

       // PhotonView currentPlayerView = ActionsManager.Instance.GetPlayerPhotonViewByNickName(currentPlayer);


        if (isUsed)
        {
            RoomCrewMenuUI.GetComponent<CanvasGroup>().interactable = true;

        }
      

    }


    private void SetCrewUITexts()
    {
        // Crew Roles UI
        for (int i = 0; i < _playersInRoomList.Count; i++)
        {
            listOfUiNamesTMP[i].text = _playersInRoomList[i].Owner.NickName;
        }

        // Crew Leader UI
        List<string> nicknamesList = new List<string>();
        foreach (var player in _playersInRoomList)
        {
            nicknamesList.Add(player.Owner.NickName);
        }

        CrewLeaderDropDown.ClearOptions();
        CrewLeaderDropDown.AddOptions(nicknamesList);
    }

    private void PopulateDropdownRoles()
    {
        string[] roles = Enum.GetNames(typeof(Roles));
        List<string> rolesList = new List<string>(roles);

        foreach (var dropdown in CrewMemberRoleDropDownList)
        {
            dropdown.AddOptions(rolesList);
        }
    }

    public void CreateCrewSubmit()
    {
        var color = Random.ColorHSV();
        _photonView.RPC("CrewCreateSubmit_RPC", RpcTarget.AllBufferedViaServer, GetCrewRolesByEnum(), GetCrewLeaderIndex());
        _photonView.RPC("ChangeCrewColors", RpcTarget.AllBufferedViaServer, new Vector3(color.r, color.g, color.b));

        //_photonView.RPC("CrewLeaderIsChosen", RpcTarget.AllBufferedViaServer, GetCrewLeader());
    }

    public int[] GetCrewRolesByEnum()
    {
        int[] roles = new int[_playersInRoomList.Count];

        for (int i = 0; i < roles.Length; i++)
        {
            roles[i] = CrewMemberRoleDropDownList[i].value;
        }
        return roles;
    }

    public int GetCrewLeaderIndex()
    {
        int leaderIndex = 0;

        for (int i = 0; i < _playersInRoomList.Count; i++)
        {
            if (CrewLeaderDropDown.GetComponentInChildren<TextMeshProUGUI>().text == _playersInRoomList[i].Owner.NickName)
            {
                leaderIndex = i;
            }

        }
        return leaderIndex;
    }

    // Show Hide MenuUI
    // --------------------
    public void ShowCrewRoomMenu()
    {
        _transfer.CrewUI();
        _photonView.RPC("ShowCrewUI_RPC", RpcTarget.AllBufferedViaServer);

        // RoomCrewMenuUI.gameObject.SetActive(true);
        //RefreshCrewUITexts();
    }

    public void HideCrewRoomMenu()
    {
        _photonView.RPC("CloseCrewUI_RPC", RpcTarget.AllBufferedViaServer);

        // RoomCrewMenuUI.gameObject.SetActive(false);
    }

    public void SpawnPatient()
    {
        if (_crewRoomIndex < 2)
        {
            switch (_crewRoomIndex)
            {
                case 1:
                    PhotonNetwork.Instantiate(_patientMale.name, GameManager.Instance.IncidentPatientSpawns[_crewRoomIndex].position, GameManager.Instance.IncidentPatientSpawns[_crewRoomIndex].rotation);
                    GameManager.Instance.CurrentIncidentsTransforms.Add(GameManager.Instance.IncidentPatientSpawns[_crewRoomIndex]);
                    break;

                case 2:
                    PhotonNetwork.Instantiate(_patientFemale.name, GameManager.Instance.IncidentPatientSpawns[_crewRoomIndex].position, GameManager.Instance.IncidentPatientSpawns[_crewRoomIndex].rotation);
                    break;

                default:
                    break;
            }
        }
    }

    public void SpawnVehicle()
    {
        _photonView.RPC("SpawnVehicle_RPC", RpcTarget.AllBufferedViaServer);
    }


    // Collision Methods
    // --------------------
    private void OnTriggerEnter(Collider other)
    {
        PhotonView playerView = other.GetComponentInParent<PhotonView>();

        if (other.CompareTag("test") && !_playersInRoomList.Contains(playerView))
        {
            _playersInRoomList.Add(playerView);
            _photonView.RPC("SetToUi_RPC", RpcTarget.AllBufferedViaServer);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        PhotonView playerView = other.GetComponentInParent<PhotonView>();

        if (other.CompareTag("test") && !_playersInRoomList.Contains(playerView))
        {
            _playersInRoomList.Add(playerView);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PhotonView playerView = other.GetComponentInParent<PhotonView>();

        if (other.CompareTag("test") && _playersInRoomList.Contains(playerView))
        {
            _photonView.RPC("UpdateUiNameOnExit", RpcTarget.AllBufferedViaServer, playerView.Owner.NickName);
            _playersInRoomList.Remove(playerView);
        }
    }

    // PUN RPC Methods
    // --------------------

    [PunRPC]
    void AddingToRoomList_RPC(string currentPlayer)
    {
        PhotonView currentPlayerView = ActionsManager.Instance.GetPlayerPhotonViewByNickName(currentPlayer);

 
        if (currentPlayerView == null)
        {
            Debug.LogError("CurrentPlayer is Null");

            return;
        }

        for (int i = 0; i < 1; i++)
        {
            if (_playersInRoomList.Contains(currentPlayerView))
            {
                continue;
            }
            else
            {
                _playersInRoomList.Add(currentPlayerView);
            }
        }
        //BlockRoomAccess();
        Debug.LogError("Added to room");

    }

    [PunRPC]
    void RemovingFromRoomList_RPC(string currentPlayer)
    {
        PhotonView currentPlayerView = ActionsManager.Instance.GetPlayerPhotonViewByNickName(currentPlayer);

        if (_playersInRoomList.Contains(currentPlayerView))
        {
            _playersInRoomList.Remove(currentPlayerView);
        }

        Debug.LogError("Remove from room");

    }

    [PunRPC]
    void ChangeCrewColors(Vector3 randomColor)
    {
        crewColor = new Color(randomColor.x, randomColor.y, randomColor.z);

        for (int i = 0; i < _playersInRoomList.Count; i++)
        {
            PlayerData currentPlayerData = _playersInRoomList[i].GetComponent<PlayerData>();
            NameTagDisplay desiredPlayerName = _playersInRoomList[i].GetComponentInChildren<NameTagDisplay>();

            desiredPlayerName.text.color = crewColor;
            currentPlayerData.CrewColor = crewColor;
        }
    }

    [PunRPC]
    void CrewCreateSubmit_RPC(int[] roleIndex, int leaderIndex)
    {
        int indexInCrewCounter = 0;
        for (int i = 0; i < roleIndex.Length; i++)
        {
            PlayerData desiredPlayerData = _playersInRoomList[i].GetComponent<PlayerData>();
            desiredPlayerData.CrewIndex = _crewRoomIndex;
            //desiredPlayerData.CrewIndex = ActionsManager.Instance.NextCrewIndex;
            desiredPlayerData.UserIndexInCrew = indexInCrewCounter;
            desiredPlayerData.UserRole = (Roles)roleIndex[i];
            //MeshFilter vest = desiredPlayerData.transform.GetChild(5).GetChild(0).GetChild(0).GetComponent<MeshFilter>();
            //vest.gameObject.SetActive(true);
            //vest.mesh = ActionsManager.Instance.Vests[(int)desiredPlayerData.UserRole].mesh;
            indexInCrewCounter++;
        }

        foreach (PhotonView player in _playersInRoomList)
        {
            player.GetComponent<PlayerData>().IsCrewLeader = false;
        }

        PlayerData leaderToBe = _playersInRoomList[leaderIndex].GetComponent<PlayerData>();
        leaderToBe.IsCrewLeader = true;

        ActionsManager.Instance.NextCrewIndex++;
    }

    [PunRPC]
    void SpawnVehicle_RPC()
    {
        VehicleChecker currentPosVehicleChecker = ActionsManager.Instance.VehiclePosTransforms[_crewRoomIndex - 1].GetComponent<VehicleChecker>();

        if (!currentPosVehicleChecker.IsPosOccupied)
        {
            /* GameObject natan = */
            PhotonNetwork.InstantiateRoomObject(ActionsManager.Instance.NatanPrefab.name, ActionsManager.Instance.VehiclePosTransforms[_crewRoomIndex - 1].position, ActionsManager.Instance.NatanPrefab.transform.rotation);
            // natan.GetComponent<CarControllerSimple>().OwnerCrew = _crewRoomIndex;
        }
    }

    [PunRPC]
    void ShowCrewUI_RPC()
    {
        RoomCrewMenuUI.gameObject.SetActive(true);
        isUsed = true;
        
    }

    [PunRPC]
    void SetToUi_RPC()
    {
        SetCrewUITexts();
    }

    [PunRPC]
    void UpdateUiNameOnExit(string playerNickName)
    {
        foreach (TextMeshProUGUI memberName in listOfUiNamesTMP)
        {
            if (playerNickName == memberName.text)
            {
                memberName.text = _waitMemberText;
                break;
            }
        }
    }

    [PunRPC]
    void RemoveFromUi_RPC(string currentPlayer)
    {
        for (int i = 0; i < _playersInRoomList.Count; i++)
        {
            if (currentPlayer == PhotonNetwork.NickName && listOfUiNamesTMP[i].text.Contains(currentPlayer))
            {
                listOfUiNamesTMP[i].text.Remove(currentPlayer[i]);
            }
        }
    }

    [PunRPC]
    void CloseCrewUI_RPC()
    {
        isUsed = false;
        RoomCrewMenuUI.gameObject.SetActive(false);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

        if (stream.IsWriting)
        {
            stream.SendNext(CrewLeaderDropDown.value);

            foreach (var dropdown in CrewMemberRoleDropDownList)
            {
                stream.SendNext(dropdown.value);
            }
        }
        else
        {
            CrewLeaderDropDown.value = (int)stream.ReceiveNext();

            foreach (var dropdown in CrewMemberRoleDropDownList)
            {
                dropdown.value = (int)stream.ReceiveNext();
            }


        }
    }
}
