using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarDoorCollision : MonoBehaviour
{
    public bool IsDoorOpen = false;
    public bool IsSeatOccupied = false;
    public int SeatNumber;
    public GameObject CollidingPlayer;
    public Transform SeatPosition;
    private OwnershipTransfer _transfer;

    [SerializeField] private CarControllerSimple _carController;
    private Animator _doorAnimator;

    void Start()
    {
        _transfer = GetComponent<OwnershipTransfer>();
        _doorAnimator = GetComponent<Animator>();

        UIManager.Instance.DriverExitBtn.onClick.RemoveAllListeners();
        UIManager.Instance.PassangerExitBtn.onClick.RemoveAllListeners();

        UIManager.Instance.HeadlightBtn.onClick.RemoveAllListeners();
        UIManager.Instance.SirenBtn.onClick.RemoveAllListeners();

        UIManager.Instance.HeadlightBtn.onClick.AddListener(delegate { ToggleHeadlights(); });
        UIManager.Instance.SirenBtn.onClick.AddListener(delegate { ToggleSiren(); });
    }

    void Update()
    {
        //GetComponent<Collider>().enabled = IsDoorOpen;
        EnterExitVehicle();
    }

    public void OpenCloseDoorToggle(int number)
    {
        if (IsDoorOpen)
        {
            UIManager.Instance.CurrentActionBarParent = number switch
            {
                0 => UIManager.Instance.AmbulanceBar,
                1 => UIManager.Instance.NatanBar,
                _ => UIManager.Instance.AmbulanceBar,
            };

            UIManager.Instance.CurrentActionBarParent.SetActive(true);

            IsDoorOpen = false;
            _doorAnimator.SetBool("IsDoorOpen", false);

        }
        else if (!IsDoorOpen)
        {
            UIManager.Instance.CurrentActionBarParent = number switch
            {
                0 => UIManager.Instance.AmbulanceBar,
                1 => UIManager.Instance.NatanBar,
                _ => UIManager.Instance.AmbulanceBar,
            };

            UIManager.Instance.CurrentActionBarParent.SetActive(true);

            IsDoorOpen = true;
            _doorAnimator.SetBool("IsDoorOpen", true);

            //if (IsSeatOccupied)
            //{
            //    EnterExitToggle();
            //}
        }
        UIManager.Instance.DriverExitBtn.onClick.AddListener(delegate { EnterExitToggle(); });
        UIManager.Instance.PassangerExitBtn.onClick.AddListener(delegate { EnterExitToggle(); });

        EnterExitToggle();
    }

    private void CloseDoor()
    {
        _doorAnimator.SetBool("IsDoorOpen", false);
    }

    private void EnterExitVehicle()
    {
        if (CollidingPlayer)
        {
            if (IsSeatOccupied)
            {
                CollidingPlayer.transform.SetPositionAndRotation(SeatPosition.position, SeatPosition.rotation);
            }
        }
    }

    public void EnterExitToggle()
    {
        if (IsDoorOpen && CollidingPlayer)
        {
            PlayerController playerController = CollidingPlayer.GetComponent<PlayerController>();

            if (!IsSeatOccupied)
            {
                Debug.Log("supposed to drive");
                
                IsSeatOccupied = true;
                playerController.IsDriving = true;
                _doorAnimator.SetBool("IsDoorOpen", false);

                if (SeatNumber == 0)
                {
                    _carController.Transfer.CarDriver();
                    playerController.CurrentCarController = _carController;

                    //playerController.PhotonView.RPC("ChangeCharControllerStateRPC", Photon.Pun.RpcTarget.Others);
                }
                else
                {
                    UIManager.Instance.VehiclePassangerUI.SetActive(true);
                }
                // use player driving state
            }
            else if (IsSeatOccupied)
            {
                Debug.Log("NOT supposed to drive");
                IsSeatOccupied = false;
                CollidingPlayer.transform.position = gameObject.transform.position;
                playerController.IsDriving = false;

                if (SeatNumber == 0)
                {
                    playerController.PlayerData.LastCarController = _carController;
                    //playerController.PhotonView.RPC("ChangeCharControllerStateRPC", Photon.Pun.RpcTarget.Others);
                }
                else
                {
                    UIManager.Instance.VehiclePassangerUI.SetActive(false);
                }
                // use player driving state
            }
        }
    }

    public void ToggleHeadlights()
    {
        Debug.Log("Attempting Toggle Lights");
        if (_carController.CarHeadLightsOn)
        {
            _carController.CarHeadLightsOn = false;
            _carController.CarHeadLights.SetActive(false);
            _carController.CarSiren.GetComponent<Animator>().enabled = false;
            //CarSirenLightLeft.SetActive(false);
            //CarSirenLightRight.SetActive(false);
            Debug.Log("Performed Toggle Lights");
        }
        else
        {
            _carController.CarHeadLightsOn = true;
            _carController.CarHeadLights.SetActive(true);
            _carController.CarSiren.GetComponent<Animator>().enabled = true;
            //CarSirenLightLeft.SetActive(true);
            //CarSirenLightRight.SetActive(true);
            Debug.Log("Performed Toggle Lights");
        }
    }
    public void ToggleSiren()
    {
        Debug.Log("Attempting Toggle Siren");
        if (_carController.CarSirenOn)
        {
            _carController.CarSirenOn = false;
            _carController.CarSirenAudioSource.Stop();
            Debug.Log("Performed Toggle Siren");
        }
        else
        {
            _carController.CarSirenOn = true;
            _carController.CarSirenAudioSource.Play();
            Debug.Log("Performed Toggle Siren");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !IsSeatOccupied)
        {
            CollidingPlayer = other.gameObject;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (!IsSeatOccupied)
        {
            CollidingPlayer = null;
        }
    }
}
