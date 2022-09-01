using UnityEngine;
using System.Collections;

public class Damage : MonoBehaviour 
{
	public float maxMoveDelta = 1.0f; 
	public float maxCollisionStrength = 50.0f;
	public float YforceDamp = 0.1f; 
	public float demolutionRange = 0.5f;
	public float impactDirManipulator = 0.0f;
	public MeshFilter[] optionalMeshList;
	//public AudioSource clickAudio;
	
	private MeshFilter[] meshfilters;
	private float sqrDemRange;

	public void Start()
	{
     
       
        
        if(optionalMeshList.Length>0)
        	meshfilters = optionalMeshList;
        else
        	meshfilters = GetComponentsInChildren<MeshFilter>();
        	
        sqrDemRange = demolutionRange*demolutionRange;
        	
 
	}
	
	public void OnCollisionEnter( Collision collision ) 
	{
		//clickAudio.Play ();
		Vector3 colRelVel = collision.relativeVelocity;
		colRelVel.y *= YforceDamp;
		
		Vector3 colPointToMe = transform.position - collision.contacts[0].point;

	
		float colStrength = colRelVel.magnitude * Vector3.Dot(collision.contacts[0].normal, colPointToMe.normalized);

		OnMeshForce( collision.contacts[0].point, Mathf.Clamp01(colStrength/maxCollisionStrength) );
	
	}
	

	public void OnMeshForce( Vector4 originPosAndForce )
	{	
        OnMeshForce( (Vector3)originPosAndForce, originPosAndForce.w );
	}

	public void OnMeshForce( Vector3 originPos, float force )
	{
       
        force = Mathf.Clamp01(force);

        for(int j=0; j<meshfilters.Length; ++j)
        {
        	Vector3 [] verts = meshfilters[j].mesh.vertices;

        	for (int i=0;i<verts.Length;++i)
			{	
        		Vector3 scaledVert = Vector3.Scale( verts[i], transform.localScale );						
        		Vector3 vertWorldPos = meshfilters[j].transform.position + (meshfilters[j].transform.rotation * scaledVert);
                Vector3 originToMeDir = vertWorldPos - originPos;
                Vector3 flatVertToCenterDir = transform.position - vertWorldPos;
                flatVertToCenterDir.y = 0.0f;
                   
      
                if( originToMeDir.sqrMagnitude < sqrDemRange ) //dot > 0.8f )
                {	

                	float dist = Mathf.Clamp01(originToMeDir.sqrMagnitude/sqrDemRange);
                    float moveDelta = force * (1.0f-dist) * maxMoveDelta;

					Vector3 moveDir = Vector3.Slerp(originToMeDir, flatVertToCenterDir, impactDirManipulator).normalized * moveDelta;

                    verts[i] += Quaternion.Inverse(transform.rotation)*moveDir;


                }

        	}
       
        	meshfilters[j].mesh.vertices = verts;
        	meshfilters[j].mesh.RecalculateBounds();
        }
	} 
}
