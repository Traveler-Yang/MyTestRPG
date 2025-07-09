using Managers;
using Services;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGuild : UIWindow
{
    public GameObject itemPrefab;//��ԱPrefab
    public ListView listMain;//��Ա�б�
    public Transform listRoot;//��Ա�б���ڵ�
    public UIGuildInfo uiInfo;//������Ϣ��
    public UIGuildMemberItem selectedItem;//��ǰѡ�еĳ�Ա
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
    /// ��ʼ����Ա�б�
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
    /// ת��
    /// </summary>
    public void OnClickTransfer()
    {
        MessageBox.Show("��δ����");
    }

    /// <summary>
    /// ��ְ
    /// </summary>
    public void OnClickPromotion()
    {
        MessageBox.Show("��δ����");
    }

    /// <summary>
    /// ����
    /// </summary>
    public void OnClickRecall()
    {
        MessageBox.Show("��δ����");
    }

    /// <summary>
    /// �����б�
    /// </summary>
    public void OnClickRequestList()
    {
        MessageBox.Show("��δ����");
    }

    /// <summary>
    /// �߳�����
    /// </summary>
    public void OnClickKickOut()
    {
        MessageBox.Show("��δ����");
    }

    /// <summary>
    /// ˽��
    /// </summary>
    public void OnClickChat()
    {
        MessageBox.Show("��δ����");
    }

    /// <summary>
    /// �˳�����
    /// </summary>
    public void OnClickLeave()
    {
        MessageBox.Show(string.Format("ȷ��Ҫ�˳����� [{0}] ��", GuildManager.Instance.guildInfo.GuildName), "�˳�����", MessageBoxType.Confirm).OnYes = () =>
        {
            GuildService.Instance.SendGuildLeaveRequest();
        };
    }
}
