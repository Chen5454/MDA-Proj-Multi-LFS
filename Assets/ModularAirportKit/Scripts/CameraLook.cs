using System.Collections;
using UnityEngine;
using Photon.Pun;


public class CameraLook : MonoBehaviour 
{
	//    public Camera  cameraToLookAt for mirrors;
	private Transform target;

	void Start ()
	{
		foreach (PhotonView photonView in ActionsManager.Instance.AllPlayersPhotonViews)
		{
			if (photonView.IsMine)
			{
				target = photonView.gameObject.GetComponent<CameraController>()/*.PlayerCamera*/.transform;
			}
		}
	}

	void Update () 

	{

		Vector3  v = target.transform.position  - transform.position ;

		v.x = v.z = 0.0f;

		transform.LookAt (target.transform.position  - v); 

	}

}