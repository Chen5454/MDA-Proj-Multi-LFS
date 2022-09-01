using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class PhotonRoom : MonoBehaviourPunCallbacks,IInRoomCallbacks
{
    public static PhotonRoom Instance;

    private PhotonView _photonView;

   // public float _minX, _minZ, _maxX, _maxZ;

    [SerializeField] public int multiplayerScene;
    [SerializeField] private int currentScene;

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
       // SceneManager.sceneLoaded -= OnFinshedLoading;
    }



    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("We are now in a room");
        //if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        //{
        //    PhotonNetwork.LoadLevel(multiplayerScene);
        //}
        StartGame();

    }

    void OnFinshedLoading(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene.buildIndex;
    
    }

    //private void CreatePlayer()
    //{
    //    Vector3 randomPos = new Vector3(Random.Range(_minX, _maxX), 1.3f, Random.Range(_minZ, _maxZ));

    //    PhotonNetwork.Instantiate(SpawnManager.Instance._playerMalePrefab.name,randomPos,Quaternion.identity,0);

    //}

    void StartGame()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        PhotonNetwork.LoadLevel(1);

    }
}
