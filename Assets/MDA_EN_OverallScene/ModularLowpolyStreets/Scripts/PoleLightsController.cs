using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * <summary>Base class for all pole lights</summary>
 */
public class PoleLightsController : MonoBehaviour
{
    private PoleLight[] allPoleLights;
    // Start is called before the first frame update
    void Start()
    {
        allPoleLights = FindObjectsOfType<PoleLight>();
    }

    /**
     * <summary>Turn on or off pole lights</summary>
     */
    public void LightsOn(bool on = true)
    {
        if (allPoleLights != null && allPoleLights.Length > 0)
            foreach (PoleLight oneLight in allPoleLights)
                oneLight.SetOn(on);
    }
}
