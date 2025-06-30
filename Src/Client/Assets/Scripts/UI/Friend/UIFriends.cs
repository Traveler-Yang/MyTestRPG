using Managers;
using Models;
using Services;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFriends : UIWindow
{
    public GameObject itemPrefab;//好友项预制体
    public ListView listMain;//好友列表框
    public Transform itemRoot;//好友列表根节点
    public UIFriendItem selectedItem;//当前选中的是谁

    void Start()
    {
        FriendService.Instance.OnFriendUpdate = RefreshUI;
        this.listMain.onItemSelected += OnFriendSelected;
        RefreshUI();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnFriendSelected(ListView.ListViewItem item)
    {
        this.selectedItem = item as UIFriendItem;
    }

    public void OnClickFriendAdd()
    {
        InputBox.Show("请输入要添加的好友的名称或ID", "添加好友").OnSubmit += OnFriendAddSubmit;
    }

    private bool OnFriendAddSubmit(string input, out string tips)
    {
        tips = "";
        int friendId = 0;
        string friendName = "";
        //因为我们输入的有可能是一个id或者是一个文本
        //所以我们要先判断这输入的内容可不可以转换成int类型
        if (!int.TryParse(input, out friendId))
            friendName = input;
        //判断输入的内容是否是自己
        if (friendId == User.Instance.CurrentCharacter.Id || friendName == User.Instance.CurrentCharacter.Name)
        {
            tips = "不可以添加自己哦~";
            return false;
        }
        friendName = CharacterManager.Instance.Characters[friendId].Name;
        FriendService.Instance.SendFriendAddRequest(friendId, friendName);
        return true;
    }

    public void OnClickFriendChat()
    {
        MessageBox.Show("暂未开放");
    }

    public void OnClickFriendRemove()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请选择要删除的好友");
            return;
        }
        MessageBox.Show(string.Format("确定要删除[{0}]好友吗", selectedItem.info.friendInfo.Name), "删除好友", MessageBoxType.Confirm, "删除", "取消").OnYes = () =>
        {
            FriendService.Instance.SendFriendRemoveRequest(selectedItem.info.Id, selectedItem.info.friendInfo.Id);
        };
    }

    void RefreshUI()
    {
        ClearFriendList();
        InitFriendItems();
    }

    /// <summary>
    /// 初始化所有好友列表
    /// </summary>
    private void InitFriendItems()
    {
        foreach (var item in FriendManager.Instance.allFriends)
        {
            GameObject go = Instantiate(itemPrefab, this.listMain.transform);
            UIFriendItem ui = go.GetComponent<UIFriendItem>();
            ui.SetFriendInfo(item);
            this.listMain.AddItem(ui);
        }
    }

    /// <summary>
    /// 清除好友列表
    /// </summary>
    private void ClearFriendList()
    {
        this.listMain.RemoveAll();
    }
}
