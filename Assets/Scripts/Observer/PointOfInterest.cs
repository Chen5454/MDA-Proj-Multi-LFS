using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointOfInterest : MonoBehaviour
{
    public static event Action<PointOfInterest> OnPointOfInterest;

    [SerializeField]
    private string _name;
    public string Name { get { return _name; } }

    public void Trigger()
    {
        OnPointOfInterest?.Invoke(this);
    }
}
