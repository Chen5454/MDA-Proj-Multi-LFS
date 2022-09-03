using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoipSystem : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(Handle3DVoipPositionUpdate(0.3f));
    }

    IEnumerator Handle3DVoipPositionUpdate(float nextUpdate)
    {
        yield return new WaitForSeconds(nextUpdate);
        LoginCredentials.Instance.GetChannelSession.Set3DPosition(transform.position, transform.position, transform.forward, transform.up);

        StartCoroutine(Handle3DVoipPositionUpdate(nextUpdate));

    }
}
