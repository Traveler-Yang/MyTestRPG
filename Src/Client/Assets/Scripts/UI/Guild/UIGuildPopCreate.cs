using Services;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildPopCreate : UIWindow
{
    public ListView listMain;//ͼ���б�
    public InputField inputName;//����
    public InputField inputNotice;//���
    public UIGuildIconItem guildIconItem;//��ǰѡ���ͼ��

    void Start()
    {
        GuildService.Instance.OnGuildCreateResult = OnGuildCreated;
        this.listMain.onItemSelected += OnIconSelected;
    }

    private void OnIconSelected(ListView.ListViewItem item)
    {
        this.guildIconItem = item as UIGuildIconItem;
    }

    private void OnDestroy()
    {
        GuildService.Instance.OnGuildCreateResult = null;
    }

    public override void OnYesClick()
    {
        if (string.IsNullOrEmpty(this.inputName.text))
        {
            MessageBox.Show("�����빫������", "����", MessageBoxType.Error);
            return;
        }
        if (this.inputName.text.Length < 2 || this.inputName.text.Length > 8)
        {
            MessageBox.Show("��������Ӧ��2-8���ַ�֮��", "����", MessageBoxType.Error);
            return;
        }
        if (string.IsNullOrEmpty(this.inputNotice.text))
        {
            MessageBox.Show("�����빫����", "����", MessageBoxType.Error);
            return;
        }
        if (this.inputNotice.text.Length < 5 || this.inputNotice.text.Length > 30)
        {
            MessageBox.Show("������Ӧ��5-30���ַ�֮��", "����", MessageBoxType.Error);
            return;
        }

        GuildService.Instance.SendGuildCreate(this.inputName.text, this.inputNotice.text, guildIconItem.path);
    }

    void OnGuildCreated(bool result)
    {
        if (result)
            this.Close(UIWindowResult.Yes);
    }
}
