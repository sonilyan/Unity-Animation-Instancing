using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(anim_baker))]
public class anim_baker_editor : Editor
{
    private bool cp2 = false;

    public override void OnInspectorGUI()
    {
        anim_baker b = (anim_baker) target;
        base.OnInspectorGUI();
        cp2 = EditorGUILayout.Toggle("ClosestPowerOfTwo", cp2);
        if (GUILayout.Button("bake"))
        {

            Bake(b.GetComponent<Animation>(), b.GetComponentsInChildren<SkinnedMeshRenderer>(), b.rawMesh);

        }

        if (GUILayout.Button("Bake bones"))
        {
            Bake_Bones(b.GetComponent<Animation>(), b.GetComponentsInChildren<SkinnedMeshRenderer>(), b.rawMesh);
        }
    }

    private void Bake_Bones(Animation ani, SkinnedMeshRenderer[] smrs, Mesh rawMesh)
    {
        List<Matrix4x4[]> data = new List<Matrix4x4[]>();
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
                    var root = smr.rootBone;
                    while (root.parent!=null)
                    {
                        root = root.parent;
                    }

                    Matrix4x4[] tmp = new Matrix4x4[smr.bones.Length];
                    for (int j = 0; j < smr.bones.Length; j++)
                    {
                        if (true)
                        {
                            //bindpose = boneTran.world2local  * root.local2world
                            //把root空间下的vertex变为骨骼空间下
                            //因为骨骼对顶点的约束不变，所以再*bones[j].local2world，使顶点变为
                            //相对于当前骨骼位置下的原约束位置的世界坐标
                            //然后再变为root下的相对位置
                            tmp[j] = root.worldToLocalMatrix * smr.bones[j].localToWorldMatrix *
                                     smr.sharedMesh.bindposes[j];
                        }
                        else
                        {
                            tmp[j] = root.localToWorldMatrix * smr.bones[j].worldToLocalMatrix;
                        }
                    }
                    
                    data.Add(tmp);
                }
            }

            ani.Stop();
        }

        Texture2D t2d = new Texture2D(smrs[0].bones.Length * 4, data.Count,
            TextureFormat.RGBAHalf, false);

        t2d.filterMode = FilterMode.Point;

        for (int f = 0; f < data.Count; f++)
        {
            var mat_d = data[f];
            for (int i = 0; i < mat_d.Length; i++)
            {
                var tmp = mat_d[i];
                t2d.SetPixel(i * 4, f, tmp.GetRow(0));
                t2d.SetPixel(i * 4 + 1, f, tmp.GetRow(1));
                t2d.SetPixel(i * 4 + 2, f, tmp.GetRow(2));
                t2d.SetPixel(i * 4 + 3, f, tmp.GetRow(3));
            }
        }

        t2d.Apply();

        AssetDatabase.CreateAsset(t2d, "Assets/AniTexture/test.bones.asset");
    }

    private void Bake(Animation ani, SkinnedMeshRenderer[] smrs, Mesh rawMesh)
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
            t2d = new Texture2D(basedata.Length, data.Count,
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

                Debug.Log(tmp.x + " " + tmp.y + " " + tmp.z + " max=" + max);
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
