using Managers;
using Models;
using Services;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows;

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

    private void OnEnable()
    {
        FriendService.Instance.OnFriendUpdate += RefreshUI;
    }

    private void OnDisable()
    {
        FriendService.Instance.OnFriendUpdate -= RefreshUI;
    }

    public void OnFriendSelected(ListView.ListViewItem item)
    {
        this.selectedItem = item as UIFriendItem;
    }

    public void OnClickFriendAdd()
    {
        InputBox.Show("请输入要添加的好友的名称或ID", "添加好友").OnSubmit += OnFriendAddSubmit;
    }

    /// <summary>
    /// 添加好友事件（点击确认按钮执行的事件）
    /// </summary>
    /// <param name="input">输入的信息</param>
    /// <param name="tips">提示信息</param>
    /// <returns></returns>
    private bool OnFriendAddSubmit(string input, out string tips)
    {
        tips = "";
        int friendId = 0;
        string friendName = "";
        //因为我们输入的有可能是一个id或者是一个文本
        //所以我们要先判断这输入的内容可不可以转换成int类型
        //如果不可以转换，就将输入的信息赋值给名字
        if (!int.TryParse(input, out friendId))
            friendName = input;
        //如果可以转换
        //判断输入的内容是否是自己
        if (friendId == User.Instance.CurrentCharacter.Id || friendName == User.Instance.CurrentCharacter.Name)
        {
            tips = "不可以添加自己哦~";
            return false;
        }
        //查询好友是否存在
        foreach (var item in FriendManager.Instance.allFriends)
        {
            //如果存在，则返回，重新查询
            if (item.friendInfo.Id == friendId)
            {
                friendName = item.friendInfo.Name;
                break;
            }
            //如果没有找到，则返回false
            return false;
        }
        FriendService.Instance.SendFriendAddRequest(friendId, friendName);
        return true;
    }

    public void OnClickFriendChat()
    {
        MessageBox.Show("暂未开放");
    }

    /// <summary>
    /// 邀请组队点击事件
    /// </summary>
    public void OnClickFriendTemp()
    {
        if (selectedItem == null)//未选择不可邀请
        {
            MessageBox.Show("请选择要邀请的好友");
            return;
        }
        if (!selectedItem.info.Status)//未在线不可邀请
        {
            MessageBox.Show("请选择在线的好友");
            return;
        }
        MessageBox.Show(string.Format("确定要邀请好友 [{0}] 加入队伍吗？", selectedItem.info.friendInfo.Name), "组队邀请", MessageBoxType.Confirm, "邀请", "取消").OnYes = () =>
        {
            TempService.Instance.SendTempInviteRequest(this.selectedItem.info.friendInfo.Id, this.selectedItem.info.friendInfo.Name);
        };

    }

    /// <summary>
    /// 点击删除好友事件
    /// </summary>
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

    /// <summary>
    /// 好友搜索点击事件
    /// </summary>
    public void OnClickSearchFriend()
    {
        InputBox.Show("请输入要搜索的好友ID或昵称", "好友搜索").OnSubmit += OnFriendSearchSubmit;
    }

    /// <summary>
    /// 在当前好友列表中搜索好友
    /// </summary>
    public NFriendInfo SearchLocalFriend(string input, out string tips)
    {
        tips = "";

        if (string.IsNullOrWhiteSpace(input))
        {
            tips = "请输入有效的好友昵称或ID";
            return null;
        }

        // 排除自己
        if (input == User.Instance.CurrentCharacter.Name || input == User.Instance.CurrentCharacter.Id.ToString())
        {
            tips = "不能搜索自己哦~";
            return null;
        }
        //查询列表
        foreach (var item in FriendManager.Instance.allFriends)
        {
            if (item.friendInfo.Name == input || item.friendInfo.Id.ToString() == input)
            {
                return item;
            }
        }

        tips = "没有找到这个好友~";
        return null;
    }
    
    /// <summary>
    /// 好友搜索事件
    /// </summary>
    /// <param name="input"></param>
    /// <param name="tips"></param>
    /// <returns></returns>
    private bool OnFriendSearchSubmit(string input, out string tips)
    {
        var result = SearchLocalFriend(input, out tips);
        if (result == null)
            return false;

        // 展示 UI
        ClearFriendList();
        GameObject go = Instantiate(itemPrefab, this.listMain.transform);
        UIFriendItem ui = go.GetComponent<UIFriendItem>();
        ui.SetFriendInfo(result);
        this.listMain.AddItem(ui);

        return true;
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
        // 按照状态排序：在线的排在前面（Status == true）
        var sortedFriends = FriendManager.Instance.allFriends
                               .OrderByDescending(f => f.Status)
                               .ToList();

        foreach (var item in sortedFriends)
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
