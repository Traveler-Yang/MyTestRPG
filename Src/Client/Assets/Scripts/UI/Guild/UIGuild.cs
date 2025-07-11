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

    public GameObject panelAdmin;//长老面板
    public GameObject panelLeader;//会长面板

    void Start()
    {
        GuildService.Instance.OnGuildUpdate += UpdateUI;
        this.listMain.onItemSelected += OnGuildMemberSelected;
        this.UpdateUI();
    }

    private void OnDestroy()
    {
        GuildService.Instance.OnGuildUpdate -= UpdateUI;
    }

    private void UpdateUI()
    {
        this.uiInfo.Info = GuildManager.Instance.guildInfo;
        ClearList();
        InitItems();

        //如果我的管理权限大于普通成员的，就显示管理员的按钮，反之否
        this.panelAdmin.SetActive(GuildManager.Instance.MyMemberInfo.Duty > GuildDuty.None);
        //如果我是会长，则显示会长的权限按钮，反之否
        this.panelLeader.SetActive(GuildManager.Instance.MyMemberInfo.Duty == GuildDuty.President);
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
        if (this.selectedItem == null)
        {
            MessageBox.Show("请选择要转让的成员", "错误", MessageBoxType.Error);
            return;
        }
        MessageBox.Show(string.Format("确定要将会长职务转让给 [{0}] 吗", selectedItem.info.charInfo.Name), "转让会长", MessageBoxType.Confirm).OnYes = () =>
        {
            GuildService.Instance.SendAdminCommand(GuildAdminCommand.Transfer, "", "", this.selectedItem.info.charInfo.Id);
        };
    }

    /// <summary>
    /// 升职
    /// </summary>
    public void OnClickPromotion()
    {
        if (this.selectedItem == null)
        {
            MessageBox.Show("请选择要晋升的成员", "错误", MessageBoxType.Error);
            return;
        }
        if (this.selectedItem.info.Duty != GuildDuty.None)
        {
            MessageBox.Show("对方似乎已是尊贵身份了", "晋升", MessageBoxType.Information);
            return;
        }
        MessageBox.Show(string.Format("确定要将 [{0}] 晋升为副会长吗", selectedItem.info.charInfo.Name), "成员晋升", MessageBoxType.Confirm).OnYes = () =>
        {
            GuildService.Instance.SendAdminCommand(GuildAdminCommand.Promote, "", "", this.selectedItem.info.charInfo.Id);
        };
    }

    /// <summary>
    /// 罢免
    /// </summary>
    public void OnClickRecall()
    {
        if (this.selectedItem == null)
        {
            MessageBox.Show("请选择要罢免的成员", "错误", MessageBoxType.Error);
            return;
        }
        if (this.selectedItem.info.Duty == GuildDuty.None)
        {
            MessageBox.Show("对方似乎无职可免", "罢免", MessageBoxType.Information);
            return;
        }
        if (this.selectedItem.info.Duty == GuildDuty.President)
        {
            MessageBox.Show("会长可不是你能动的", "罢免", MessageBoxType.Information);
            return;
        }
        MessageBox.Show(string.Format("确定要将 [{0}] 罢免为普通成员吗", selectedItem.info.charInfo.Name), "职务罢免", MessageBoxType.Confirm).OnYes = () =>
        {
            GuildService.Instance.SendAdminCommand(GuildAdminCommand.Depost, "", "", this.selectedItem.info.charInfo.Id);
        };
    }

    /// <summary>
    /// 申请列表
    /// </summary>
    public void OnClickRequestList()
    {
        UIManager.Instance.Show<UIGuildApplyList>();
    }

    /// <summary>
    /// 踢出公会
    /// </summary>
    public void OnClickKickOut()
    {
        if (this.selectedItem == null)
        {
            MessageBox.Show("请选择要踢出的成员", "错误", MessageBoxType.Error);
            return;
        }
        MessageBox.Show(string.Format("确定要将 [{0}] 踢出公会吗", selectedItem.info.charInfo.Name), "踢出公会", MessageBoxType.Confirm).OnYes = () =>
        {
            GuildService.Instance.SendAdminCommand(GuildAdminCommand.Kickout, "", "", this.selectedItem.info.charInfo.Id);
        };
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

    /// <summary>
    /// 修改公会公告
    /// </summary>
    public void OnClickChangeNotice()
    {
        UIManager.Instance.Show<UIGuildChangeInfo>();
    }
}
