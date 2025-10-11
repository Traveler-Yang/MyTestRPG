using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using SkillBridge.Message;

public class UIFriendItem : ListView.ListViewItem
{
    public Image icon;//头像
    public TextMeshProUGUI name;//名字
    public TextMeshProUGUI level;//等级
    public TextMeshProUGUI @class;//职业
    public TextMeshProUGUI status;//状态

    public Image backGround;//商品的背景图
    public Sprite normalBg;//正常状态的背景图
    public Sprite selectBg;//选中状态的背景图

    public NFriendInfo info;

    public override void onSelected(bool selected)
    {
        this.backGround.overrideSprite = selected ? selectBg : normalBg;
    }

    public void SetFriendInfo(NFriendInfo info)
    {
        this.info = info;
        if (this.name != null) this.name.text = this.info.Info.Name;
        if (this.level != null) this.level.text = this.info.Info.Level.ToString();
        if (this.@class != null)
        {
            switch (this.info.Info.Class)
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
        if (this.status != null) this.status.text = this.info.Status ? "在线" : "离线";
    }
}
