using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SkillBridge.Message;
using Assets.Scripts.UI.Temp;

public class UIFriendItem : ListView.ListViewItem
{
    public Image icon;//头像图标
    public Text nickName;//昵称
    public TextMeshProUGUI @class;//职业
    public TextMeshProUGUI Level;//等级
    public Text status;//状态

    public Image backGround;//背景
    public Sprite normalBg;//正常背景
    public Sprite selectBg;//选中背景
    public override void onSelected(bool selected)
    {
        this.backGround.overrideSprite = selected ? selectBg : normalBg;
    }

    public NFriendInfo info;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 设置好友项信息
    /// </summary>
    /// <param name="item"></param>
    public void SetFriendInfo(NFriendInfo item)
    {
        this.info = item;
        if (this.nickName != null) this.nickName.text = this.info.friendInfo.Name;
        if (this.icon != null) this.icon.overrideSprite = SpriteManager.Instance.classIcons[(int)this.info.friendInfo.Class - 1];
        if (this.@class != null)
        {
            switch (this.info.friendInfo.Class)
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
                default:
                    break;
            }
        }
        if (this.Level != null) this.Level.text = string.Format("Lv: {0}", this.info.friendInfo.Level);
        if (this.status != null) this.status.text = this.info.Status == true ? "在线" : "离线";
    }
}
