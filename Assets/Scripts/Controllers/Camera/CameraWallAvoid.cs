using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraWallAvoid : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform ogCameraPosition;
    [SerializeField] private float height = 2f;
    [SerializeField] private float smoothTime = .3f;
    //[SerializeField] private float maxDistance = 3f;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] bool ignoreTrigger = true;

    private Vector3 velocity = Vector3.zero;

    void Update()
    {
        // Raycast from the camera to the player
        RaycastHit hit;
        if (Physics.Linecast(transform.position, new Vector3(player.position.x, player.position.y + height, player.position.z), out hit, wallLayer, ignoreTrigger ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.UseGlobal))
        {
            // If the raycast hits a wall, move the camera closer to the player
            transform.position = Vector3.SmoothDamp(transform.position, hit.point, ref velocity, smoothTime);
        }
        else
        {
            // The camera will return to its original position if raycast does not hit the wall
            if (transform.position != ogCameraPosition.position)
            {
                transform.position = Vector3.SmoothDamp(transform.position, ogCameraPosition.position, ref velocity, smoothTime);
            }
            //// If the raycast doesn't hit a wall, move the camera closer to or farther from the player depending on the current distance
            //float distance = Vector3.Distance(transform.position, player.position);
            //if (distance > maxDistance)
            //{
            //    // Move the camera closer to the player if it's too far away
            //    transform.position = Vector3.SmoothDamp(transform.position, player.position, ref velocity, smoothTime);
            //}
            //else if (distance < maxDistance)
            //{
            //    // Move the camera farther from the player if it's too close
            //    Vector3 direction = (transform.position - player.position).normalized;
            //    Vector3 targetPosition = player.position + direction * maxDistance;
            //    targetPosition.y -= height;
            //    transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
            //}
        }
    }
}