using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmisiionFlash : MonoBehaviour {




	void Update () {
		Renderer renderer = GetComponent<Renderer> ();
		Material mat = renderer.material;

		float emission = Mathf.PingPong (Time.time, 0.5f);
		Color baseColor = Color.red; //Replace this with whatever you want for your base color at emission level '1'

		Color finalColor = baseColor * Mathf.LinearToGammaSpace (emission);

		mat.SetColor ("_EmissionColor", finalColor);
	}
}
