using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterObject : MonoBehaviour
{
    public int ID;//´«ËÍµãID
    Mesh mesh = null;

    void Start()
    {
        mesh = this.GetComponent<MeshFilter>().sharedMesh;
    }


    void Update()
    {

    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (this.mesh != null)
        {
            Gizmos.DrawMesh(this.mesh, this.transform.position + Vector3.up * this.transform.localScale.y * .5f, this.transform.rotation, this.transform.localScale);
        }
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.ArrowHandleCap(0, this.transform.position,this.transform.rotation, 1f, EventType.Repaint);
    }
#endif
}
