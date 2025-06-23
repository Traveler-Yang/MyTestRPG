using Entities;
using Services;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputContorller : MonoBehaviour {

	public Rigidbody rb;//刚体组件
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
		state = SkillBridge.Message.CharacterState.Idle;
		if (this.character == null)
		{
			DataManager.Instance.Load();
			NCharacterInfo cinfo = new NCharacterInfo();
			cinfo.Id = 1;
			cinfo.Name = "Test";
			cinfo.Tid = 1;
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
    
    void FixedUpdate()
	{
        if (character == null || this.entityContorller == null)
            return;
        #region 前后移动
        float v = Input.GetAxis("Vertical");
        if (v > 0.01)//向前移动
        {
            //判断当前状态是否为移动状态，如果不是则切换到移动状态
            if (state != SkillBridge.Message.CharacterState.Move)
            {
                //切换为移动状态
                state = SkillBridge.Message.CharacterState.Move;
                this.character.MoveForward();
                this.SendEntityEvent(EntityEvent.MoveFwd);
            }
            this.rb.velocity = this.rb.velocity.y * Vector3.up + GameObjectTool.LogicToWorld(character.direction) * (character.speed + 9.81f) / 100f;
        }
        else if (v < -0.01)//向后移动
        {
            if (state != SkillBridge.Message.CharacterState.Move)
            {
                state = SkillBridge.Message.CharacterState.Move;
                this.character.MoveBack();
                this.SendEntityEvent(EntityEvent.MoveBack);
            }
            this.rb.velocity = this.rb.velocity.y * Vector3.up + GameObjectTool.LogicToWorld(character.direction) * (character.speed + 9.81f) / 100f;
        }
        else
        {
            if (state != SkillBridge.Message.CharacterState.Idle)
            {
                state = SkillBridge.Message.CharacterState.Idle;
                this.rb.velocity = Vector3.zero;
                this.character.Stop();
                this.SendEntityEvent(EntityEvent.Idle);
            }
        }
        #endregion

        if (rb.velocity.y == 0)
        {
            onAir = false; //如果y轴速度为0则设置为不在空中状态
        }
        if (Input.GetButtonDown("Jump"))
		{
            if (onAir)
                //如果在空中则不允许跳跃
                return;

            //如果不在空中则允许跳跃
            this.rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            onAir = true; //设置为在空中状态
            //如果当前速度大于0，则发送RunningJump事件
            if (v > 0)
            {
                this.SendEntityEvent(EntityEvent.RunningJump);
                return;
            }
            this.SendEntityEvent(EntityEvent.Jump);
        }

        #region 左右转向
        float h = Input.GetAxis("Horizontal");
        if (h > 0.01 || h < -0.01)
        {
            this.transform.Rotate(0, h * rotateSpeed, 0);
            Vector3 dir = GameObjectTool.LogicToWorld(character.direction);
            Quaternion rot = new Quaternion();
            rot.SetFromToRotation(dir, this.transform.forward);

            if (rot.eulerAngles.y > this.turnAngle && rot.eulerAngles.y < (360 - this.turnAngle))
            {
                character.SetDirection(GameObjectTool.WorldToLogic(this.transform.forward));
                rb.transform.forward = this.transform.forward;
                this.SendEntityEvent(EntityEvent.None);
            }
        }
        #endregion
    }

    Vector3 lastPos;
    float lastSync = 0;
    private void LateUpdate()
    {
        Vector3 offset = this.rb.transform.position - lastPos;
        this.speed = (int)(offset.magnitude * 100f / Time.deltaTime);

        this.lastPos = this.rb.transform.position;

        if ((GameObjectTool.WorldToLogic(this.rb.transform.position) - this.character.position).magnitude > 50)
        {
            this.character.SetPosition(GameObjectTool.WorldToLogic(this.rb.transform.position));
            this.SendEntityEvent(EntityEvent.None);
        }
        this.transform.position = this.rb.transform.position;

        Vector3 dir = GameObjectTool.LogicToWorld(character.direction);
        Quaternion rot = new Quaternion();
        rot.SetFromToRotation(dir, this.transform.forward);
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
	void SendEntityEvent(EntityEvent entityEvent)
	{
        //把当前的信息发送给角色
		if (entityContorller != null)
        {
            entityContorller.OnEntityEvent(entityEvent);
            Debug.Log("SendEntityEvent" + entityEvent);
        }
        //将自身当前的状态发送给当前地图中的所有角色
        MapService.Instance.SendMapEntitySync(entityEvent, this.character.EntityData);
	}
}
