using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Apis.Services;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System.IO;
using ExitGames.Client.Photon.StructWrapping;

public class CredentialsToDrive : MonoBehaviour
{
    static string spreadsheetID = "19otEzZVGU13MVzLzueVyNuFukGN9dOW0X1DZeCcYIdY";
    static string path = "/StreamingAssets/credentials.json";

    static SheetsService sheetsService;
    string _writeRange = "A1:B1";
    string _readRange = "Credentials!A1:B";
    private int credentialsCount;

    public static CredentialsToDrive Instance;

    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SetUpCredentials();
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

        credentialsCount = values.Count;

        SetWriteRange((credentialsCount + 1).ToString());
    }
    void SetWriteRange(string number)
    {
        _writeRange = $"A{number}:C{number}";
    }

    public void WriteCredentialsToDrive(string username, string password)
    {

        var valueRange = new Google.Apis.Sheets.v4.Data.ValueRange();
        var objectList = new List<System.Object> { username, password };


        valueRange.Values = new List<IList<object>> { objectList };
        var updateRequest = sheetsService.Spreadsheets.Values.Update(valueRange, spreadsheetID, _writeRange);
        updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
        var updateResponse = updateRequest.Execute();
        Debug.Log(updateResponse);
        credentialsCount++;

    }


   public bool CheckLogin(string inputUsername, string inputPassword)
    {
        // Retrieve the data from the specified range in the sheet
        ValueRange response = sheetsService.Spreadsheets.Values.Get(spreadsheetID, _readRange).Execute();
        var values = response.Values;

        // Check if the username and password match the ones in the sheet
        bool loginSuccess = false;
        foreach (var row in values)
        {
            string username = (string)row[0];
            string password = (string)row[1];
            if (username == inputUsername && password == inputPassword)
            {
                loginSuccess = true;
                break;
            }
        }

        return loginSuccess;
    }
}
