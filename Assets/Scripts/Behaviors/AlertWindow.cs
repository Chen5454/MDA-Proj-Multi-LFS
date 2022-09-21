using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertWindow : MonoBehaviour
{
    [SerializeField] private float _alertTimer = 3;
    private void OnEnable()
    {
        StartCoroutine(CloseAlert());
    }

    private IEnumerator CloseAlert()
    {
        yield return new WaitForSeconds(_alertTimer);

        gameObject.SetActive(false);
        StopCoroutine(CloseAlert());
    }
}
