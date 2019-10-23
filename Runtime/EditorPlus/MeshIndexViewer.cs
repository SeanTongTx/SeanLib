using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeanLib.Core;

[ExecuteInEditMode]
public class MeshIndexViewer : MonoBehaviour
{
    private MeshFilter _meshFilter;
    private Mesh _mesh;
    SkinnedMeshRenderer skinRender;
    [HideInInspector]
    public List<Vector3> verticesList = new List<Vector3>();
    [HideInInspector]
    public List<Vector2> uvList = new List<Vector2>();
    [HideInInspector]
    public List<int> triList = new List<int>();
    [InspectorPlus.ReadOnly]
    public int index;
    public int count=100;
    [InspectorPlus.Button(Editor =true)]
    public void AddIndex()
    {
        index = index+1;
    }
    [InspectorPlus.Button(Editor = true)]
    public void DecIndex()
    {

        index =Mathf.Max(0, index -1);
    }
    private void Start()
    {
        _meshFilter = this.GetComponent<MeshFilter>();
        if (!_meshFilter)
        {
            skinRender = GetComponent<SkinnedMeshRenderer>();
            _mesh = skinRender.sharedMesh;
        }
        else
        {
            _mesh = _meshFilter.mesh;
        }

        ReadMeshInfo();

    }
    private void ReadMeshInfo()
    {

        for (int i = 0, imax = _mesh.vertexCount; i < imax; ++i)
        {
            verticesList.Add(_mesh.vertices[i]);
            uvList.Add(_mesh.uv[i]);
        }

        for (int i = 0, imax = _mesh.triangles.Length; i < imax; ++i)
        {
            triList.Add(_mesh.triangles[i]);
        }
    }
}
