using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginProcedure : MonoBehaviour
{
    [SerializeField] private float _lerpDuration;
    [SerializeField] private TMP_InputField _userName;
    [SerializeField] private TextMeshProUGUI _chosenUserName;
    [SerializeField] private Image _chosenAvater;
    [SerializeField] private int _avaterIndex, _halfScreenWidth = 960;
    private Image _avatarPicked;

    private const int _scrollDistance = 640;
    private Vector3 _desiredPosition = Vector3.zero;

    private void Start()
    {
        _desiredPosition = new Vector3(transform.position.x - _scrollDistance, transform.position.y, transform.position.z);
    }
    private IEnumerator LerpPosition(Vector3 desiredPos, float duration)
    {
        float time = 0;
        Vector3 startPos = transform.position;

        while (time < _lerpDuration)
        {
            transform.position = Vector3.Lerp(startPos, desiredPos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = desiredPos;

        _desiredPosition = new Vector3(transform.position.x - _scrollDistance, transform.position.y, transform.position.z);
    }
    public void Next(bool isForward)
    {
        int direction = isForward ? -1 : 1;
        _desiredPosition = new Vector3(transform.position.x + direction * _scrollDistance, transform.position.y, transform.position.z);

        if (direction == -1)
            StartCoroutine(LerpPosition(_desiredPosition, _lerpDuration));
        else if (transform.position.x < _halfScreenWidth)
            StartCoroutine(LerpPosition(_desiredPosition, _lerpDuration));
    }
    public void KeepUserName()
    {
        _chosenUserName.text = _userName.text;
    }
    public void PickAvatar(Image avatarImage)
    {
        _avatarPicked = avatarImage;
        _chosenAvater.sprite = _avatarPicked.sprite;
    }
    //public void PickAvatar(int avatarIndex) // Moved to "PhotonRoom" Script (Needed Script that stay from lobby to actual game. )
    //{
    //    //_avatarPicked = avatarImage;
    //    _avaterIndex = avatarIndex;
    //    //_chosenAvater.sprite = _avatarPicked.sprite;
    //}
}
