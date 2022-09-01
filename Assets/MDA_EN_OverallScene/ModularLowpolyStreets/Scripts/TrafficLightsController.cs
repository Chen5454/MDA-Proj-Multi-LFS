using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/**
 * <summary>Class to control all master traffic lights</summary>
 */
public class TrafficLightsController : MonoBehaviour
{
    private BaseTrafficLight[] allTrafficLights;

    // Start is called before the first frame update
    void Start()
    {
        allTrafficLights = FindObjectsOfType<BaseTrafficLight>();
        if (allTrafficLights.Length > 0)
            allTrafficLights = FilterTrafficLights(allTrafficLights);

    }

    BaseTrafficLight[] FilterTrafficLights(BaseTrafficLight[] source)
    {
        return source.Where(c => c.IsMaster()).ToArray();
    }

    /**
     * <summary>Set all master traffic lights to normal working state</summary>
     */
    public void StartWork()
    {
        if (allTrafficLights != null && allTrafficLights.Length > 0)
            foreach (BaseTrafficLight oneLight in allTrafficLights)
                oneLight.StartSyncMode();
    }

    /**
     * <summary>Set all master traffic lights to idle state</summary>
     */
    public void StartIdle()
    {
        if (allTrafficLights != null && allTrafficLights.Length > 0)
            foreach (BaseTrafficLight oneLight in allTrafficLights)
                oneLight.StartIdle();
    }

    /**
     * <summary>Turn off all master traffic lights</summary>
     */
    public void SetOff()
    {
        if (allTrafficLights != null && allTrafficLights.Length > 0)
            foreach (BaseTrafficLight oneLight in allTrafficLights)
                oneLight.SetOff();
    }
}
