using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleBackDoorsInteraction : MonoBehaviour
{
    [SerializeField] private int _barType;

    private void CloseAllCurrentBarPanels(bool isNatan)
    {
        if (!isNatan)
        {
            UIManager.Instance.AmbulanceNoBagPanel.SetActive(false);
            UIManager.Instance.AmbulanceAmbuPanel.SetActive(false);
            UIManager.Instance.AmbulanceKidsAmbuPanel.SetActive(false);
            UIManager.Instance.AmbulanceMedicPanel.SetActive(false);
            UIManager.Instance.AmbulanceDefibrilationPanel.SetActive(false);
            UIManager.Instance.AmbulanceOxygenPanel.SetActive(false);
            UIManager.Instance.AmbulanceMonitorPanel.SetActive(false);
        }
        else
        {
            UIManager.Instance.NatanNoBagPanel.SetActive(false);
            UIManager.Instance.NatanAmbuPanel.SetActive(false);
            UIManager.Instance.NatanKidsAmbuPanel.SetActive(false);
            UIManager.Instance.NatanMedicPanel.SetActive(false);
            UIManager.Instance.NatanQuickDrugsPanel.SetActive(false);
            UIManager.Instance.NatanDrugsPanel.SetActive(false);
            UIManager.Instance.NatanOxygenPanel.SetActive(false);
            UIManager.Instance.NatanMonitorPanel.SetActive(false);
        }
    }

    public void Interact()
    {
        for (int i = 0; i < ActionsManager.Instance.AllPlayersPhotonViews.Count; i++)
        {
            if (ActionsManager.Instance.AllPlayersPhotonViews[i].IsMine)
            {
                if (UIManager.Instance.CurrentActionBarParent.activeInHierarchy)
                {
                    if (UIManager.Instance.CurrentActionBarParent != UIManager.Instance.NatanBar)
                        CloseAllCurrentBarPanels(false);
                    else
                        CloseAllCurrentBarPanels(true);

                    UIManager.Instance.CurrentActionBarParent.SetActive(false);
                }

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
