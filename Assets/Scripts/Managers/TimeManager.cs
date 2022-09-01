using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public const int HoursInDay = 24, MinutesInHour = 60;

    public float DayDurationInMinutes = 24f;
    private float _dayDurationInSeconds;

    private float _totalTime = 0;
    private float _currentTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        _dayDurationInSeconds = DayDurationInMinutes * 60;
    }

    // Update is called once per frame
    void Update()
    {
        _totalTime += Time.deltaTime;
        _currentTime = _totalTime % _dayDurationInSeconds;
    }

    private float GetHour()
    {
        return _currentTime * HoursInDay / _dayDurationInSeconds;
    }    
    
    private float GetMinutes()
    {
        return (_currentTime * HoursInDay * MinutesInHour / _dayDurationInSeconds) % MinutesInHour;
    }

    public string Clock24Hours()
    {
        return Mathf.FloorToInt(GetHour()).ToString("00") + ":" + Mathf.FloorToInt(GetMinutes()).ToString("00");
    }

    public string Clock12Hours()
    {
        int hour = Mathf.FloorToInt(GetHour());
        string abbreviation = "AM";

        if (hour >= 12)
        {
            abbreviation = "PM";
            hour -= 12;
        }

        if (hour == 0)
        {
            hour = 12;
        }

        return hour.ToString("00") + ":" + Mathf.FloorToInt(GetMinutes()).ToString("00") + " " + abbreviation;
    }
}
