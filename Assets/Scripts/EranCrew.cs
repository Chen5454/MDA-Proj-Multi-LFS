using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using TMPro;
using UnityEngine;

public enum AranType { PlaineCrash}

public class EranCrew : MonoBehaviour
{
    [SerializeField] private string _startAranTitle, _startAranText;

    private PhotonView _photonView;
    private OwnershipTransfer _transfer;
    private Coroutine updatePlayerListCoroutine;


    #region Metargel Variables
    [Header("Metargel")]
    public GameObject MetargelEranMenuUI;

    [SerializeField] private GameObject _mokdanDropDown;
    [SerializeField] private GameObject _mainMokdanDropDown;
    [SerializeField] private TMP_Dropdown _mainMokdanPlayerListDropDown;
    [SerializeField] private TMP_Dropdown _mokdanPlayerListDropDown;

    #endregion

    #region Metargel Variables
    [SerializeField] private GameObject[] _aranPrefabs;
    #endregion
    void Start()
    {
        _transfer = GetComponent<OwnershipTransfer>();
        _photonView = GetComponent<PhotonView>();
    }


    #region StopEranMethods

    [PunRPC]
    public void ResetPlayerData_RPC()
    {
        foreach (var player in ActionsManager.Instance.AllPlayersPhotonViews)
        {
            PlayerData currentPlayerData = player.GetComponent<PlayerData>();
            NameTagDisplay desiredPlayerName = player.GetComponentInChildren<NameTagDisplay>();
            PlayerController currentPlayer = player.GetComponentInChildren<PlayerController>();


            currentPlayerData.UserRole = 0;
            //currentPlayerData.UserIndexInCrew = 0;
            currentPlayerData.CrewIndex = 0;
            desiredPlayerName.text.color = Color.white;
            currentPlayerData.CrewColor = Color.white;
            currentPlayer.Vest.SetActive(false);
        }
    }

    #endregion


    #region Metargel Methods
    public int GetMokdanIndex()
    {
        int Index = 0;

        for (int i = 0; i < ActionsManager.Instance.AllPlayersPhotonViews.Count; i++)
        {
            if (_mokdanDropDown.GetComponentInChildren<TextMeshProUGUI>().text == ActionsManager.Instance.AllPlayersPhotonViews[i].Owner.NickName)
            {
                Index = i;
            }
        }
        return Index;
    }
    public int GetMainMokdanIndex()
    {
        int Index = 0;

        for (int i = 0; i < ActionsManager.Instance.AllPlayersPhotonViews.Count; i++)
        {
            if (_mainMokdanDropDown.GetComponentInChildren<TextMeshProUGUI>().text == ActionsManager.Instance.AllPlayersPhotonViews[i].Owner.NickName)
            {
                Index = i;
            }
        }
        return Index;
    }
    public void GiveMokdanRoleClick()
    {
        _photonView.RPC("GiveMokdanRole", RpcTarget.AllBufferedViaServer, GetMokdanIndex());
    }
    public void GiveMainMokdanRoleClick()
    {
        _photonView.RPC("GiveMainMokdanRole", RpcTarget.AllBufferedViaServer, GetMainMokdanIndex());
    }
    public void ShowMetargelMenu()
    {
        _transfer.TvOwner();
        MetargelEranMenuUI.SetActive(true);
        updatePlayerListCoroutine = StartCoroutine(HandleDropDownUpdates(0.5f));

    }
    public void StartAran()
    {
        _photonView.RPC("ResetsEventsLists_RPC",RpcTarget.AllBufferedViaServer);
        _photonView.RPC("ResetPlayerData_RPC", RpcTarget.AllBufferedViaServer);
        ClearAllPatient();

        // should be replaced later with the Create Aran UI and behaviours
        PhotonNetwork.Instantiate(_aranPrefabs[0].name, new Vector3(-130f, 0f, 210f), Quaternion.identity);
        GameManager.Instance.ChangeAranState(true);

        GameManager.Instance.photonView.RPC("SetPopUp",RpcTarget.All, _startAranTitle, _startAranText);
        //GameManager.Instance.SetPopUp(_startAranTitle, _startAranText);


    }

    public void ClearAllPatient()
    {
        foreach (var patient in GameManager.Instance.AllPatients.ToList())
        {
            PhotonNetwork.Destroy(patient.gameObject);
        }

    }
    public void StopAran()
    {
        GameManager.Instance.ChangeAranState(false);
    }
    public void CloseMetargelRoomMenu()
    {
        StopCoroutine(updatePlayerListCoroutine);
        MetargelEranMenuUI.SetActive(false);
    }
    #endregion

    #region Pikod10 Methods

    public void GivePikud10Role()
    {
        //_photonView.RPC("GivePikud10RoleRPC", RpcTarget.AllBufferedViaServer, GetRefuaIndex());
    }

    IEnumerator HandleDropDownUpdates(float nextUpdate)
    {
        while (true)
        {
            if (ActionsManager.Instance.AllPlayersPhotonViews.Count != _mainMokdanPlayerListDropDown.options.Count)
            {
                _photonView.RPC("DropdownPlayersNickNamesMetargel", RpcTarget.AllBufferedViaServer);
            }

            if (ActionsManager.Instance.AllPlayersPhotonViews.Count != _mokdanPlayerListDropDown.options.Count)
            {
                _photonView.RPC("DropdownPlayersNickNamesMetargel", RpcTarget.AllBufferedViaServer);
            }
            yield return new WaitForSeconds(nextUpdate);
        }
   
        // StartCoroutine(HandleDropDownUpdates(nextUpdate));
    }
    #endregion

    #region PunRpc
    [PunRPC]
    private void DropdownPlayersNickNamesMetargel()
    {
        List<string> value = new List<string>();
        foreach (PhotonView player in ActionsManager.Instance.AllPlayersPhotonViews)
        {
            
            value.Add(player.Owner.NickName);
        }

        //Metargel Player list
        _mainMokdanPlayerListDropDown.ClearOptions();
        _mainMokdanPlayerListDropDown.AddOptions(value);
        _mokdanPlayerListDropDown.ClearOptions();
        _mokdanPlayerListDropDown.AddOptions(value);
    }

    [PunRPC]
    public void GiveMokdanRole(int index)
    {
        PlayerData chosenPlayerData = ActionsManager.Instance.AllPlayersPhotonViews[index].GetComponent<PlayerData>();
        chosenPlayerData.IsMokdan = true;
        chosenPlayerData.AssignAranRole(AranRoles.Mokdan);

        Mokdan mokdanPlayer = ActionsManager.Instance.AllPlayersPhotonViews[index].GetComponent<Mokdan>();
        mokdanPlayer.isMainMokdan = false;
    }

    [PunRPC]
    public void GiveMainMokdanRole(int index)
    {
        PlayerData chosenPlayerData = ActionsManager.Instance.AllPlayersPhotonViews[index].GetComponent<PlayerData>();
        chosenPlayerData.IsMokdan = true;
        chosenPlayerData.AssignAranRole(AranRoles.Mokdan);


        Mokdan mokdanPlayer = ActionsManager.Instance.AllPlayersPhotonViews[index].GetComponent<Mokdan>();
        mokdanPlayer.isMainMokdan = true;
    }

    [PunRPC]
    public void GivePikud10RoleRPC(int index)
    {
        foreach (PhotonView player in ActionsManager.Instance.AllPlayersPhotonViews)
        {
            PlayerData playerData = player.GetComponent<PlayerData>();
            playerData.IsPikud10 = false;

            if (playerData.AranRole == AranRoles.Pikud10)
            {
                playerData.AssignAranRole(AranRoles.None);
            }
        }

        PlayerData chosenPlayerData = ActionsManager.Instance.AllPlayersPhotonViews[index].GetComponent<PlayerData>();
        chosenPlayerData.IsPikud10 = true;
        chosenPlayerData.AssignAranRole(AranRoles.Pikud10);
    }

    [PunRPC]
    public void ResetsEventsLists_RPC()
    {
    
        foreach (var ambulanceCar in GameManager.Instance.AmbulanceCarList.ToList())
        {
            GameManager.Instance.AmbulanceCarList.Remove(ambulanceCar);
            Destroy(ambulanceCar.gameObject);
        }
        foreach (var natanCar in GameManager.Instance.NatanCarList.ToList())
        {
            GameManager.Instance.NatanCarList.Remove(natanCar);
            Destroy(natanCar.gameObject);
        }
    }
    #endregion
}
