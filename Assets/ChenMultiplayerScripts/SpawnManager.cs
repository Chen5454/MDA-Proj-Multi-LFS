using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{

    public static SpawnManager Instance;

    [Header("PlayerPrefabs")]
    [SerializeField] public GameObject _playerMalePrefab;
    //[SerializeField] private GameObject _playerFemalePrefab;

    [Header("PatientPrefabs")]
    [SerializeField] private GameObject _patientMalePrefab;
    [SerializeField] private GameObject _patientFemalePrefab;//, _patientMaleOldPrefab, _patientFemaleOldPrefab, _patientKid, _patientToddler;

    [Header("VehiclesPrefabs")]
    //[SerializeField] private GameObject _ambulancePrefab;
    [SerializeField] private GameObject _natanPrefab;

    [Header("GeneralPrefabs")]
    [SerializeField] private GameObject _crewRoomColliderPrefab;
    [SerializeField] private GameObject _eranRoomPrefab;
    [SerializeField] private GameObject _operationRoomPrefab;


    [Header("Transform Positions In Scene")]
    [SerializeField] private Transform _patientMalePosTransform;
    [SerializeField] private Transform _patientFemalePosTransform;
    [SerializeField] private Transform _eranRoomPosTransform;
    [SerializeField] private Transform _operationRoomTransform;
    [SerializeField] private Transform /*_ambulancePosTransform,*/ _natanPosTransform;
    [SerializeField] private Transform[] _crewRoomPosTransforms; // crew rooms collider positions in scene

    public float _minX, _minZ, _maxX, _maxZ;

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

    void Start()
    {
        Vector3 randomPos = new Vector3(Random.Range(_minX, _maxX), 1.3f, Random.Range(_minZ, _maxZ));
        PhotonNetwork.Instantiate(_playerMalePrefab.name, randomPos, Quaternion.identity);

        PhotonNetwork.InstantiateRoomObject(_patientMalePrefab.name, _patientMalePosTransform.position, _patientMalePrefab.transform.rotation);
        //PhotonNetwork.InstantiateRoomObject(_patientFemalePrefab.name, _patientFemalePosTransform.position, _patientFemalePrefab.transform.rotation);


        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.InstantiateRoomObject(_eranRoomPrefab.name, _eranRoomPosTransform.position,_eranRoomPosTransform.rotation);
            PhotonNetwork.InstantiateRoomObject(_operationRoomPrefab.name, _operationRoomTransform.position, _operationRoomTransform.rotation);

            foreach (Transform crewRoomPosTr in _crewRoomPosTransforms)
            {
                PhotonNetwork.InstantiateRoomObject(_crewRoomColliderPrefab.name, crewRoomPosTr.position, crewRoomPosTr.rotation);
            }
        }   
    }
}
