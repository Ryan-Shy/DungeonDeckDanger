using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AOEMeshBuilder
{
	public static int polyCircleCount = 100;

    public static Mesh BuildMesh(AoeType aoeType)
	{
		switch (aoeType)
		{
			case AoeType.Line:
				return BuildLineMesh();
			case AoeType.HalveCircleFront:
				return BuildCircleSegmentMesh(0.5f);
			case AoeType.QuarterCircleFront:
				return BuildCircleSegmentMesh(0.25f);
			case AoeType.Circle:
			default:
				return BuildCircleMesh();
		}

	}

	private static Mesh BuildCircleMesh()
	{
		Mesh mesh = new();
		// create vertices
		List<Vector3> vertices = new();
		vertices.Add(Vector3.zero); //origin
		vertices.Add(Vector3.up); // center of cylinder top
		for(int i = 0; i < polyCircleCount; i++)
		{
			float angle = (2*Mathf.PI*i)/polyCircleCount;// angle in radiants
			Vector3 lower = Vector3.zero;
			lower.x = Mathf.Cos(angle);
			lower.z = Mathf.Sin(angle);
			Vector3 upper = lower + Vector3.up;
			vertices.Add(lower);
			vertices.Add(upper);
		}
		// vertices.Count should be 2 * polyCircleCount + 2
		// last index is 2*polyCircleCount + 1
		mesh.vertices = vertices.ToArray();
		// create triangles
		List<int> tris = new();
		// top tris
		for(int i = 1; i < polyCircleCount; i++)
		{
			tris.Add(1);
			tris.Add(2 * (i + 1) + 1);
			tris.Add(2 * i + 1);
		}
		tris.Add(1);
		tris.Add(3);
		tris.Add(2 * polyCircleCount + 1);
		// bottom tris
		for (int i = 1; i < polyCircleCount; i++)
		{
			tris.Add(0);
			tris.Add(2 * i);
			tris.Add(2 * (i + 1));
		}
		tris.Add(0);
		tris.Add(2 * polyCircleCount);
		tris.Add(2);
		// mantel
		//
		//	2*n+1	3		5		7
		//	
		//	2*n		2		4		6
		//
		tris.Add(2 * polyCircleCount);
		tris.Add(2 * polyCircleCount + 1);
		tris.Add(3);
		tris.Add(2 * polyCircleCount);
		tris.Add(3);
		tris.Add(2);
		for (int i = 1; i < polyCircleCount; i++)
		{
			// top right of vertical
			tris.Add(2 * i);
			tris.Add(2 * i + 1);
			tris.Add(2 * (i + 1) + 1);
			// bottom right of vertical
			tris.Add(2 * i);
			tris.Add(2 * (i + 1) + 1);
			tris.Add(2 * (i + 1));
		}
		mesh.triangles = tris.ToArray();
		return mesh;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="fullness">value between 0 and 1 indicating how large the segment base is</param>
	/// <returns>a prism with circle segment base</returns>
	private static Mesh BuildCircleSegmentMesh(float fullness)
	{
		// if fullness doesn't make sense, just build circle
		if(fullness >= 1 || fullness <= 0)
		{
			return BuildCircleMesh();
		}
		Mesh mesh = new();
		// create vertices
		List<Vector3> vertices = new();
		vertices.Add(Vector3.zero); //origin
		vertices.Add(Vector3.up); // center of cylinder top
		for (int i = 0; i <= polyCircleCount; i++)
		{
			float angle = (2 * Mathf.PI * i * fullness) / polyCircleCount; // angle in radiants
			angle -= Mathf.PI * fullness; // change angle to make shape symetric around x-y plane
			angle += Mathf.PI / 2; // rotate to make symetric around y-z plane (forward direction)
			Vector3 lower = Vector3.zero;
			lower.x = Mathf.Cos(angle);
			lower.z = Mathf.Sin(angle);
			Vector3 upper = lower + Vector3.up;
			vertices.Add(lower);
			vertices.Add(upper);
		}
		// vertices.Count should be 2*polyCircleCount + 4
		// last index is 2*polyCircleCount + 3
		mesh.vertices = vertices.ToArray();
		// create triangles
		List<int> tris = new();
		// top tris
		for (int i = 1; i <= polyCircleCount; i++)
		{
			tris.Add(1);
			tris.Add(2 * (i + 1) + 1);
			tris.Add(2 * i + 1);
		}
		// bottom tris
		for (int i = 1; i <= polyCircleCount; i++)
		{
			tris.Add(0);
			tris.Add(2 * i);
			tris.Add(2 * (i + 1));
		}
		// mantel
		//
		//	2*n+1	2*n+3	1		3		5		7
		//	
		//	2*n		2*n+2	0		2		4		6
		//
		tris.Add(2 * polyCircleCount + 2);
		tris.Add(2 * polyCircleCount + 3);
		tris.Add(1);
		tris.Add(2 * polyCircleCount + 2);
		tris.Add(1);
		tris.Add(0);
		for (int i = 0; i <= polyCircleCount; i++)
		{
			// top right of vertical
			tris.Add(2 * i);
			tris.Add(2 * i + 1);
			tris.Add(2 * (i + 1) + 1);
			// bottom right of vertical
			tris.Add(2 * i);
			tris.Add(2 * (i + 1) + 1);
			tris.Add(2 * (i + 1));
		}
		mesh.triangles = tris.ToArray();
		return mesh;
	}

	private static Mesh BuildLineMesh()
	{
		Mesh mesh = new();
		// create vertices
		List<Vector3> vertices = new();
		/* wrong orientation lol
		vertices.Add(new Vector3(0, 0, -0.5f));
		vertices.Add(new Vector3(0, 0, 0.5f));
		vertices.Add(new Vector3(0, 1, -0.5f));
		vertices.Add(new Vector3(0, 1, 0.5f));
		vertices.Add(new Vector3(1, 0, -0.5f));
		vertices.Add(new Vector3(1, 0, 0.5f));
		vertices.Add(new Vector3(1, 1, -0.5f));
		vertices.Add(new Vector3(1, 1, 0.5f));
		*/
		vertices.Add(new Vector3(-0.5f, 0, 0));
		vertices.Add(new Vector3(0.5f, 0, 0));
		vertices.Add(new Vector3(-0.5f, 1, 0));
		vertices.Add(new Vector3(0.5f, 1, 0));
		vertices.Add(new Vector3(-0.5f, 0, 1));
		vertices.Add(new Vector3(0.5f, 0, 1));
		vertices.Add(new Vector3(-0.5f, 1, 1));
		vertices.Add(new Vector3(0.5f, 1, 1));
		mesh.vertices = vertices.ToArray();
		// create triangles
		int[] triangles =
		{
			//back
			0,2,1,
			1,2,3,
			//front
			4,5,6,
			6,5,7,
			//left
			0,4,2,
			4,6,2,
			//right
			1,3,5,
			5,3,7,
			//bottom
			0,1,4,
			4,1,5,
			//top
			2,6,3,
			3,6,7
		};
		mesh.triangles = triangles;
		return mesh;
	}

}
