using Services;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildPopCreate : UIWindow
{
    public ListView listMain;//图标列表
    public InputField inputName;//名字
    public InputField inputNotice;//简介
    public UIGuildIconItem[] iconItems;
    public UIGuildIconItem selectIconItem;//当前选择的图标

    void Start()
    {
        GuildService.Instance.OnGuildCreateResult = OnGuildCreated;
        this.listMain.onItemSelected += OnIconSelected;
        InitIcons();
    }

    private void OnIconSelected(ListView.ListViewItem item)
    {
        this.selectIconItem = item as UIGuildIconItem;
    }

    private void OnDestroy()
    {
        GuildService.Instance.OnGuildCreateResult = null;
        this.selectIconItem = null;
    }

    public override void OnYesClick()
    {
        if (string.IsNullOrEmpty(this.inputName.text))
        {
            MessageBox.Show("请输入公会名称", "错误", MessageBoxType.Error);
            return;
        }
        if (this.inputName.text.Length < 2 || this.inputName.text.Length > 8)
        {
            MessageBox.Show("公会名称应在2-8个字符之内", "错误", MessageBoxType.Error);
            return;
        }
        if (string.IsNullOrEmpty(this.inputNotice.text))
        {
            MessageBox.Show("请输入公会简介", "错误", MessageBoxType.Error);
            return;
        }
        if (this.inputNotice.text.Length < 5 || this.inputNotice.text.Length > 30)
        {
            MessageBox.Show("公会简介应在5-30个字符之内", "错误", MessageBoxType.Error);
            return;
        }
        if (this.selectIconItem == null)
        {
            MessageBox.Show("请选择公会图标", "错误", MessageBoxType.Error);
            return;
        }

        GuildService.Instance.SendGuildCreate(this.inputName.text, this.inputNotice.text, DataManager.Instance.Icons[selectIconItem.id].Icon);
    }

    public void InitIcons()
    {
        for (int i = 0; i < this.iconItems.Length; i++)
        {
            iconItems[i].Init();
            this.listMain.AddItem(iconItems[i]);
        }
    }

    void OnGuildCreated(bool result)
    {
        if (result)
            this.Close(UIWindowResult.Yes);
    }
}
