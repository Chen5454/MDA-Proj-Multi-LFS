using UnityEngine;
using System.Collections;

public class BreakObject : MonoBehaviour {

	public float health;
	public GameObject destroyed_ob;

	// Update is called once per frame
	private void OnCollisionEnter(Collision collision) 
	{
		//if (Input.GetButtonDown ("q"))
		if(health > 0)
		{
			this.transform.SendMessageUpwards("GetBulletDamage", 40, SendMessageOptions.DontRequireReceiver);
		}

	}

	public void GetBulletDamage(float damage)
	{
		health -= damage;
		if (health <= 0)
		{
			GameObject destroy_ob = Instantiate (destroyed_ob, this.transform.position, this.transform.rotation) as GameObject;
			Destroy(gameObject);

		}
	}
}