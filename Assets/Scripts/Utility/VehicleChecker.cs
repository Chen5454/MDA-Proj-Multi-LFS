using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleChecker : MonoBehaviour
{
    public bool IsPosOccupied;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Car")
        {
            for (int i = 0; i < ActionsManager.Instance.VehiclePosTransforms.Count; i++)
            {
                Transform natanPos = ActionsManager.Instance.VehiclePosTransforms[i];

                if (natanPos.position.x == natanPos.position.x)
                {
                    IsPosOccupied = true;
                    ActionsManager.Instance.VehiclePosOccupiedList[i] = IsPosOccupied;
                    break;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Car")
        {
            for (int i = 0; i < ActionsManager.Instance.VehiclePosTransforms.Count; i++)
            {
                Transform natanPos = ActionsManager.Instance.VehiclePosTransforms[i];

                if (natanPos.position.x == natanPos.position.x)
                {
                    IsPosOccupied = false;
                    ActionsManager.Instance.VehiclePosOccupiedList[i] = IsPosOccupied;
                    break;
                }
            }
        }
    }
}
