using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using UnityEngine;
using Photon.Pun;

[System.Serializable]
public enum Roles { CFR, Medic, SeniorMedic, Paramedic, Doctor,None }

[System.Serializable]
public enum AranRoles { None, HeadMokdan, Mokdan, Pikud10, Refua10, Henyon10, Pinuy10 }

public class PlayerData : MonoBehaviourPunCallbacks,IPunObservable
{
    public PhotonView PhotonView => gameObject.GetPhotonView();
    public bool IsJoinedNearbyPatient => CurrentPatientNearby.IsPlayerJoined(this);

    [field: SerializeField] public string UserName { get; set; }
    [field: SerializeField] public string CrewName { get; set; }
    [field: SerializeField] public int CrewIndex { get; set; }
    [field: SerializeField] public bool IsCrewLeader { get; set; }
    [field: SerializeField] public bool IsInstructor { get; set; }
    [field: SerializeField] public bool IsMokdan { get; set; }
    [field: SerializeField] public bool IsPikud10 { get; set; }
    [field: SerializeField] public bool IsRefua10 { get; set; }
    [field: SerializeField] public bool IsPinuy10 { get; set; }
    [field: SerializeField] public bool IsHenyon10 { get; set; }
    [field: SerializeField] public bool IsDataInitialized { get; set; }
    [field: SerializeField] public Roles UserRole { get; set; }
    [field: SerializeField] public AranRoles AranRole { get; set; }
    [field: SerializeField] public Color CrewColor { get; set; }
    [field: SerializeField] public Patient CurrentPatientNearby { get; set; }
    [field: SerializeField] public Animation PlayerAnimation { get; set; }
    [field: SerializeField] public CarControllerSimple LastCarController { get; set; }
    [field: SerializeField] public VehicleController LastVehicleController { get; set; }


    #region MonobehaviourCallbacks
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {

        if (PhotonView.IsMine)
        {
         //   PhotonView.RPC("AddingPlayerToAllPlayersList", RpcTarget.AllViaServer);
            UserRole = Roles.None;
            AranRole = AranRoles.None;
            CrewColor = Color.white;
        }

        PhotonView.ObservedComponents.Add(this);

    }

    private void OnDestroy()
    {
        ActionsManager.Instance.AllPlayersPhotonViews.Remove(PhotonView);
    }


    #endregion

    #region Private Methods
    private void TryClearAranRoleByOrder()
    {
        if (TryGetComponent(out Mokdan mokdan))
        {
            Destroy(mokdan);
        }
        else if (TryGetComponent(out Pikud10 pikud10))
        {
            Destroy(pikud10);
        }
        else if (TryGetComponent(out Refua10 refua10))
        {
            Destroy(refua10);
        }
        else if (TryGetComponent(out Henyon10 henyon10))
        {
            Destroy(henyon10);
        }
        else if (TryGetComponent(out Pinuy10 pinuy10))
        {
            Destroy(pinuy10);
        }
    }
    private void ClearAllRoles()
    {
        if (TryGetComponent(out Mokdan mokdan))
        {
            Destroy(mokdan);
        }
        if (TryGetComponent(out Pikud10 pikud10))
        {
            Destroy(pikud10);
        }
        if (TryGetComponent(out Refua10 refua10))
        {
            Destroy(refua10);
        }
        if (TryGetComponent(out Henyon10 henyon10))
        {
            Destroy(henyon10);
        }
        if (TryGetComponent(out Pinuy10 pinuy10))
        {
            Destroy(pinuy10);
        }

        IsMokdan = false;
        IsPikud10 = false;
        IsRefua10 = false;
        IsHenyon10 = false;
        IsPinuy10 = false;
        AranRole = AranRoles.None;
    }
    private void ClearAranRole(Mokdan mokdan)
    {
        if (mokdan = GetComponent<Mokdan>())
            Destroy(mokdan);
    }
    private void ClearAranRole(Pikud10 pikud10)
    {
        if (pikud10 = GetComponent<Pikud10>())
            Destroy(pikud10);
    }
    private void ClearAranRole(Refua10 refua10)
    {
        if (refua10 = GetComponent<Refua10>())
            Destroy(refua10);
    }
    private void ClearAranRole(Pinuy10 pinuy10)
    {
        if (pinuy10 = GetComponent<Pinuy10>())
            Destroy(pinuy10);
    }
    private void ClearAranRole(Henyon10 henyon10)
    {
        if (henyon10 = GetComponent<Henyon10>())
            Destroy(henyon10);
    }
    #endregion

    #region Public Methods
    public void AssignAranRole(AranRoles newRole)
    {
        switch (newRole)
        {
            case AranRoles.None:
                ClearAllRoles();
                break;
            case AranRoles.HeadMokdan:
                ClearAllRoles();
                gameObject.AddComponent<Mokdan>();
                IsMokdan = true;
                AranRole = newRole;
                break;
            case AranRoles.Mokdan:
                ClearAllRoles();
                gameObject.AddComponent<Mokdan>();
                IsMokdan = true;
                AranRole = newRole;
                break;
            case AranRoles.Pikud10:
                ClearAllRoles();
                gameObject.AddComponent<Pikud10>();
                IsPikud10 = true;
                AranRole = newRole;
                break;
            case AranRoles.Refua10:
                ClearAllRoles();
                gameObject.AddComponent<Refua10>();
                IsRefua10 = true;
                AranRole = newRole;
                break;
            case AranRoles.Henyon10:
                ClearAllRoles();
                gameObject.AddComponent<Henyon10>();
                IsHenyon10 = true;
                AranRole = newRole;
                break;
            case AranRoles.Pinuy10:
                ClearAllRoles();
                gameObject.AddComponent<Pinuy10>();
                IsPinuy10 = true;
                AranRole = newRole;
                break;
            default:
                break;
        }
    }
    #endregion

    #region PunRPC invoked by Player
    //[PunRPC]
    //void AddingPlayerToAllPlayersList()
    //{
    //    ActionsManager.Instance.AllPlayersPhotonViews.Add(PhotonView);
    //}



    [PunRPC]
    private void OnLeavePatient(int patientViewID)
    {
        for (int i = 0; i < GameManager.Instance.AllPatients.Count; i++)
        {
            if (GameManager.Instance.AllPatients[i].PhotonView.ViewID == patientViewID)
            {
                Debug.Log("Attempting leave patient");
                GameManager.Instance.AllPatients[i].TreatingUsers.Remove(this);
                Debug.Log("Left Patient Succesfully");
                break;
            }
        }
    }

    [PunRPC]
    private void AssignAranRoleRPC(int newRoleIndex)
    {
        AranRoles newRole = (AranRoles)newRoleIndex;

        switch (newRole)
        {
            case AranRoles.None:
                TryClearAranRoleByOrder();
                break;
            case AranRoles.HeadMokdan:
                gameObject.AddComponent<Mokdan>();
                IsMokdan = false;
                AranRole = newRole;
                break;
            case AranRoles.Mokdan:
                gameObject.AddComponent<Mokdan>();
                IsMokdan = false;
                AranRole = newRole;
                break;
            case AranRoles.Pikud10:
                gameObject.AddComponent<Pikud10>();
                IsPikud10 = false;
                AranRole = newRole;
                break;
            case AranRoles.Refua10:
                gameObject.AddComponent<Refua10>();
                IsRefua10 = false;
                AranRole = newRole;
                break;
            case AranRoles.Henyon10:
                gameObject.AddComponent<Henyon10>();
                IsHenyon10 = false;
                AranRole = newRole;
                break;
            case AranRoles.Pinuy10:
                gameObject.AddComponent<Pinuy10>();
                IsPinuy10 = false;
                AranRole = newRole;
                break;
            default:
                break;
        }
    }
    #endregion

    #region Pikud10 RPC
    [PunRPC]
    private void DropdownPlayersNickNamesPikud10()
    {
        if (IsPikud10 && PhotonView.IsMine)
        {
            List<string> value = new List<string>();
            foreach (PhotonView player in ActionsManager.Instance.AllPlayersPhotonViews)
            {
                if (player.GetComponent<PlayerData>().IsCrewLeader)
                    value.Add(player.Owner.NickName);
            }

            Pikud10 pikud10 = GetComponent<Pikud10>();
            pikud10.PlayerListDropdownRefua10.ClearOptions();
            pikud10.PlayerListDropdownRefua10.AddOptions(value);
            pikud10.PlayerListDropdownPinuy10.ClearOptions();
            pikud10.PlayerListDropdownPinuy10.AddOptions(value);
            pikud10.PlayerListDropdownHenyon10.ClearOptions();
            pikud10.PlayerListDropdownHenyon10.AddOptions(value);
        }

    }

    [PunRPC]
    public void GiveRefuaRole(int index)
    {
        foreach (PhotonView player in ActionsManager.Instance.AllPlayersPhotonViews)
        {
            PlayerData playerData = player.GetComponent<PlayerData>();
            playerData.IsRefua10 = false;

            if (playerData.AranRole == AranRoles.Refua10)
            {
                playerData.AssignAranRole(AranRoles.None);
            }
        }

        PlayerData chosenPlayerData = ActionsManager.Instance.AllPlayersPhotonViews[index].GetComponent<PlayerData>();
        chosenPlayerData.IsRefua10 = true;
        chosenPlayerData.AssignAranRole(AranRoles.Refua10);

        UIManager.Instance.HenyonParent.SetActive(false);
        UIManager.Instance.TeamLeaderParent.SetActive(false);
        UIManager.Instance.Pinuy10Parent.SetActive(false);
    }

    [PunRPC]
    public void GivePinoyeRole(int index)
    {
        foreach (PhotonView player in ActionsManager.Instance.AllPlayersPhotonViews)
        {
            PlayerData playerData = player.GetComponent<PlayerData>();
            playerData.IsPinuy10 = false;

            if (playerData.AranRole == AranRoles.Pinuy10)
            {
                playerData.AssignAranRole(AranRoles.None);
            }
        }

        PlayerData chosenPlayerData = ActionsManager.Instance.AllPlayersPhotonViews[index].GetComponent<PlayerData>();
        chosenPlayerData.IsPinuy10 = true;
        chosenPlayerData.AssignAranRole(AranRoles.Pinuy10);

        UIManager.Instance.RefuaParent.SetActive(false);
        UIManager.Instance.TeamLeaderParent.SetActive(false);
        UIManager.Instance.HenyonParent.SetActive(false);
    }

    [PunRPC]
    public void GiveHenyonRole(int index)
    {
        foreach (PhotonView player in ActionsManager.Instance.AllPlayersPhotonViews)
        {
            PlayerData playerData = player.GetComponent<PlayerData>();
            playerData.IsHenyon10 = false;

            if (playerData.AranRole == AranRoles.Henyon10)
            {
                playerData.AssignAranRole(AranRoles.None);
            }
        }

        PlayerData chosenPlayerData = ActionsManager.Instance.AllPlayersPhotonViews[index].GetComponent<PlayerData>();
        chosenPlayerData.IsHenyon10 = true;
        chosenPlayerData.AssignAranRole(AranRoles.Henyon10);

        //need to disable the other panels.
        UIManager.Instance.RefuaParent.SetActive(false);
        UIManager.Instance.TeamLeaderParent.SetActive(false);
        UIManager.Instance.Pinuy10Parent.SetActive(false);

    }

    //[PunRPC]
    //public void ActivateAreaMarkingRPC(int markerIndex)
    //{
    //    if (TryGetComponent(out Pikud10 pikud10))
    //    {
    //        pikud10.CreateMarkedArea(markerIndex, GetComponent<CameraController>());
    //    }
    //}

    [PunRPC]
    public void SpectatePikudCamera_RPC()
    {

        if (GameManager.Instance.Pikud10View != null)
        {
            Pikud10 pikud10 = GameManager.Instance.Pikud10View.GetComponent<Pikud10>();
            GameManager.Instance.Pikud10View = PhotonView;

            if (pikud10 != null)
            {
                if (GameManager.Instance.Pikud10TextureRenderer != null)
                {
                    pikud10.Pikud10Camera.targetTexture = GameManager.Instance.Pikud10TextureRenderer;
                    pikud10.Pikud10Camera.gameObject.SetActive(true);
                }
                else
                {
                    Debug.LogError("GameManager.Instance.Pikud10TextureRenderer is null, cannot set target texture");
                }

            }
            else
            {
                Debug.LogError("Pikud10 component not found on GameManager.Instance.Pikud10View");
            }
        }
        else
        {
            Debug.LogError("GameManager.Instance.Pikud10View is null, cannot get Pikud10 component");
        }
   
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

        if (stream.IsWriting)
        {
            stream.SendNext(IsRefua10);
            stream.SendNext(IsCrewLeader);
            stream.SendNext(IsInstructor);
            stream.SendNext(IsMokdan);
            stream.SendNext(IsPikud10);
            stream.SendNext(IsPinuy10);
            stream.SendNext(IsHenyon10);
            stream.SendNext(CrewIndex);
            stream.SendNext(IsDataInitialized);
            stream.SendNext(UserRole);

        }
        else
        {
            IsRefua10 = (bool)stream.ReceiveNext();
            IsCrewLeader = (bool)stream.ReceiveNext();
            IsInstructor = (bool)stream.ReceiveNext();
            IsMokdan = (bool)stream.ReceiveNext();
            IsPikud10 = (bool)stream.ReceiveNext();
            IsPinuy10 = (bool)stream.ReceiveNext();
            IsHenyon10 = (bool)stream.ReceiveNext();
            CrewIndex = (int)stream.ReceiveNext();
            IsDataInitialized = (bool)stream.ReceiveNext();
            UserRole = (Roles)stream.ReceiveNext();


        }
    }
    #endregion

    
}
