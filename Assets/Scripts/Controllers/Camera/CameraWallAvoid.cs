using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraWallAvoid : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _ogCameraPosition;
    [SerializeField] private float _height = 2f;
    [SerializeField] private float _smoothTime = .3f;
    //[SerializeField] private float maxDistance = 3f;
    [SerializeField] private LayerMask _wallLayer;
    [SerializeField] bool _ignoreTrigger = true;

    private Vector3 _velocity = Vector3.zero;

    void Update()
    {
        // Raycast from the camera to the player
        RaycastHit hit;
        if (Physics.Linecast(transform.position, new Vector3(_player.position.x, _player.position.y + _height, _player.position.z), out hit, _wallLayer, _ignoreTrigger ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.UseGlobal))
        {
            // If the raycast hits a wall, move the camera closer to the player
            transform.position = Vector3.SmoothDamp(transform.position, hit.point, ref _velocity, _smoothTime);
        }
        else
        {
            // The camera will return to its original position if raycast does not hit the wall
            if (transform.position != _ogCameraPosition.position)
            {
                transform.position = Vector3.SmoothDamp(transform.position, _ogCameraPosition.position, ref _velocity, _smoothTime);
            }
        }
    }
}