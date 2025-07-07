using Services;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGuildList : UIWindow
{
    public GameObject itemPrefab;//����Ԥ����
    public ListView listMain;//�����б�
    public Transform listRoot;//�����б���ڵ�
    public UIGuildInfo uiInfo;//������Ϣ��
    public UIGuildItem selectedItem;//��ǰѡ�еĹ���

    void Start()
    {
        this.listMain.onItemSelected += OnGuildMemberSelected;
        this.uiInfo = null;
        GuildService.Instance.OnGuildListResult += UpdateGuildList;

        GuildService.Instance.SendGuildListRequest();//�����б�����
    }

    private void UpdateGuildList(List<NGuildInfo> guilds)
    {
        ClearList();
        InitItems(guilds);
    }

    public void OnGuildMemberSelected(ListView.ListViewItem item)
    {
        this.selectedItem = item as UIGuildItem;
        this.uiInfo.Info = this.selectedItem.Info;
    }

    /// <summary>
    /// ��ʼ�������б�
    /// </summary>
    /// <param name="guilds"></param>
    private void InitItems(List<NGuildInfo> guilds)
    {
        foreach (var item in guilds)
        {
            GameObject go = Instantiate(itemPrefab, this.listMain.transform);
            UIGuildItem ui = go.GetComponent<UIGuildItem>();
            ui.SetGuildInfo(item);
            this.listMain.AddItem(ui);
        }
    }

    private void ClearList()
    {
        this.listMain.RemoveAll();
    }

    /// <summary>
    /// ���빫�ᰴť
    /// </summary>
    public void OnClickJoin()
    {
        if (this.selectedItem == null)
        {
            MessageBox.Show("��ѡ��Ҫ����Ĺ���", "����", MessageBoxType.Error);
            return;
        }
        MessageBox.Show(string.Format("ȷ��Ҫ���빫�� [{0}] ��?", selectedItem.Info.GuildName), "������빫��", MessageBoxType.Confirm).OnYes = () =>
        {
            GuildService.Instance.SendGuildJoinRequest(this.selectedItem.Info);
        };
    }
}
