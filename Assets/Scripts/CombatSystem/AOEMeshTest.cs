using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEMeshTest : MonoBehaviour
{
	public AoeType type;
	public bool changeFilter;
	public bool changeCollider;
	public bool extendTime;

    // Start is called before the first frame update
    void Start()
    {
		Mesh mesh = AOEMeshBuilder.BuildMesh(type);
		if(changeFilter && TryGetComponent<MeshFilter>(out MeshFilter filter))
		{
			Debug.Log("Set Mesh of MeshFilter");
			filter.mesh = mesh;
		}
		if(changeCollider && TryGetComponent<MeshCollider>(out MeshCollider collider))
		{
			Debug.Log("Set Mesh of MeshCollider");
			collider.sharedMesh = mesh;
		}
		if (extendTime)
		{
			transform.GetChild(0).GetComponent<Renderer>().material.SetFloat("_Expand_Time", 20);
			transform.GetChild(0).GetComponent<Renderer>().material.SetFloat("_Reset_Time", 20 * 1.1f);
		}
    }

	private void OnTriggerEnter(Collider other)
	{
		Debug.Log("Object entered: '"+other.gameObject.name+"'");
	}

	private void OnTriggerExit(Collider other)
	{
		Debug.Log("Object exited: '" + other.gameObject.name + "'");
	}
}
