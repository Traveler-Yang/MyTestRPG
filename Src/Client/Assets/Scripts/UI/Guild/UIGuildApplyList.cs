using Managers;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGuildApplyList : UIWindow
{
    public GameObject itemPrefab;//成员Prefab
    public ListView listMain;//成员列表
    public Transform listRoot;//成员列表根节点

    void Start()
    {
        GuildService.Instance.OnGuildUpdate += UpdateList;
        GuildService.Instance.SendGuildListRequest();
        this.UpdateList();
    }

    private void OnDestroy()
    {
        GuildService.Instance.OnGuildUpdate -= UpdateList;
    }

    public void UpdateList()
    {
        ClearList();
        InitItems();
    }

    /// <summary>
    /// 初始化成员列表
    /// </summary>
    /// <param name="guilds"></param>
    private void InitItems()
    {
        foreach (var item in GuildManager.Instance.guildInfo.Applies)
        {
            GameObject go = Instantiate(itemPrefab, this.listMain.transform);
            UIGuildApplyItem ui = go.GetComponent<UIGuildApplyItem>();
            ui.SetItemInfo(item);
            this.listMain.AddItem(ui);
        }
    }

    private void ClearList()
    {
        this.listMain.RemoveAll();
    }
}
