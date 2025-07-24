using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Entities;
using Common.Data;
using Assets.Scripts.UI.Temp;
using Managers;

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
		InputManager.Instance.EnableGameplayInput = true;
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
	/// 背包按钮
	/// </summary>
	public static void OnClickBag()
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Btn_1);
        UIManager.Instance.Show<UIBag>();
	}

	/// <summary>
	/// 装备按钮
	/// </summary>
	public static void OnClickCharEquip()
	{
		SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Btn_1);
        UIManager.Instance.Show<UICharEquip>();
	}

	/// <summary>
	/// 任务按钮
	/// </summary>
	public static void OnClickQuest()
	{
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Btn_1);
        UIManager.Instance.Show<UIQuestSystem>();
    }

	/// <summary>
	/// 好友按钮
	/// </summary>
	public static void OnClickFriend()
	{
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Btn_1);
        UIManager.Instance.Show<UIFriends>();
	}

	/// <summary>
	/// 公会按钮
	/// </summary>
	public static void OnClickGuild()
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Btn_1);
        GuildManager.Instance.ShowGuild();
	}

	/// <summary>
	/// 坐骑按钮
	/// </summary>
	public static void OnClickRide()
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Btn_1);
        UIManager.Instance.Show<UIRide>();
	}

	/// <summary>
	/// 技能按钮
	/// </summary>
	public static void OnClickSKill()
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Btn_1);
        MessageBox.Show("暂未开放", "提示", MessageBoxType.Information);
    }

    /// <summary>
    /// 设置按钮
    /// </summary>
    public static void OnClickSetting()
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Btn_1);
        UIManager.Instance.Show<UISetting>();	
    }

	/// <summary>
	/// 邮件按钮
	/// </summary>
	public static void OnClickEmail()
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Btn_1);
        MessageBox.Show("暂未开放", "提示", MessageBoxType.Information);
	}

    public void ShowTeamUI(bool show)
	{
		TempWindow.ShowTeam(show);
	}

}
