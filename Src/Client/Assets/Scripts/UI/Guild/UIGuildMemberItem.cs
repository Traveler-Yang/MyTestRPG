using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildMemberItem : ListView.ListViewItem
{
    public Text nickName;//昵称
    public TextMeshProUGUI @class;//职业
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
        if (this.nickName != null) this.nickName.text = this.info.charInfo.Name;
        if (this.@class != null) this.@class.text = this.info.charInfo.Class.ToString();
        if (this.level != null) this.level.text = this.info.charInfo.Level.ToString();
        if (this.duty != null) this.duty.text = this.info.Duty.ToString();
        if (this.status != null) this.status.text = this.info.Status.ToString();
    }

    /// <summary>
    /// 点击详细信息
    /// </summary>
    public void OnclickDetails()
    {

    }
}
