using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AutomaticDoors : MonoBehaviour
{
    [SerializeField] private bool _isSingleDoor, _isOpen;
    [SerializeField] private float _moveDuration = 0.65f;
    [SerializeField] private GameObject _leftDoor, _rightDoor;
    [SerializeField] private Vector3 _leftDoorStart, _leftDoorEnd;
    [SerializeField] private Vector3 _rightDoorStart, _rightDoorEnd;
    [SerializeField] private string _playerTag;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(_playerTag))
        {
            if (!_isOpen)
                StartCoroutine(LerpDoors(_moveDuration));
            else
                return;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(_playerTag))
        {
            if (_isOpen)
                StartCoroutine(LerpDoors(_moveDuration));
            else
                return;
        }
    }

    private IEnumerator LerpDoors(float duration)
    {
        float time = 0;

        if (!_isOpen)
        {
            while (time < duration)
            {
                _leftDoor.transform.localPosition = Vector3.Lerp(_leftDoorEnd, _leftDoorStart, time / duration);
                _rightDoor.transform.localPosition = Vector3.Lerp(_rightDoorEnd, _rightDoorStart, time / duration);
                time += Time.deltaTime;
                yield return null;
            }

            _leftDoor.transform.localPosition = _leftDoorStart;
            _rightDoor.transform.localPosition = _rightDoorStart;
            _isOpen = true;
        }
        else
        {
            while (time < duration)
            {
                _leftDoor.transform.localPosition = Vector3.Lerp(_leftDoorStart, _leftDoorEnd, time / duration);
                _rightDoor.transform.localPosition = Vector3.Lerp(_rightDoorStart, _rightDoorEnd, time / duration);
                time += Time.deltaTime;
                yield return null;
            }

            _leftDoor.transform.localPosition = _leftDoorEnd;
            _rightDoor.transform.localPosition = _rightDoorEnd;
            _isOpen = false;
        }
        
    }
}
