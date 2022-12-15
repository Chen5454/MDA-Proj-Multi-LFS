using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Google.Apis.Services;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System.IO;

public class RequestTest : MonoBehaviour
{
    static string spreadsheetID = "19otEzZVGU13MVzLzueVyNuFukGN9dOW0X1DZeCcYIdY";
    static string path = "/StreamingAssets/credentials.json";
    static SheetsService sheetsService;
    string _writeRange = "A1:C1";
    string _readRange = "PatientSheet!A1:C";
    public static RequestTest Instance;

    private int patientCount;

    List<string> strings; //all A1:A35 first lines in the 

    string currentRangeName;
    int rowsPerIterration = 10;
    void Start()
    {
      //  Debug.LogError("Google Sheets Master performs start - it's not really an error");
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SetUpCredentials();
        Invoke("GetRows",2);
       // GetRows("nehC");
        //Invoke("LogPlayerTersting", 2);
        //LogPlayer(); // not sure we should
    }
    void SetUpCredentials() //this must also be done async  
    {
        string fullPath = Application.dataPath + path;

        Stream creds = File.Open(fullPath, FileMode.Open);

        ServiceAccountCredential serviceAccountCredential = ServiceAccountCredential.FromServiceAccountData(creds);

        sheetsService = new SheetsService(new BaseClientService.Initializer() { HttpClientInitializer = serviceAccountCredential });

        var request = sheetsService.Spreadsheets.Values.Get(spreadsheetID, _readRange);
        var response = request.Execute();
        var values = response.Values;

        patientCount = values.Count;

        SetWriteRange((patientCount+1).ToString());



    }

    public void PrintCell() //just prints us the first(A1) that inside the cell
    {
        var request = sheetsService.Spreadsheets.Values.Get(spreadsheetID, _readRange);

        var response = request.Execute();
        var values = response.Values;

        if (values != null && values.Count >= 0)
        {
            foreach (var row in values)
            {
                // row 0 PatientData and row 1 names
                Debug.Log(row[0]);
            }

        }
        else
        {
            Debug.Log("No data");
        }
    }
    //public List<string> GetRows(string PatientName)
    //{
    //    var request = sheetsService.Spreadsheets.Values.Get(spreadsheetID, _readRange);

    //    var response = request.Execute(); //could not async?
    //    var values = response.Values;

    //    if (values != null && values.Count >= 0)
    //    {
    //        //return (List<string>)values;
    //        List<string> rowsFirstCells = new List<string>();
    //        foreach (var row in values)
    //        {
    //            if (PatientName == row[0].ToString())
    //            {
    //                rowsFirstCells.Add((string)row[0]);
    //                Debug.Log(rowsFirstCells[0]);
    //            }
    //            else
    //            {
    //                Debug.Log("There is no patient name");

    //            }
    //        }
    //        return rowsFirstCells;
    //    }

    //    else
    //    {
    //        Debug.Log("There is no patient name");
    //        return null;

    //    }
    //}

    public string GetRow(string PatientName)
    {
        var request = sheetsService.Spreadsheets.Values.Get(spreadsheetID, _readRange);

        var response = request.Execute(); //could not async?
        var values = response.Values;

        if (values != null && values.Count >= 0)
        {
            //return (List<string>)values;
            string rowsFirstCells="";
            foreach (var row in values)
            {
                if (PatientName == row[0].ToString())
                {
                    rowsFirstCells = row[0].ToString();
                    Debug.Log(rowsFirstCells[0]);
                }
                else
                {
                    Debug.Log("There is no patient name");

                }
            }
            return rowsFirstCells;
        }

        else
        {
            Debug.Log("There is no patient name");
            return null;

        }
    }

    public List<string> GetRows()
    {
        var request = sheetsService.Spreadsheets.Values.Get(spreadsheetID, _readRange);

        var response = request.Execute(); //could not async?
        var values = response.Values;

        if (values != null && values.Count >= 0)
        {
            //return (List<string>)values;
            List<string> rowsFirstCells = new List<string>();
            foreach (var row in values)
            {
              
                    rowsFirstCells.Add((string)row[0]);
                    Debug.Log(rowsFirstCells[0]);
            }
            return rowsFirstCells;
        }

        else
        {
            Debug.Log("There is no patient name");
            return null;

        }
    }

    public void LogPlayer(string patientName,string patientJson, string treatmentJson)
    {

        var valueRange = new Google.Apis.Sheets.v4.Data.ValueRange();
        var objectList = new List<System.Object>{patientName,patientJson,treatmentJson};


        valueRange.Values = new List<IList<object>> { objectList };// // List<System.object> Objects to save in order of coloumns
        var updateRequest = sheetsService.Spreadsheets.Values.Update(valueRange, spreadsheetID, _writeRange);
        updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
        //updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;

        // var updateResponse = updateRequest.ExecuteAsync();
        var updateResponse = updateRequest.Execute();
        Debug.Log(updateResponse);
        patientCount++;

    }


    void SetWriteRange(string number)
    {
        _writeRange = $"A{number}:C{number}";
    }


    void LogPlayerTersting()
    {
        LogPlayer("a", "b", "c");
    }

    /// <summary>
    /// UN COMMENT BELOW!
    /// </summary>

    //public void LogPlayer()
    //{

    //    var valueRange = new Google.Apis.Sheets.v4.Data.ValueRange();
    //    var objectList = new List<System.Object>(); //fill object list with things!

    //    //if(strings==null)
    //    //strings = GetRows("A1:A35"); //currently limited to 35 different players. Disregards any new name after the 35th, but will continue to update any of the first 35


    //    ///Solution to player limit
    //    ///loop that collects GetRows of a set size from A##:A[##+sizePerLoopItteration] and checks if the last memeber has any value
    //    ///if not, the loop ends
    //    ///if it does hold value, the loop goes again, collecting another [sizePerLoopItteration] of users, checking again if the last one holds any value

    //    //if (strings !=null && strings.Count >0 && strings.Contains(PlayerDataMaster.Instance.currentPlayerData.playerName)) 
    //    //{
    //    //    //True - the player is already logged:
    //    //    int i = strings.IndexOf(PlayerDataMaster.Instance.currentPlayerData.playerName);

    //    //    string newRange = "A" + (i+1);
    //    //objectList = new List<System.Object>() { PlayerDataMaster.Instance.currentPlayerData.playerName, PlayerDataMaster.Instance.currentPlayerData.lostMercs, PlayerDataMaster.Instance.currentPlayerData.numOfavailableMercs, PlayerDataMaster.Instance.currentPlayerData.gold};


    //    valueRange.Values = new List<IList<object>> { PlayerDataMaster.Instance.GetLog };

    //    var updateRequest = sheetsService.Spreadsheets.Values.Update(valueRange, spreadsheetID, currentRangeName);
    //    updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
    //    //var updateResponse = updateRequest.Execute();
    //    var updateResponse = updateRequest.ExecuteAsync();
    //    //}
    //    //else
    //    //{
    //    //    //objectList = new List<System.Object>() { PlayerDataMaster.Instance.currentPlayerData.playerName, PlayerDataMaster.Instance.currentPlayerData.deadMercs, PlayerDataMaster.Instance.currentPlayerData.numOfavailableMercs, PlayerDataMaster.Instance.currentPlayerData.gold };

    //    //    valueRange.Values = new List<IList<object>> { PlayerDataMaster.Instance.GetLog };
    //    //    //valueRange.Values = new List<IList<object>> { objectList };

    //    //    var appendRequest = sheetsService.Spreadsheets.Values.Append(valueRange, spreadsheetID, "A2"); //just adds under A
    //    //    appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
    //    //    //var appendResponse = appendRequest.Execute();
    //    //    var appendResponse = appendRequest.ExecuteAsync();
    //    //}
    //}
    //public void LogNewPlayer()
    //{
    //    var valueRange = new Google.Apis.Sheets.v4.Data.ValueRange();
    //    var objectList = new List<System.Object>(); //fill object list with things!


    //    valueRange.Values = new List<IList<object>> { PlayerDataMaster.Instance.GetLog };
    //    //valueRange.Values = new List<IList<object>> { objectList };

    //    var appendRequest = sheetsService.Spreadsheets.Values.Append(valueRange, spreadsheetID, "A2"); //just adds under A
    //    appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
    //    //var appendResponse = appendRequest.Execute();
    //    var appendResponse = appendRequest.ExecuteAsync();

    //}

    //bool FindRangeForName(string nameToSearch) //sets currentRangeName
    //{
    //    int start = 2;
    //    int end = start + rowsPerIterration - 1; //for a total of A2:A11(2+10-1) which is 10
    //    bool keepGoing = true;

    //    int runningtotal = 0;

    //    while (keepGoing)
    //    {
    //        List<string> s = GetRows("A" + start + ":A" + end);
    //        if (s == null) //string.NullOrEmpty()
    //        {
    //            currentRangeName = null;
    //            return false; //couldn't find name
    //        }

    //        if (s.Count > 0)
    //        {
    //            if (s.Contains(PlayerDataMaster.Instance.currentPlayerData.playerName))
    //            {
    //                int i = s.IndexOf(PlayerDataMaster.Instance.currentPlayerData.playerName);

    //                currentRangeName = "A" + (runningtotal + i + 2); //names start at line 2, i starts at 0
    //                keepGoing = false; //redundant
    //                break;
    //            }
    //            //if(s[s.Count-1] == "")
    //            if (s.Count() < rowsPerIterration)
    //            {
    //                //keepGoing = false; //redundant
    //                currentRangeName = null;
    //                return false;
    //                //currentRangeName = "A"+(runningtotal+1).ToString();
    //                //return false;
    //            }
    //        }

    //        runningtotal += s.Count;
    //        start = end + 1;
    //        end = start + rowsPerIterration - 1; //for a total of A12:A21(12+10-1) which is 10

    //    }
    //    return true;
    //}
}
