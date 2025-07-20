using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavPathRenderer : MonoSingleton<NavPathRenderer>
{
    LineRenderer pathRenderer;
    NavMeshPath path;

    void Start()
    {
        pathRenderer = GetComponent<LineRenderer>();
        pathRenderer.enabled = false;
    }

    public void SetPath(NavMeshPath path, Vector3 target)
    {
        this.path = path;
        if (this.path == null)
        {
            //如果没有路径，禁用渲染器
            pathRenderer.enabled = false;
            pathRenderer.positionCount = 0;
        }
        else
        {
            pathRenderer.enabled = true;//启用渲染器
            pathRenderer.positionCount = path.corners.Length + 1;//根据path路径的拐点，设置renderer的拐点（+1是因为终点不包含）
            pathRenderer.SetPositions(path.corners);//设置renderer每一个的拐点位置
            pathRenderer.SetPosition(pathRenderer.positionCount - 1, target);//设置renderer的最后一个点为目标点
            for (int i = 0; i < pathRenderer.positionCount; i++)
            {
                //将每个点的高度设置为0.2f，避免渲染在地面上
                pathRenderer.SetPosition(i, pathRenderer.GetPosition(i) + Vector3.up * 0.5f);
            }
        }
    }
}
