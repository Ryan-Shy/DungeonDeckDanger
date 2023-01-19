using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Connector : MonoBehaviour
{
    public Vector2 size = Vector2.one * 4f;
    public bool isConnected;
    private bool isPlaying;

    private void Start()
    {
        isPlaying = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isConnected ? Color.green : Color.red;
        if (!isPlaying)
        {
            Gizmos.color = Color.cyan;
        }
        
        Vector2 halfsize = size * 0.5f;
        Vector3 offset = transform.position + transform.up * halfsize.y;
        Gizmos.DrawLine(offset, offset +  transform.forward);
        
        // Define top and side vector
        Vector3 top = transform.up * size.y;
        Vector3 side = transform.right * halfsize.x;
        
        // Define corner vectors
        Vector3 topRight = transform.position + top + side;
        Vector3 topLeft = transform.position + top - side;
        Vector3 bottomRight = transform.position + side;
        Vector3 bottomLeft = transform.position - side;
        
        // Draw outer lines
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);
        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        
        // Draw diagonal lines
        Gizmos.DrawLine(topRight, offset);
        Gizmos.DrawLine(topLeft, offset);
        Gizmos.DrawLine(bottomRight, offset);
        Gizmos.DrawLine(bottomLeft, offset);
        
        
    }
}
