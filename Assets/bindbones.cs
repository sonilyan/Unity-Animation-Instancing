using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Internal;
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

		for (int i = 0; i < skin_mesh.bones.Length; i++)
		{
			var boneTrans = skin_mesh.bones[i];
			var m1 = boneTrans.worldToLocalMatrix * root.localToWorldMatrix;
			var m2 = root.localToWorldMatrix * boneTrans.worldToLocalMatrix;
			var m3 = skin_mesh.sharedMesh.bindposes[i];

			if (m1 != m2)
			{
				Debug.Log("error!");
			}
			
			test.Add(m1);

			if (m1 != m3)
			{
				Debug.Log(m1);
				Debug.Log(m3);
			}
		}
		
		Debug.Log("done!");
	}

	// Update is called once per frame
	void Update () {
		Debug.Log("update");
	}
}
