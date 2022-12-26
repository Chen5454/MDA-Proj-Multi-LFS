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

   public const string credentialsJsonString = @"{
  ""type"": ""service_account"",
  ""project_id"": ""mda-patient-master"",
  ""private_key_id"": ""fd8843857c7c1c99953d3ec9b8aa06c5ca5c5d51"",
  ""private_key"": ""-----BEGIN PRIVATE KEY-----\nMIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQDDrfUdfVtbFMBu\n0A2AKZteOGOxUipjELnuggbunvl10+XnZJadUOixfNA8hUXGEa3SJJ8HSh5S68xC\nwjVETg5sJ53EDTswmoR8YiZi/1YtywlVQm5eQ9lMpJLRcH11W8SSDFYIsvIJPqQd\n/BshigdBAyjrfjbPLFzmC5L6EbW+65ELW1pDpclqIVKuUKU4lEcMWLIvc56y7obY\n/JHZn4yhpGRl8aqpqqLi0SK44vlVS+PKM8oMyw+1M0kpGxz04IKgwspWFeN8C6Oq\nYDVIJRRlLuwZf05tx9uKQL3dIUpxWCqHZSBT3+IH0/NKcoA1RF4wLp62Wc6I6Xo4\numI14i95AgMBAAECggEAFGg4M6ze4esSXYwjmE3ehu89EGFYC9BS/7l+RRT9wajJ\nZX4jSgFF3zq70c/5VJyD4vJQRneXnYC7Yuvzl63HScbA6JETrvAyaHFo/PKRl4GA\n9ewRbBHJ3/ka2a5QWzSola2lLE+TN90ZSwo/fxowwvo0IokdaLJA9vnWiG9f46rp\nUlsEyE9XykN2GT0ytnLfQYIKTFjKiqRdBFi7Fp0pUUEAWT8dJePlZdCMgLLLFlJ+\nmDoPI3Onkvjp8uYyYNkS571IKyAxEyey7AgCJ10rfAJJkKNDOlHjZIPt5aFfNweJ\nWlk/3PTgK+iUwzSqX2bAG0teX8bjpWKmJvWt91sYHQKBgQDpV770BHwGxLGZDPUe\ngLSX5mtyuQA/f9dT68mj5MI3w8fPeau3q+QmurqgElgIH5cnY7MZDO95gIVOuGxo\ntBZ0kprGTmnV8l7pJ96rIX/sfpz9bPEj8ohGcLheEZH6BM73RD6AXZX3MjSbTfrq\nk89ddEsa1FwXA3M9MzfpEWaYvQKBgQDWrgIHtxA2OvphuUfTjZLkdh4Y6mtcTJxg\nOwzzsWqUT3F+AAq3jvOns0QJAQsROuP64KPK2lDz+wkqwW5t30tEdQ8G47EaFPF8\nIM45ITrj99h8k9wbWKT1IVlQgGxO64njYhESEB/LLH3JUPXc//srMDB/uqcc6bEG\nCrtOOfOzbQKBgHmhHE7zu3aHto8xut9QdVIscFuXZTK0P5hl8nA0wKvyqEdUg5a0\nybMKaFch2LM7TD3i8Ssgm/84Z8RVy2R0YrQW6whtPSuhL6nW93E6gATVJp3O2I9F\nT5VwEhoujGnHcd5fuziMD6yPhe5iImV98HwlLzTn9qykRsBi30Tkphb9AoGAXek+\n8BsP13lTYH6JwiyzboeRQPsDPhwxaBNM5indYsWh8ymXhpbdVaYhBANV0RK++ldc\nr3dzZFeTugVmBmBg77B5g10Fc1BoFxHOxMUZmvIn3M9dwQS1HJqbFJEpUBjPeA3F\nQ+6TyfBLkk59T1EXkdwBy05enlJIY0k8iMrnJb0CgYAqpw+Ca1VlExALscf99H4E\nPV22kxvRsYNm35n7fFoBX4BKjXzyF4vOk68oZ1jFk76JCSgfJ+TZXmS2b1jpoOTL\n+PtD3Idz50omDL48jCek+Y8C69barHR2up8kOt9JVEVbZEOwJg2u3JlmlayUK1Q3\nqZqCMhbmtWMZCJqIm1+inA==\n-----END PRIVATE KEY-----\n"",
  ""client_email"": ""serviceaccount1@mda-patient-master.iam.gserviceaccount.com"",
  ""client_id"": ""112812890842171375445"",
  ""auth_uri"": ""https://accounts.google.com/o/oauth2/auth"",
  ""token_uri"": ""https://oauth2.googleapis.com/token"",
  ""auth_provider_x509_cert_url"": ""https://www.googleapis.com/oauth2/v1/certs"",
  ""client_x509_cert_url"": ""https://www.googleapis.com/robot/v1/metadata/x509/serviceaccount1%40mda-patient-master.iam.gserviceaccount.com""
}";

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
        //old method with local files
        //string fullPath = Application.dataPath + path;

        //Stream creds = File.Open(fullPath, FileMode.Open);
        Stream creds = GenerateStreamFromString(credentialsJsonString);
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

    public static Stream GenerateStreamFromString(string s)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }
}
