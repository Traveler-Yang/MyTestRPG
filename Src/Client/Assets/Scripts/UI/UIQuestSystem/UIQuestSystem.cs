using Common.Data;
using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIQuestSystem : UIWindow
{
    public GameObject itemPrefab;//������Ԥ����

    public TabView Tabs;//Tab��ť
    public ListView listMain;// ���������б�
    public ListView listBranch;// ֧�������б�

    public UIQuestInfo questInfo;// ������Ϣ��ʾ���

    private bool showAvailableList = false;

    
    void Start()
    {
        this.listMain.onItemSelected += OnQuestSelected;
        this.listBranch.onItemSelected += OnQuestSelected;
        this.Tabs.OnTabSelect += OnSelectTab;
        RefreshUI();
    }


    void OnSelectTab(int idx)
    {
        showAvailableList = idx == 1;
        RefreshUI();
    }

    private void OnDestroy()
    {
        
    }

    void RefreshUI()
    {
        ClearAllQuestList();
        InitAllQuestItems();
    }

    /// <summary>
    /// ��ʼ���������б�
    /// </summary>
    private void InitAllQuestItems()
    {
        foreach (var kv in QuestManager.Instance.allQuests)
        {
            if (showAvailableList)
            {
                if (kv.Value.Info != null)
                    continue;
            }
            else
            {
                if (kv.Value.Define == null)
                    continue;
            }
            //�����������Ԥ���壬����������������ӵ���Ӧ���б���
            GameObject go = Instantiate(itemPrefab, kv.Value.Define.Type == QuestType.Main ? this.listMain.transform : this.listBranch.transform);
            UIQuestItem ui = go.GetComponent<UIQuestItem>();
            //�������������Ϣ
            ui.SetQuestInfo(kv.Value);
            //���������������������ӵ����������б�������ӵ�֧�������б�
            //if (kv.Value.Define.Type == QuestType.Main)
            //    this.listMain.AddItem(ui as ListView.ListViewItem);
            //else
            //    this.listBranch.AddItem(ui as ListView.ListViewItem);
            this.listMain.AddItem(ui);
        }
    }   

    void ClearAllQuestList()
    {
        this.listMain.RemoveAll();
        this.listBranch.RemoveAll();
    }

    public void OnQuestSelected(ListView.ListViewItem item)
    {
        //���õ�ǰѡ������������Ϣ��Info��
        UIQuestItem questItem = item as UIQuestItem;
        this.questInfo.SetQuestInfo(questItem.quest);
    }
}
