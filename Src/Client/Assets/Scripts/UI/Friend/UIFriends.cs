using Managers;
using Models;
using Services;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFriends : UIWindow
{
    public GameObject itemPrefab;//������Ԥ����
    public ListView listMain;//�����б��
    public Transform itemRoot;//�����б���ڵ�
    public UIFriendItem selectedItem;//��ǰѡ�е���˭

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
        InputBox.Show("������Ҫ��ӵĺ��ѵ����ƻ�ID", "��Ӻ���").OnSubmit += OnFriendAddSubmit;
    }

    private bool OnFriendAddSubmit(string input, out string tips)
    {
        tips = "";
        int friendId = 0;
        string friendName = "";
        //��Ϊ����������п�����һ��id������һ���ı�
        //��������Ҫ���ж�����������ݿɲ�����ת����int����
        if (!int.TryParse(input, out friendId))
            friendName = input;
        //�ж�����������Ƿ����Լ�
        if (friendId == User.Instance.CurrentCharacter.Id || friendName == User.Instance.CurrentCharacter.Name)
        {
            tips = "����������Լ�Ŷ~";
            return false;
        }
        friendName = CharacterManager.Instance.Characters[friendId].Name;
        FriendService.Instance.SendFriendAddRequest(friendId, friendName);
        return true;
    }

    public void OnClickFriendChat()
    {
        MessageBox.Show("��δ����");
    }

    public void OnClickFriendRemove()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("��ѡ��Ҫɾ���ĺ���");
            return;
        }
        MessageBox.Show(string.Format("ȷ��Ҫɾ��[{0}]������", selectedItem.info.friendInfo.Name), "ɾ������", MessageBoxType.Confirm, "ɾ��", "ȡ��").OnYes = () =>
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
    /// ��ʼ�����к����б�
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
    /// ��������б�
    /// </summary>
    private void ClearFriendList()
    {
        this.listMain.RemoveAll();
    }
}
