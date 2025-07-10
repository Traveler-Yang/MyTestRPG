using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildMemberItem : ListView.ListViewItem
{
    public Text nickName;//昵称
    public Text @class;//职业
    public TextMeshProUGUI level;//等级
    public Text duty;//职务
    public Text status;//状态

    public Image backGround;//背景
    public Sprite normalBg;//正常背景
    public Sprite selectBg;//选中背景
    public override void onSelected(bool selected)
    {
        this.backGround.overrideSprite = selected ? selectBg : normalBg;
    }

    public NGuildMemberInfo info;
    public void SetGuildMemberInfo(NGuildMemberInfo info)
    {
        this.info = info;
        if (this.nickName != null) this.nickName.text = this.info.charInfo.Name;
        if (this.@class != null)
        {
            switch (this.info.charInfo.Class)
            {
                case CharacterClass.Warrior:
                    this.@class.text = "战士";
                    break;
                case CharacterClass.Wizard:
                    this.@class.text = "法师";
                    break;
                case CharacterClass.Archer:
                    this.@class.text = "弓箭手";
                    break;
            }
        }
        if (this.level != null) this.level.text = this.info.charInfo.Level.ToString();
        if (this.duty != null)
        {
            switch (this.info.Duty)
            {
                case GuildDuty.None:
                    this.duty.text = "普通成员";
                    break;
                case GuildDuty.President:
                    this.duty.text = "会长";
                    break;
                case GuildDuty.VicePresident:
                    this.duty.text = "副会长";
                    break;
            }
        }
        if (this.status != null) this.status.text = this.info.Status ? "在线" : "离线";
    }

    /// <summary>
    /// 点击详细信息
    /// </summary>
    public void OnclickDetails()
    {

    }
}
