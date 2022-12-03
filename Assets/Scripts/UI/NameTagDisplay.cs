using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class NameTagDisplay : MonoBehaviour
{
    [SerializeField] private PhotonView playerPhotonView;
    [SerializeField] private GameObject crown;
    [SerializeField] public TMP_Text text;

    private PlayerData playerData;
    
    private void Start()
    {
        if (!playerData)
        {
            if (playerPhotonView.GetComponent<PlayerData>())
            {
                playerData = playerPhotonView.GetComponent<PlayerData>();
            }
            else
            {
                return;
            }

            text.text = playerPhotonView.Owner.NickName;
            playerData.UserName = playerPhotonView.Owner.NickName;
            playerPhotonView.gameObject.name = playerPhotonView.Owner.NickName;
        }
        //if (playerPhotonView.IsMine) // if we are the local player we disable the text
        //{
        //    gameObject.SetActive(false);
        //}
    }

    private void Update()
    {
        if (playerData)
        {
            if (playerData.IsCrewLeader && !crown.activeInHierarchy)
            {
                crown.SetActive(true);
            }
            else if (!playerData.IsCrewLeader && crown.activeInHierarchy)
            {
                crown.SetActive(false);
            }
            else
            {
                return;
            }
        }
        else
        {
            return;
        }
    }
}
