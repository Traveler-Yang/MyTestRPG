using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SkillBridge.Message;
using Assets.Scripts.UI.Temp;

public class UITempItem : ListView.ListViewItem
{
    public Text nickName;//昵称
    public Image classIcon;//职业图标
    public TextMeshProUGUI level;//等级
    public Image leaderIcon;//队长图标

    public Image backGround;//背景
    public override void onSelected(bool selected)
    {
        //是否显示
        this.backGround.enabled = selected ? true : false;
    }

    public int idx;
    public NCharacterInfo Info;

    void Start()
    {
        
    }

    /// <summary>
    /// 设置队伍成员项信息
    /// </summary>
    /// <param name="idx">索引</param>
    /// <param name="info">角色信息</param>
    /// <param name="isLeader">是否是队长</param>
    public void SetMemberInfo(int idx, NCharacterInfo info, bool isLeader)
    {
        this.idx = idx;
        this.Info = info;
        if (this.nickName != null) this.nickName.text = this.Info.Name;
        if (this.classIcon != null) this.classIcon.overrideSprite = SpriteManager.Instance.classIcons[(int)this.Info.Class - 1];
        if (this.level != null) this.level.text = this.Info.Level.ToString();
        this.leaderIcon.gameObject.SetActive(isLeader ? true : false);
    }
}
