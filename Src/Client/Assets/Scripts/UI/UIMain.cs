using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Entities;
using Common.Data;
using Assets.Scripts.UI.Temp;

public class UIMain : MonoSingleton<UIMain>
{
	public Text avatarName;//玩家名字
	public TextMeshProUGUI avatarLevel;//玩家等级
	public Image Icon;//玩家头像
	public Image miniMapIcon;//小地图头像
	public UITemp TempWindow;
    protected override void OnStart()
    {
		this.UpdateAvatar();
	}

	void UpdateAvatar()
	{
		//将角色的名字赋值给UI显示
		this.avatarName.text = string.Format("{0}", User.Instance.CurrentCharacter.Name);
		//将角色的等级赋值给UI显示
		this.avatarLevel.text = User.Instance.CurrentCharacter.Level.ToString();
		this.Icon.overrideSprite = SpriteManager.Instance.classIcons[(int)User.Instance.CurrentCharacter.Class - 1];
        this.miniMapIcon.overrideSprite = SpriteManager.Instance.classIcons[(int)User.Instance.CurrentCharacter.Class - 1];
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

	public void OnClickBag()
	{
		UIManager.Instance.Show<UIBag>();
	}

	public void OnClickCharEquip()
	{
		UIManager.Instance.Show<UICharEquip>();
	}

	public void OnClickQuest()
	{
		UIManager.Instance.Show<UIQuestSystem>();
    }

	public void OnClickFriend()
	{
		UIManager.Instance.Show<UIFriends>();
	}

	public void OnClickTemp()
	{

	}

	public void ShowTeamUI(bool show)
	{
		TempWindow.ShowTeam(show);
	}

	public void OnClickSettings()
	{

	}
}
