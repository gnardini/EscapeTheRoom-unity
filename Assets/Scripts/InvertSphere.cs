using UnityEngine;
using System.Collections;
using System.Linq;

public class InvertSphere : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.triangles = mesh.triangles.Reverse().ToArray();
	}
	
}
