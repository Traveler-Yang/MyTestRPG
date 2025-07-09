using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SkillBridge.Message;
using Services;

public class UIGuildApplyItem : ListView.ListViewItem
{
    public Text nickName;//昵称
    public Text @class;//职业
    public TextMeshProUGUI level;//等级

    void Start()
    {
        
    }

    private NGuildApplyInfo info;
    public void SetItemInfo(NGuildApplyInfo info)
    {
        this.info = info;
        if (this.nickName != null) this.nickName.text = this.info.characterName;
        if (this.@class != null)
        {
            switch (this.info.Class)
            {
                case (int)CharacterClass.Warrior:
                    this.@class.text = "战士";
                    break;
                case (int)CharacterClass.Wizard:
                    this.@class.text = "法师";
                    break;
                case (int)CharacterClass.Archer:
                    this.@class.text = "弓箭手";
                    break;
            }
        }
        if (this.level != null) this.level.text = string.Format("Lv: {0}", this.info.Level);
    }

    /// <summary>
    /// 通过
    /// </summary>
    public void OnAccept()
    {
        MessageBox.Show(string.Format("确定要通过 [{0}] 的公会申请吗", this.info.characterName), "审批申请", MessageBoxType.Confirm, "通过", "取消").OnYes = () =>
        {
            GuildService.Instance.SendGuilJoinApply(true, this.info);
        };
    }

    /// <summary>
    /// 拒绝
    /// </summary>
    public void OnDecline()
    {
        MessageBox.Show(string.Format("确定要拒绝 [{0}] 的公会申请吗", this.info.characterName), "审批申请", MessageBoxType.Confirm, "拒绝", "取消").OnYes = () =>
        {
            GuildService.Instance.SendGuilJoinApply(false, this.info);
        };
    }
}
