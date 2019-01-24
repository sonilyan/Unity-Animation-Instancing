using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(anim_baker))]
public class anim_baker_editor : Editor
{
    private bool cp2 = false;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        cp2 = EditorGUILayout.Toggle("ClosestPowerOfTwo", cp2);
        if (GUILayout.Button("bake"))
        {
            anim_baker b = (anim_baker) target;

            Bake(b.GetComponent<Animation>(), b.GetComponentsInChildren<SkinnedMeshRenderer>(),b.rawMesh);
           
        }
    }

    private void Bake(Animation ani, SkinnedMeshRenderer[] smrs,Mesh rawMesh)
    {
        Vector3[] basedata = rawMesh.vertices;
        List<Vector3[]> data = new List<Vector3[]>();

        var indexs = rawMesh.GetIndices(0);
        Mesh mesh = new Mesh();

        var ass = ani.Cast<AnimationState>();
        foreach (var animationState in ass)
        {
            ani.Play(animationState.name);
            var perFrameTime = animationState.clip.length / animationState.clip.frameRate;
            var curTime = 0f;

            for (int i = 0; i < animationState.clip.frameRate; i++)
            {
                animationState.time = curTime;

                ani.Sample();

                curTime += perFrameTime;

                foreach (var smr in smrs)
                {
                    smr.BakeMesh(mesh);
                    data.Add(mesh.vertices);
                }
            }
            
            ani.Stop();
        }

        Debug.Log(basedata.Length + "-" + Mathf.ClosestPowerOfTwo(basedata.Length));
        Debug.Log(data.Count + "-" + Mathf.ClosestPowerOfTwo(data.Count));

        Texture2D t2d;
        if (cp2)
        {
            t2d = new Texture2D(
                Mathf.ClosestPowerOfTwo(basedata.Length),
                Mathf.ClosestPowerOfTwo(data.Count),
                TextureFormat.RGBA32, false);
        }
        else
        {
            t2d = new Texture2D(basedata.Length,data.Count,
                TextureFormat.RGBA32, false);
        }

        t2d.filterMode = FilterMode.Point;
        
        for (int frame = 0; frame < data.Count; frame++)
        {
            for (int vertex_index = 0; vertex_index < data[frame].Length; vertex_index++)
            {
                var tmp = data[frame][vertex_index] - basedata[vertex_index];
                
                var x = Mathf.Abs(tmp.x);
                var y = Mathf.Abs(tmp.y);
                var z = Mathf.Abs(tmp.z);
                
                float max = x > y ? x > z ? x : z : y > z ? y : z;

                tmp /= max;
                tmp += new Vector3(1, 1, 1);
                tmp /= 2;

                t2d.SetPixel(vertex_index, frame, new Color(tmp.x, tmp.y, tmp.z, max));
            }
        }

        t2d.Apply();

        AssetDatabase.CreateAsset(t2d, "Assets/AniTexture/test.asset");
    }
}
