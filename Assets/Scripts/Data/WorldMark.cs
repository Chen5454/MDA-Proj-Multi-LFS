using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine;


public class WorldMark : MonoBehaviour, IPunInstantiateMagicCallback
{

    [SerializeField] private List<Sprite> _marks;
    public List<Sprite> Marks => _marks;
    public string nameID;

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] instantiationData = info.photonView.InstantiationData;
        Vector2 targetPos = (Vector2)instantiationData[0];
        string IndexRandom = (string)instantiationData[1];
        int markIndex = (int) instantiationData[2];

        ChangeColorForArea(markIndex, this.gameObject);
        info.photonView.gameObject.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = info.photonView.gameObject.GetComponent<WorldMark>().Marks[markIndex];
        info.photonView.gameObject.GetComponent<WorldMark>().nameID = IndexRandom;


    }

    private void ChangeColorForArea(int markIndex, GameObject worldMark)
    {
        var colorArea = worldMark.transform.GetComponentInChildren<Renderer>().material;
        var ColorFillArea = worldMark.transform.Find("FillArea").GetComponentInChildren<Renderer>().material;

        switch (markIndex)
        {
            case 0:
                colorArea.color = Color.red;
                ColorFillArea.color = new Color(1, 0, 0, 0.2f);
                break;
            case 1:
                colorArea.color = Color.green;
                ColorFillArea.color = new Color(0, 1, 0, 0.2f);
                break;
            case 2:
                colorArea.color = Color.blue;
                ColorFillArea.color = new Color(0, 0, 1, 0.2f);
                break;
            case 3:
                colorArea.color = Color.white;
                ColorFillArea.color = new Color(1, 1, 1, 0.2f);
                break;
            case 4:
                colorArea.color = Color.black;
                ColorFillArea.color = new Color(0, 0, 0, 0.2f);
                break;
            case 5:
                colorArea.color = Color.yellow;
                ColorFillArea.color = new Color(1, 1, 0, 0.2f);
                break;
        }
    }

}
