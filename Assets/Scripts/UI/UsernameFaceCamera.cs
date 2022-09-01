using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsernameFaceCamera : MonoBehaviour
{
    private Transform _mainCam;

    private void Start()
    {
        _mainCam = Camera.main.transform;
    }

    void Update()
    {
        transform.LookAt(transform.position + _mainCam.rotation * Vector3.forward, _mainCam.rotation * Vector3.up);
    }
}
