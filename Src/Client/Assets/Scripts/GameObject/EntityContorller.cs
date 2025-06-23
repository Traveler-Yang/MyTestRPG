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
	public Rigidbody rb;
	private AnimatorStateInfo currentBaseState;

	public Entity entity;

	public UnityEngine.Vector3 position;
	public UnityEngine.Vector3 direction;
	Quaternion rotation;

	public UnityEngine.Vector3 lastPosition;
	Quaternion lastRotation;

    public float forwardSpeed = -1f;//前进速度
    public float backwardSpeed = 2f;//后退速度
    float currentSpeed;
    float targetSpeed;//当前速度
    Vector3 movement;//移动向量

    public bool isPlayer  = false;

	void Start ()
	{
		if (entity != null)
		{
			EntityManager.Instance.RegisterEntityChangeNotify(entity.entityId, this);
			this.UpdateTransform();
		}

        if (!this.isPlayer)
            rb.useGravity = false;
    }

    private void Update()
    {
		anim.SetFloat("Speed",rb.velocity.z);
    }

    void UpdateTransform()
	{
		this.position = GameObjectTool.LogicToWorld(entity.position);
		this.direction = GameObjectTool.LogicToWorld(entity.direction);

		this.rb.MovePosition(this.position);
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

    public void OnEntityEvent(EntityEvent entityEvent)
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
        }
    }

    public void OnEntityChanged(Entity entity)
    {
        Debug.LogFormat("OnEntityChanged :ID:{0} POS:{1} DIR:{2} SPD:{3}", entity.entityId, entity.position, entity.direction, entity.speed);
    }
}
