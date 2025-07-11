using Services;
using SkillBridge.Message;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildList : UIWindow
{
    public GameObject itemPrefab;//公会预制体
    public ListView listMain;//公会列表
    public Transform listRoot;//公会列表根节点
    public UIGuildInfo uiInfo;//公会信息栏
    public UIGuildItem selectedItem;//当前选中的公会
    public InputField searchInput;//搜索输入框

    void Start()
    {
        this.listMain.onItemSelected += OnGuildMemberSelected;
        this.uiInfo.Info = null;
        GuildService.Instance.OnGuildListResult += UpdateGuildList;//更新列表事件

        GuildService.Instance.OnGuildSearchResult += OnSearchEvent;//搜索事件
        GuildService.Instance.SendGuildListRequest();//发送列表请求
    }

    private void OnDisable()
    {
        GuildService.Instance.OnGuildListResult -= UpdateGuildList;
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
    /// 初始化公会列表
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
    /// 搜索公会按钮
    /// </summary>
    public void OnClickSearch()
    {
        if (this.searchInput.text.Length > 0)
        {
            GuildService.Instance.SendGuildSearch(searchInput.text);
        }
        else
        {
            MessageBox.Show("请输入要搜索的公会名", "搜索公会", MessageBoxType.Information);
        }
    }

    /// <summary>
    /// 公会搜索成功事件
    /// </summary>
    /// <param name="guild"></param>
    public void OnSearchEvent(NGuildInfo guild)
    {
        ClearList();
        GameObject go = Instantiate(itemPrefab, this.listMain.transform);
        UIGuildItem ui = go.GetComponent<UIGuildItem>();
        ui.SetGuildInfo(guild);
        this.listMain.AddItem(ui);
    }

    /// <summary>
    /// 加入公会按钮
    /// </summary>
    public void OnClickJoin()
    {
        if (this.selectedItem == null)
        {
            MessageBox.Show("请选择要加入的公会", "错误", MessageBoxType.Error);
            return;
        }
        MessageBox.Show(string.Format("确定要加入公会 [{0}] 吗?", selectedItem.Info.GuildName), "申请加入公会", MessageBoxType.Confirm).OnYes = () =>
        {
            GuildService.Instance.SendGuildJoinRequest(this.selectedItem.Info.Id);
        };
    }
}
