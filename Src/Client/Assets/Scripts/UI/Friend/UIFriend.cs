using Entities;
using Managers;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFriend : UIWindow
{
    public ListView friendList;//好友列表
    public GameObject friendItemPrefab;//好友预制体
    public UIFriendItem selected;//选中的好友


    private void Start()
    {
        FriendService.Instance.UpdateFriendList = RefreshUI;
        friendList.onItemSelected += OnSelectedItem;
        this.RefreshUI();
    }

    private void OnEnable()
    {
        MouseControl.Instance.OnUIOpen();
        FriendService.Instance.UpdateFriendList += RefreshUI;
    }

    private void OnDisable()
    {
        MouseControl.Instance.OnUIClose();
        FriendService.Instance.UpdateFriendList -= RefreshUI;
    }

    private void OnSelectedItem(ListView.ListViewItem item)
    {
        this.selected = item as UIFriendItem;
    }

    public void OnAddFriendClick()
    {
        InputBox.Show("请输入要添加的好友ID", "添加好友").OnSubmit += AddFriendOnSubmit;
    }

    private bool AddFriendOnSubmit(string inputText, out string tips)
    {
        int id = 0;
        string name = "";
        //如果可以转换为整形，则用id
        if (inputText != "")
        {
            if (int.TryParse(inputText, out id))
            {
                if (User.Instance.Info.Id == id)
                {
                    tips = "不可以添加自己哦~";
                    return false;
                }
                FriendService.Instance.SendFriendAddRequest(id, CharacterManager.Instance.GetCharacter(id)?.Info.Name);
                tips = "发送请求成功！";
                return true;
            }
            name = inputText;
            Character cha = CharacterManager.Instance.GetCharacterByName(name);
            if (cha != null)
            {
                FriendService.Instance.SendFriendAddRequest(cha.Info.Id, name);
                tips = "发送请求成功！";
                return true;
            }
        }
        tips = "请输入要搜索的好友ID";
        return false;
    }

    public void OnChatClick()
    {
        if (this.selected != null)
        {
            MessageBox.Show("暂未开放", "私聊");
            return;
        }
        MessageBox.Show("请选择要私聊的好友", "私聊");
    }

    public void OnFriendRemoveClick()
    {
        if (this.selected != null)
        {
            var msgBox = MessageBox.Show("确定要删除该好友吗？", "删除好友", MessageBoxType.Confirm);
            msgBox.OnYes = () =>
            {
                FriendService.Instance.SendFriendRemoveReq(this.selected.info.Info.Id);
            };
            return;
        }
        MessageBox.Show("请选择要删除的好友", "删除好友");
    }

    public void OnLookFriendInfoClick()
    {
        if (this.selected != null)
        {
            MessageBox.Show("暂未开放", "删除好友");
            return;
        }
        MessageBox.Show("请选择要查看的好友", "查看信息");
    }

    public void RefreshUI()
    {
        ClearFriend();
        InitFriend();
    }

    public void InitFriend()
    {
        foreach (var f in FriendManager.Instance.allFriend)
        {
            GameObject go = Instantiate(friendItemPrefab, friendList.transform);
            UIFriendItem item =  go.GetComponent<UIFriendItem>();
            item.owner = this.friendList;
            item.SetFriendInfo(f);
            this.friendList.AddItem(item);
        }
    }

    public void ClearFriend()
    {
        this.friendList.RemoveAll();
    }
}
