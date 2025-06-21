using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestItem : ListView.ListViewItem
{
    public Text title;//任务标题

    public Image backGround;//任务项背景
    public Sprite normalBg;//任务项正常背景
    public Sprite selectBg;//任务项选中背景

    public override void onSelected(bool selected)
    {
        this.backGround.overrideSprite = selected ? selectBg : normalBg;
    }

    public Quest quest;
    void Start()
    {

    }

    bool isEquiped = false;

    /// <summary>
    /// 设置任务项的信息
    /// </summary>
    /// <param name="item"></param>
    public void SetQuestInfo(Quest item)
    {
        this.quest = item;
        if (this.title != null) this.title.text = quest.Define.Name;
    }
}
