using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bindbones : MonoBehaviour
{
	private Animator an;

	private SkinnedMeshRenderer skin_mesh;
	
	// Use this for initialization
	void Start ()
	{
		 an = GetComponent<Animator>();
		 skin_mesh = GetComponentInChildren<SkinnedMeshRenderer>();

		var bb = skin_mesh.sharedMesh.bindposes;
		var cc = skin_mesh.sharedMesh.boneWeights;
		
		Debug.Log("test");

		__Test();
	}

	private void __Test()
	{
		List<Matrix4x4> test = new List<Matrix4x4>();
			
		var root = skin_mesh.rootBone;
		while (root.parent != null)
		{
			root = root.parent;
		}
		
		Debug.Log(skin_mesh.sharedMesh.bindposes[0]);

		Matrix4x4 m1 = skin_mesh.bones[0].worldToLocalMatrix * root.localToWorldMatrix;
		
		Debug.Log(m1);

		Matrix4x4 m2 = root.localToWorldMatrix * skin_mesh.bones[0].worldToLocalMatrix;
		
		Debug.Log(m2);
		
		var x = root.worldToLocalMatrix * skin_mesh.bones[0].localToWorldMatrix * skin_mesh.sharedMesh.bindposes[0];
	}

	// Update is called once per frame
	void Update () {
	}
}
