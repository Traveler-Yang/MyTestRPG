using Managers;
using Services;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGuild : UIWindow
{
    public GameObject itemPrefab;//成员Prefab
    public ListView listMain;//成员列表
    public Transform listRoot;//成员列表根节点
    public UIGuildInfo uiInfo;//公会信息栏
    public UIGuildMemberItem selectedItem;//当前选中的成员
    void Start()
    {
        GuildService.Instance.OnGuildUpdate = UpdateUI;
        this.listMain.onItemSelected += OnGuildMemberSelected;
        this.UpdateUI();
    }

    private void OnDestroy()
    {
        
    }

    private void UpdateUI()
    {
        this.uiInfo.Info = GuildManager.Instance.guildInfo;
        ClearList();
        InitItems();
    }

    private void OnGuildMemberSelected(ListView.ListViewItem item)
    {
        this.selectedItem = item as UIGuildMemberItem;
    }

    /// <summary>
    /// 初始化成员列表
    /// </summary>
    /// <param name="guilds"></param>
    private void InitItems()
    {
        foreach (var item in GuildManager.Instance.guildInfo.Members)
        {
            GameObject go = Instantiate(itemPrefab, this.listMain.transform);
            UIGuildMemberItem ui = go.GetComponent<UIGuildMemberItem>();
            ui.SetGuildMemberInfo(item);
            this.listMain.AddItem(ui);
        }
    }

    private void ClearList()
    {
        this.listMain.RemoveAll();
    }

    /// <summary>
    /// 转让
    /// </summary>
    public void OnClickTransfer()
    {
        MessageBox.Show("暂未开放");
    }

    /// <summary>
    /// 升职
    /// </summary>
    public void OnClickPromotion()
    {
        MessageBox.Show("暂未开放");
    }

    /// <summary>
    /// 罢免
    /// </summary>
    public void OnClickRecall()
    {
        MessageBox.Show("暂未开放");
    }

    /// <summary>
    /// 申请列表
    /// </summary>
    public void OnClickRequestList()
    {
        MessageBox.Show("暂未开放");
    }

    /// <summary>
    /// 踢出公会
    /// </summary>
    public void OnClickKickOut()
    {
        MessageBox.Show("暂未开放");
    }

    /// <summary>
    /// 私聊
    /// </summary>
    public void OnClickChat()
    {
        MessageBox.Show("暂未开放");
    }

    /// <summary>
    /// 退出公会
    /// </summary>
    public void OnClickLeave()
    {
        MessageBox.Show(string.Format("确定要退出公会 [{0}] 吗", GuildManager.Instance.guildInfo.GuildName), "退出公会", MessageBoxType.Confirm).OnYes = () =>
        {
            GuildService.Instance.SendGuildLeaveRequest();
        };
    }
}
