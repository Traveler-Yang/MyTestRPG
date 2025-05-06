using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldElement : MonoBehaviour {

    public Transform owner;//UI跟随者

    public float height = 1.5f;//UI向上偏移的大小
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (owner != null)
        {
            this.transform.position = owner.position + Vector3.up * height;
        }
    }
}
