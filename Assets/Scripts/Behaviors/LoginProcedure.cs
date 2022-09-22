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
    public void Next()
    {
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
}
