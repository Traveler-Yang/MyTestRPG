using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMain : MonoSingleton<UIMain>
{

	public Text avatarName;//玩家名字
	public Text avatarLevel;//玩家等级
    protected override void OnStart()
    {
		this.UpdateAvatar();
	}

	void UpdateAvatar()
	{
		//将角色的名字赋值给UI显示
		this.avatarName.text = string.Format("{0} [{1}]", User.Instance.CurrentCharacter.Name, User.Instance.CurrentCharacter.Id);
		//将角色的等级赋值给UI显示
		this.avatarLevel.text = User.Instance.CurrentCharacter.Level.ToString();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	/// <summary>
	/// 返回角色选择按钮
	/// </summary>
	public void BackToCharSelect()
	{
		SceneManager.Instance.LoadScene("CharSelect");//切换场景为角色选择
		Services.UserService.Instance.SendGameLeave();//发送角色离开的消息到服务器
	}

	public void TestClick()
	{
		UITest test = UIManager.Instance.Show<UITest>(); //打开UITest界面
		test.Set("这是一个标题");
    }

	public void ClickBag()
	{
		UIManager.Instance.Show<UIBag>();
	}
}
