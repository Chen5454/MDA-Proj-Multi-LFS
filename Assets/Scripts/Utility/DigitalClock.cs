using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DigitalClock : MonoBehaviour
{
    private TimeManager _timeManager;
    private TextMeshProUGUI _textMeshProUGUI;

    public bool Is24HoursClock = true;

    // Start is called before the first frame update
    void Start()
    {
        _timeManager = FindObjectOfType<TimeManager>();
        _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Is24HoursClock)
        {
            _textMeshProUGUI.text = _timeManager.Clock24Hours();
        }
        else
        {
            _textMeshProUGUI.text = _timeManager.Clock12Hours();
        }
    }
}
