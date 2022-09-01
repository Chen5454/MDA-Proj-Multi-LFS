using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Instructor : MonoBehaviour
{
    private void StartAran()
    {
        foreach (PlayerData playerData in ActionsManager.Instance.AllPlayerData)
        {
            playerData.CrewIndex = 0;
            playerData.CrewColor = Color.white;
            playerData.IsCrewLeader = false;
        }
    }
}
