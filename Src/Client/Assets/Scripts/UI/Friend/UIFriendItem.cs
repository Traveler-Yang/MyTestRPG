using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SkillBridge.Message;

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

    public void SetFriendInfo(NFriendInfo item)
    {
        this.info = item;
        if (this.nickName != null) this.nickName.text = this.info.friendInfo.Name;
        if (this.@class != null) this.@class.text = this.info.friendInfo.Class.ToString();
        if (this.Level != null) this.Level.text = this.info.friendInfo.Level.ToString();
        if (this.status != null) this.status.text = this.info.Status == true ? "在线" : "离线";
    }
}
