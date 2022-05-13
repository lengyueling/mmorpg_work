using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AI;

public class NavPathRenderer : MonoSingleton<NavPathRenderer>
{
    LineRenderer pathRenderer;
    NavMeshPath path;

    private void Start()
    {
        pathRenderer = this.GetComponent<LineRenderer>();
        pathRenderer.enabled = false;
    }

    /// <summary>
    /// 设置路径
    /// </summary>
    /// <param name="path"></param>
    /// <param name="target"></param>
    public void SetPath(NavMeshPath path, Vector3 target)
    {
        this.path = path;
        if (this.path == null)
        {
            pathRenderer.enabled = false;
            pathRenderer.positionCount = 0;
        }
        else
        {
            pathRenderer.enabled = true;
            pathRenderer.positionCount = path.corners.Length + 1;
            pathRenderer.SetPositions(path.corners);
            pathRenderer.SetPosition(pathRenderer.positionCount - 1, target);

            //设置一个偏移量让每部分的路径浮在地面上
            for (int i = 0; i < pathRenderer.positionCount; i++)
            {
                pathRenderer.SetPosition(i, pathRenderer.GetPosition(i) + Vector3.up * 0.2f);
            }
        }
    }
}
