using Common.Data;
using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIQuestSystem : UIWindow
{
    public GameObject itemPrefab;

    public TabView Tabs;
    public ListView listMain;
    public ListView listBranch;

    public UIQuestInfo questInfo;

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

            GameObject go = Instantiate(itemPrefab, kv.Value.Define.Type == QuestType.Main ? this.listMain.transform : this.listBranch.transform);
            UIQuestItem ui = go.GetComponent<UIQuestItem>();
            ui.SetQuestInfo(kv.Value);
            if (kv.Value.Define.Type == QuestType.Main)
                this.listMain.AddItem(ui);
            else
                this.listBranch.AddItem(ui);
        }
    }   

    void ClearAllQuestList()
    {
        this.listMain.RemoveAll();
        this.listBranch.RemoveAll();
    }

    public void OnQuestSelected(ListView.ListViewItem item)
    {
        UIQuestItem questItem = item as UIQuestItem;
        this.questInfo.SetQuestInfo(questItem.quest);
    }
}
