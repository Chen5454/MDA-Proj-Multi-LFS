using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MakeItAButton : MonoBehaviour
{
    [SerializeField] private bool _disabledWhileUsed = false;
    private UnityEvent _tempEventToCall;

    public UnityEvent EventToCall = new UnityEvent();

    public void ClearAndCache()
    {
        if (_disabledWhileUsed)
        {
            _tempEventToCall = EventToCall;

            MakeItAButton mIAB = gameObject.AddComponent<MakeItAButton>();
            mIAB._tempEventToCall = _tempEventToCall;
            mIAB.EventToCall.AddListener(delegate { AddActionBackOnClick(); });
            Destroy(this);
        }
    }

    public void AddActionBackOnClick()
    {
        EventToCall = _tempEventToCall;
    }
}
