using Entities;
using Managers;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EntityContorller : MonoBehaviour, IEntityNotify{

	public Animator anim;
    public CharacterController controller;

	public Entity entity;

	public UnityEngine.Vector3 position;
	public UnityEngine.Vector3 direction;
	Quaternion rotation;

	public UnityEngine.Vector3 lastPosition;
	Quaternion lastRotation;

    public bool isPlayer  = false;

    public RideController rideController;

    private int currentRide = 0;//当前坐骑的id

    public Transform rideBone;//坐骑骨骼

	void Start ()
	{
		if (entity != null)
		{
			EntityManager.Instance.RegisterEntityChangeNotify(entity.entityId, this);
			this.UpdateTransform();
		}
    }

    public void SetSpeed(float speed)
    {
        anim.SetFloat("Speed", speed);
    }

    public void SetDirectionSpeed(float speed)
    {
        anim.SetFloat("Turn Speed", speed);
    }

    void UpdateTransform()
	{
		this.position = GameObjectTool.LogicToWorld(entity.position);
		this.direction = GameObjectTool.LogicToWorld(entity.direction);

		this.transform.position = this.position;
		this.transform.forward = this.direction;
		this.lastPosition = this.position;
		this.lastRotation = this.rotation;
	}

	void OnDestroy()
	{
		if (entity != null)
			Debug.LogFormat("{0} OnDestroy :ID:{1} POS:{2} DIR:{3} SPD:{4}", this.name, entity.entityId, entity.position, entity.direction, entity.speed);

		if(UIWorldElementManager.Instance != null)
		{
			UIWorldElementManager.Instance.RemoveCharacterNameBar(this.transform);
		}
	}

	void FixedUpdate()
	{
		if (this.entity == null)
			return;

		this.entity.OnUpdate(Time.fixedDeltaTime);

        if (!this.isPlayer)
        {
            this.UpdateTransform();
        }
    }

    public void OnEntityRemoved()
    {
        if (UIWorldElementManager.Instance != null)
            UIWorldElementManager.Instance.RemoveCharacterNameBar(this.transform);
        Destroy(this.gameObject);
    }

    public void OnEntityEvent(EntityEvent entityEvent, int param)
    {
        switch(entityEvent)
        {
            case EntityEvent.Idle:
                anim.SetBool("Move", false);
                anim.SetBool("MoveFow", false);
                anim.SetBool("MoveBack", false);
                anim.SetTrigger("Idle");
                break;
            case EntityEvent.MoveFwd:
                anim.SetBool("Move", true);
                anim.SetBool("MoveFow", true);
                break;
            case EntityEvent.MoveBack:
                anim.SetBool("Move", true);
                anim.SetBool("MoveBack", true);
                break;
            case EntityEvent.Jump:
                anim.SetTrigger("Jump");
                break;
            case EntityEvent.RunningJump:
                anim.SetTrigger("RunningJump");
                break;
            case EntityEvent.Ride:
                this.Ride(param);
                break;
        }
        if (this.rideController != null) this.rideController.OnEntityEvent(entityEvent, param);
    }

    public void OnEntityChanged(Entity entity)
    {
        Debug.LogFormat("OnEntityChanged :ID:{0} POS:{1} DIR:{2} SPD:{3}", entity.entityId, entity.position, entity.direction, entity.speed);
    }

    public void Ride(int rideId)
    {
        if (this.currentRide == rideId) return;
        this.currentRide = rideId;
        if (currentRide > 0)
        {
            //上马
            this.rideController = GameObjectManager.Instance.LoadRide(rideId, this.transform);
        }
        else
        {
            //下马
            Destroy(this.rideController.gameObject);
            this.rideController = null;
        }

        if (this.rideController == null)
        {
            //下马后将角色的动画设为0
            this.anim.transform.localPosition = Vector3.zero;
            //如果不是骑乘状态，将权重设置为0
            this.anim.SetLayerWeight(1, 0);
        }
        else
        {
            //上马后设置骑乘者和权重
            this.rideController.SetRider(this);
            //骑马状态，将权重设置为1
            this.anim.SetLayerWeight(1, 1);
        }
    }

    /// <summary>
    /// 根据坐骑设置角色位置
    /// </summary>
    /// <param name="position"></param>
    public void SetridePosition(Vector3 position)
    {
        this.anim.transform.position = position + (this.anim.transform.position - this.rideBone.position);
    }
}
