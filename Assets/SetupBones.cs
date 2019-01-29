using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupBones : MonoBehaviour {

	List<int> meshs = new List<int>();
	// Use this for initialization
	private void Awake()
	{
		var smrs = GetComponentsInChildren<MeshFilter>();

		foreach (var smr in smrs)
		{
			var code = smr.sharedMesh.GetHashCode();
			if(meshs.Contains(code))
				continue;
			meshs.Add(code);
			
			List<Vector4> uvs = new List<Vector4>(smr.sharedMesh.vertexCount);
			Color[] colors = new Color[smr.sharedMesh.vertexCount];
			
			var bws = smr.sharedMesh.boneWeights;

			for (int i = 0; i < bws.Length; i++)
			{
				var bw = bws[i];
				colors[i] = new Color(bw.boneIndex0,bw.boneIndex1,bw.boneIndex2,bw.boneIndex3);
				uvs.Add(new Vector4(bw.weight0,bw.weight1,bw.weight2,bw.weight3));
			}
			
			smr.sharedMesh.colors = colors;
			smr.sharedMesh.SetUVs(1,uvs);
			smr.sharedMesh.SetUVs(2,uvs);
			smr.sharedMesh.SetUVs(3,uvs);
			
			smr.sharedMesh.UploadMeshData(false);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
