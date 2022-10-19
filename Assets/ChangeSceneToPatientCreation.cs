using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSceneToPatientCreation : MonoBehaviour
{
    public void LoadSceneByIndex(int sceneIndex)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex); //better to load by name like "Lobby 2" or "PatientCreationScene"? I think not...
    }
}
