using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleBackDoorsInteraction : MonoBehaviour
{
    [SerializeField] private int _barType;

    public void Interact()
    {
        for (int i = 0; i < ActionsManager.Instance.AllPlayersPhotonViews.Count; i++)
        {
            if (ActionsManager.Instance.AllPlayersPhotonViews[i].IsMine)
            {
                if (UIManager.Instance.CurrentActionBarParent.activeInHierarchy)
                    UIManager.Instance.CurrentActionBarParent.SetActive(false);

                UIManager.Instance.CurrentActionBarParent = _barType switch
                {
                    0 => UIManager.Instance.AmbulanceBar,
                    1 => UIManager.Instance.NatanBar,
                    _ => UIManager.Instance.AmbulanceBar,
                };

                UIManager.Instance.CurrentActionBarParent.SetActive(true);
            }
        }
    }
}
