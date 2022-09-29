using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsernameFaceCamera : MonoBehaviour
{
    private Transform _mainCam;

    private void Start()
    {
        try
        {
            _mainCam = Camera.main.transform;
        }
        catch (Exception)
        {

            Debug.Log("No Camera");
        }
    }

    void Update()
    {
        if (_mainCam)
        {
            transform.LookAt(transform.position + _mainCam.rotation * Vector3.forward, _mainCam.rotation * Vector3.up);
            return;
        }

        try
        {
            _mainCam = Camera.main.transform;
        }
        catch (Exception)
        {

            Debug.Log("No Camera");
        }
    }
}
