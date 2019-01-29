using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimBonesTextureReader : MonoBehaviour
{
	public Texture2D t2d;
	// Use this for initialization
	void Start () {
		for (int i = 0; i < 10; i++)
		{
			var r1 = t2d.GetPixel(0, i);
			var r2 = t2d.GetPixel(1, i);
			var r3 = t2d.GetPixel(2, i);
			var r4 = t2d.GetPixel(3, i);

			//Matrix4x4 tmp = new Matrix4x4(r1,r2,r3,r4);//TODO:参数是列而不是行
			Matrix4x4 tmp = new Matrix4x4();
			tmp.SetRow(0,r1);
			tmp.SetRow(1,r2);
			tmp.SetRow(2,r3);
			tmp.SetRow(3,r4);
			
			Debug.Log(tmp);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
