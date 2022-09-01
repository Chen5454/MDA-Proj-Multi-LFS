using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DocumentationLogManager : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI UIDisplayer;
    public int LogsToDisplayAtOnce = 5;
    public bool InfiniteList;
    public bool DisplayAllLog;
    private string myLog;
    private string[] _queueArray;
    private List<string> _queueList = new List<string>();
    private int _queueIndex = 0;

   // private PhotonView _photonView;
    

    private void Awake()
    {
       // _photonView = GetComponent<PhotonView>();
        _queueArray = new string[LogsToDisplayAtOnce + 1];
        _queueList.Add("");
    }

    void Update()
    {
        if (!InfiniteList)
        {
            if (_queueArray[LogsToDisplayAtOnce] != null)
            {
                Dequeue();
            }

            RefreshText();
        }
        else
        {
            RefreshText();
        }
    }

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (DisplayAllLog)
        {
            myLog = logString;
            string newString =$"{PhotonNetwork.LocalPlayer.NickName} :  [{type}]:  {myLog} + \n----------------------------------------\n";
            Enqueue(newString);
            if (type == LogType.Exception)
            {
                newString = "\n" + stackTrace;
                Enqueue(newString);
            }
            myLog = string.Empty;
            if (!InfiniteList)
            {
                foreach (string mylog in _queueArray)
                {
                    myLog += mylog;
                }
            }
            else
            {
                foreach (string mylog in _queueList)
                {
                    myLog += mylog;
                }
            }
        }
    }

    public void LogThisText(int senderCrewIndex, string senderName, string text)
    {
        myLog = text;
        string newString = $"[{senderCrewIndex}] <{senderName}> {myLog} \n";
        Enqueue(newString);
        myLog = string.Empty;
        if (!InfiniteList)
        {
            foreach (string mylog in _queueArray)
            {
                myLog += mylog;
            }
        }
        else
        {
            foreach (string mylog in _queueList)
            {
                myLog += mylog;
            }
        }
    }

    void RefreshText()
    {
        string text = "";

        if (!InfiniteList)
        {
            for (int i = 0; i < _queueArray.Length; i++)
            {
                text += _queueArray[i];
            }
        }
        else
        {
            for (int i = 0; i < _queueList.Count; i++)
            {
                text += _queueList[i];
            }
        }

        UIDisplayer.text = text;
    }

    public void Enqueue(string word)
    {
        if (!InfiniteList)
        {
            if (_queueIndex < _queueArray.Length)
            {
                _queueArray[_queueIndex] = word;
                _queueIndex++;
            }
        }
        else
        {
            if (_queueIndex == 0)
            {
                _queueList[0] = word;
            }
            else
            {
                _queueList.Add(word);
            }
            _queueIndex++;
        }
    }

    public void Dequeue()
    {
        _queueArray[0] = null;
        if (_queueArray[0] == null)
        {
            for (int b = 0; b < _queueArray.Length; b++)
            {
                if (b == _queueArray.Length - 1)
                {
                    _queueArray[b] = null;
                }
                else
                {
                    _queueArray[b] = _queueArray[b + 1];
                }
            }
        }
        _queueIndex--;
    }


}
