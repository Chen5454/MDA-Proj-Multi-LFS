using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class EranCrew : MonoBehaviour
{
    private PhotonView _photonView;
    private OwnershipTransfer _transfer;
    private Coroutine updatePlayerListCoroutine;

    #region Mokdan Variables
    [Header("Metargel")]
    public GameObject MetargelEranMenuUI;

    [SerializeField] private GameObject _mokdanDropDown;
    [SerializeField] private GameObject _mainMokdanDropDown;
    [SerializeField] private TMP_Dropdown _mainMokdanPlayerListDropDown;
    [SerializeField] private TMP_Dropdown _mokdanPlayerListDropDown;
    #endregion

    void Start()
    {
        _transfer = GetComponent<OwnershipTransfer>();
        _photonView = GetComponent<PhotonView>();
    }

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
    private void StartAran_RPC()
    {
        GameManager.Instance.IsAranActive = true;
    }

    [PunRPC]
    private void StopAran_RPC()
    {
        GameManager.Instance.IsAranActive = false;
    }
    #endregion
}
