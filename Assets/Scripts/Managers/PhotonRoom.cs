using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class PhotonRoom : MonoBehaviourPunCallbacks,IInRoomCallbacks
{
    public static PhotonRoom Instance;

    private PhotonView _photonView;

   public int _avaterIndex;


    // public float _minX, _minZ, _maxX, _maxZ;

    [SerializeField] public int multiplayerScene;
    [SerializeField] private int currentScene;
     [SerializeField] private TMP_Text currentVersion;
     public string currentVer;
    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            Instance = this;

        }

        currentVersion.text = currentVer;
        DontDestroyOnLoad(this.gameObject);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
       // SceneManager.sceneLoaded += OnFinshedLoading;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
        DisconnectPlayer();
        // SceneManager.sceneLoaded -= OnFinshedLoading;
    }



    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("We are now in a room");
        StartGame();

    }

    void OnFinshedLoading(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene.buildIndex;
    
    }


    void StartGame()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        PhotonNetwork.LoadLevel(1);

    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("OnPlayerLeftRoom Activated Function");

        var views = PhotonNetwork.PhotonViewCollection;

        int leavingPlayerId = otherPlayer.ActorNumber;
        bool isInactive = otherPlayer.IsInactive;

        // SOFT DISCONNECT: A player has timed out to the relay but has not yet exceeded PlayerTTL and may reconnect.
        // Master will take control of this objects until the player hard disconnects, or returns.
        if (isInactive)
        {
            foreach (var view in views)
            {
                // v2.27: changed from owner-check to controller-check
                if (view.ControllerActorNr == leavingPlayerId)
                    view.ControllerActorNr = PhotonNetwork.MasterClient.ActorNumber;
            }

        }
        // HARD DISCONNECT: Player permanently removed. Remove that actor as owner for all items they created (Unless AutoCleanUp is false)
        else
        {
            bool autocleanup = PhotonNetwork.CurrentRoom.AutoCleanUp;

            foreach (var view in views)
            {
                // Skip changing Owner/Controller for items that will be cleaned up.
                if (autocleanup && view.CreatorActorNr == leavingPlayerId)
                    continue;

                // Any views owned by the leaving player, default to null owner (which will become master controlled).
                if (view.OwnerActorNr == leavingPlayerId || view.ControllerActorNr == leavingPlayerId)
                {
                    view.OwnerActorNr = 0;
                    view.ControllerActorNr = PhotonNetwork.MasterClient.ActorNumber;
                }
            }
        }



    }

    public void DisconnectPlayer()
    {
        StartCoroutine(DisconnectAndLoad());

    }

    IEnumerator DisconnectAndLoad()
    {

        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
        {
            yield return null;
        }
        PhotonNetwork.LoadLevel(1);
    }

    public void PickAvatar(int avatarIndex)
    {
        _avaterIndex = avatarIndex;
    }
}
