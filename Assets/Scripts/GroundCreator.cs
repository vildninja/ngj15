using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class GroundCreator : MonoBehaviour {

    public bool updateMesh;
    public bool export;

	// Use this for initialization
	void Start () {

        
	}
	
	// Update is called once per frame
	void Update () {
        if (updateMesh)
        {
            UpdateMesh();
            updateMesh = false;
        }
//#if UNITY_EDITOR
//        if (export)
//        {
//            export = false;
//            UnityEditor.AssetDatabase.CreateAsset(GetComponent<MeshFilter>().mesh, "Assets/" + name + ".asset");
//        }
//#endif
    }

    public void UpdateMesh()
    {
        var poly = GetComponent<PolygonCollider2D>();
        var tris = new Triangulator(poly.points);
        var indices = tris.Triangulate();

        Mesh mesh;
        if (export)
        {
            mesh = GetComponent<MeshFilter>().mesh;
        }
        else
        {
            mesh = new Mesh();
        }
        Vector3[] points3 = new Vector3[poly.points.Length];
        Vector3[] norms = new Vector3[poly.points.Length];
        for (int i = 0; i < poly.points.Length; i++)
        {
            points3[i] = poly.points[i];
            norms[i] = Vector3.back;
        }

        Vector2[] uv = new Vector2[poly.points.Length];
        for (int i = 0; i < poly.points.Length; i++)
            uv[i] = transform.TransformPoint(poly.points[i]);
        mesh.vertices = points3;
        mesh.uv = uv;
        mesh.normals = norms;
        mesh.triangles = indices;

        GetComponent<MeshFilter>().mesh = mesh;
    }
}
