using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleCameraFollow : MonoBehaviour
{
    [SerializeField] private Vector3 _offset;
    [SerializeField] private float _translateSpeed;
    [SerializeField] private float _rotationSpeed;

    public Transform Target;

    private void FixedUpdate()
    {
        if (!Target)
            return;

        HandleTranslation();
        HandleRotation();
    }

    private void HandleTranslation()
    {
        Vector3 targetPosition = Target.TransformPoint(_offset);
        transform.position = Vector3.Lerp(transform.position, targetPosition, _translateSpeed * Time.deltaTime);
    }
    private void HandleRotation()
    {
        Vector3 direction = Target.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, _rotationSpeed * Time.deltaTime);
    }
}
