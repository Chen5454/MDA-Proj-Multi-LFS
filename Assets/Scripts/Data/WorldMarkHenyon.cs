using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;


public class WorldMarkHenyon : MonoBehaviour, IPunInstantiateMagicCallback
{
    [SerializeField] private List<Sprite> _marks;
    public List<Sprite> Marks => _marks;
    public string nameID;

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] instantiationData = info.photonView.InstantiationData;
        Vector2 targetPos = (Vector2)instantiationData[0];
        string IndexRandom = (string)instantiationData[1];


        ChangeColorForArea(this.gameObject);
        info.photonView.gameObject.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = info.photonView.gameObject.GetComponent<WorldMarkHenyon>().Marks[2];
        info.photonView.gameObject.GetComponent<WorldMarkHenyon>().nameID = IndexRandom;


    }

    private void ChangeColorForArea(GameObject worldMark)
    {
        var colorArea = worldMark.transform.GetComponentInChildren<Renderer>().material;
        var ColorFillArea = worldMark.transform.Find("FillArea").GetComponentInChildren<Renderer>().material;
        colorArea.color = Color.blue;
        ColorFillArea.color = new Color(0, 0, 1, 0.2f);
    }
}
