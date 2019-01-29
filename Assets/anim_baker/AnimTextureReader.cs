using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimTextureReader : MonoBehaviour
{

	public Texture2D t2d;
	// Use this for initialization
	void Start () {
		for (int i = 0; i < 10; i++)
		{
			var tmp = t2d.GetPixel(0, i);
			Debug.Log(tmp.r+" "+tmp.g+" "+tmp.b + " max="+tmp.a);
			
			float x = tmp.r * 2 - 1;
			float y = tmp.g * 2 - 1;
			float z = tmp.b * 2 - 1;
			x *= tmp.a;
			y *= tmp.a;
			z *= tmp.a;
			Debug.Log(x+" "+y+" "+z + " max="+tmp.a);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
