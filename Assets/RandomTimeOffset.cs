using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTimeOffset : MonoBehaviour {

	// Use this for initialization
	void Start () {
		MaterialPropertyBlock props = new MaterialPropertyBlock();
		MeshRenderer renderer;

		foreach (var mr in GetComponentsInChildren<MeshRenderer>())
		{
			float r = Random.Range(0.0f, 1.0f);
			props.SetFloat("_TimeOffset", r);
   
			mr.SetPropertyBlock(props);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
