using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
* <summary>Class to control one pole light</summary>
*/
public class PoleLight : MonoBehaviour
{
    GameObject emission;
    // Start is called before the first frame update
    void Start()
    {
        emission = transform.GetChild(0).gameObject;
    }

    /**
     * <summary>Turn on or off pole light</summary>
     * <param name="on"><c>true</c> to turn on, <c>false</c> to turn off</param>
     */
    public void SetOn(bool on = true)
    {
        if (emission != null)
            emission.SetActive(on);
    }
}
