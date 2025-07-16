using Entities;
using Managers;
using Services;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputContorller : MonoBehaviour {

    public CharacterController controller;

    private float verticalVelocity = 0f; // 垂直速度
    public float gravity = 9.81f;        // 重力值

    SkillBridge.Message.CharacterState state;

	public Character character;//角色

	public float rotateSpeed = 2.0f;//旋转速度

	public float turnAngle = 10;

	public int speed;//移动速度

    public float jumpPower = 3.0f;//跳跃力度

    public EntityContorller entityContorller;

	public bool onAir = false;
	void Start () 
	{
        controller = GetComponent<CharacterController>();
		state = SkillBridge.Message.CharacterState.Idle;
		if (this.character == null)
		{
			DataManager.Instance.Load();
			NCharacterInfo cinfo = new NCharacterInfo();
			cinfo.Id = 1;
			cinfo.Name = "Test";
			cinfo.ConfigId = 1;
			cinfo.Entity = new NEntity();
			cinfo.Entity.Position = new NVector3();
			cinfo.Entity.Direction = new NVector3();
			cinfo.Entity.Direction.X = 0;
			cinfo.Entity.Direction.Y = 100;
			cinfo.Entity.Direction.Z = 0;
			this.character = new Character(cinfo);

			if (entityContorller != null) entityContorller.entity = this.character;
		}
    }

    void Update ()
    {
        if (character == null || this.entityContorller == null) return;

        CaculateGravity();
        if (InputManager.Instance != null && InputManager.Instance.IsInputMode) return;

        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        Vector3 move = new Vector3(h, 0, v);
        Vector3 moveDir = transform.TransformDirection(move.normalized);

        // 前后移动方向（角色面向方向）
        // 状态切换
        if (v > 0)
        {
            character.MoveForward();
            SendEntityEvent(EntityEvent.MoveFwd);
        }
        else if (v < 0)
        {
            character.MoveBack();
            SendEntityEvent(EntityEvent.MoveBack);
        }
        else
        {
            character.Stop();
            SendEntityEvent(EntityEvent.Idle);
        }

        // 左右转向
        if (h != 0)
        {
            transform.Rotate(0, h * rotateSpeed, 0);
            Vector3 dir = GameObjectTool.LogicToWorld(character.direction);
            Quaternion rot = Quaternion.FromToRotation(dir, transform.forward);
            if (rot.eulerAngles.y > turnAngle && rot.eulerAngles.y < (360 - turnAngle))
            {
                character.SetDirection(GameObjectTool.WorldToLogic(transform.forward));
                SendEntityEvent(EntityEvent.None);
            }
        }

        //跳跃
        if (controller.isGrounded && Input.GetButtonDown("Jump"))
        {
            verticalVelocity = jumpPower;
            onAir = true;
            if (v > 0)
                SendEntityEvent(EntityEvent.RunningJump);
            else
                SendEntityEvent(EntityEvent.Jump);
        }

        //加入垂直方向重力
        Vector3 finalMove = moveDir * character.speed;
        finalMove.y = verticalVelocity;

        controller.Move(finalMove * Time.deltaTime);

        if (entityContorller != null)
        {
            float speed = moveDir.magnitude;
            entityContorller.SetSpeed(speed);
        }
    }

    /// <summary>
    /// 模拟重力
    /// </summary>
    void CaculateGravity()
    {
        if (controller.isGrounded)
        {
            verticalVelocity = -gravity * Time.deltaTime;
            return;
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }
    }


    Vector3 lastPos;
    float lastSync = 0;
    private void LateUpdate()
    {
        Vector3 offset = this.transform.position - lastPos;
        this.speed = (int)(offset.magnitude * 100f / Time.deltaTime);

        this.lastPos = this.transform.position;

        var logicPos = GameObjectTool.WorldToLogic(this.transform.position);
        if ((logicPos - this.character.position).magnitude > 50)
        {
            this.character.SetPosition(logicPos);
            this.SendEntityEvent(EntityEvent.None);
        }

        Vector3 dir = GameObjectTool.LogicToWorld(character.direction);
        Quaternion rot = Quaternion.FromToRotation(dir, this.transform.forward);
        if (rot.eulerAngles.y > this.turnAngle && rot.eulerAngles.y < (360 - this.turnAngle))
        {
            character.SetDirection(GameObjectTool.WorldToLogic(this.transform.forward));
            this.SendEntityEvent(EntityEvent.None);
        }
    }

    /// <summary>
    /// 发送角色状态消息
    /// </summary>
    /// <param name="entityEvent">状态</param>
	public void SendEntityEvent(EntityEvent entityEvent, int param = 0)
	{
        //把当前的信息发送给角色
		if (entityContorller != null)
        {
            entityContorller.OnEntityEvent(entityEvent, param);
            Debug.Log("SendEntityEvent" + entityEvent);
        }
        //将自身当前的状态发送给当前地图中的所有角色
        MapService.Instance.SendMapEntitySync(entityEvent, this.character.EntityData, param);
	}
}
