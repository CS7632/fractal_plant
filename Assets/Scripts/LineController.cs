using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    private LineRenderer lr;
    private List<Vector3> points;
    
    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 0;
        points = new List<Vector3>();
    }

    public void AddPoint(Vector3 point)
    {
        lr.positionCount++;
        points.Add(point);
    }

    public LineRenderer LRend()
    {
        return lr;
    }

    private void LateUpdate()
    {
        if (points.Count < 2) return;
        
        for (var i = 0; i < points.Count; i++)
        {
            lr.SetPosition(i, points[i]);
        }
    }
}
