using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RideController : MonoBehaviour
{
    public Transform mountPoint;//骑乘点
    public EntityContorller rider;//骑乘者
    public Vector3 offset;//偏移
    public Animator anim;

    void Start()
    {
        //this.anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.mountPoint == null || this.rider == null) return;//如果没有骑乘点或则骑乘者，则返回

        //否则，设置骑乘者的pos
        this.rider.SetridePosition(mountPoint.position + this.mountPoint.TransformDirection(offset));
    }
    
    /// <summary>
    /// 设置骑乘者
    /// </summary>
    /// <param name="rider"></param>
    public void SetRider(EntityContorller rider)
    {
        this.rider = rider;
    }

    /// <summary>
    /// 坐骑状态触发事件
    /// </summary>
    /// <param name="entityEvent"></param>
    /// <param name="param"></param>
    public void OnEntityEvent(EntityEvent entityEvent, int param)
    {
        switch (entityEvent)
        {
            case EntityEvent.Idle:
                anim.SetBool("Move", false);
                anim.SetBool("Idle", true);
                break;
            case EntityEvent.MoveFwd:
                anim.SetBool("Move", true);
                anim.SetBool("Idle", false);
                break;
            case EntityEvent.MoveBack:
                anim.SetBool("Move", true);
                anim.SetBool("Idle", false);
                break;
            case EntityEvent.Jump:
                //anim.SetTrigger("Jump");
                break;
        }
    }
}
