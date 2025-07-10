using Common.Data;
using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIQuestSystem : UIWindow
{
    public GameObject itemPrefab;//任务项预制体

    public TabView Tabs;//Tab按钮
    public ListView listMain;// 主线任务列表
    public ListView listBranch;// 支线任务列表

    public UIQuestInfo questInfo;// 任务信息显示组件

    private bool showAvailableList = false;

    
    void Start()
    {
        this.listMain.onItemSelected += OnQuestSelected;
        this.listBranch.onItemSelected += OnQuestSelected;
        this.Tabs.OnTabSelect += OnSelectTab;
        RefreshUI();

        OnSelectTab(1);
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
    /// 初始化任务项列表
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
                if (kv.Value.Info == null)
                    continue;
            }
            if (kv.Value.Info != null)
            {
                if (kv.Value.Info.Status == SkillBridge.Message.QuestStatus.Finished)
                    continue;
            }
            //加载任务项的预制体，并根据任务类型添加到对应的列表中
            GameObject go = Instantiate(itemPrefab, kv.Value.Define.Type == QuestType.Main ? this.listMain.transform : this.listBranch.transform);
            UIQuestItem ui = go.GetComponent<UIQuestItem>();
            //设置任务项的信息
            ui.SetQuestInfo(kv.Value);
            //如果任务是主线任务，则添加到主线任务列表，否则添加到支线任务列表
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
        //设置当前选择的任务项的信息到Info中
        UIQuestItem questItem = item as UIQuestItem;
        if (questItem == null || questItem.quest == null)
        {
            Debug.LogWarning("任务项为空或未设置任务数据");
            return;
        }

        this.questInfo.SetQuestInfo(questItem.quest);
    }
}
