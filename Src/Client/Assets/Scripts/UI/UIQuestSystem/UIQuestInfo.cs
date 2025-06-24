using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Common.Data;

public class UIQuestInfo : MonoBehaviour
{
    public Text title;//任务标题

    public Text[] targets;//任务目标列表

    public Text description;//任务描述

    public UIIconItem[] rewardItems;//奖励物品列表

    public TextMeshProUGUI rewarGold;//任务奖励金币
    public TextMeshProUGUI rewarExp;//任务奖励经验

    private void Start()
    {
        for (int i = 0; i < rewardItems.Length; i++)
        {
            rewardItems[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 设置任务面板的信息
    /// </summary>
    /// <param name="quest"></param>
    internal void SetQuestInfo(Quest quest)
    {
        //设置任务信息面板的标题
        this.title.text = string.Format("[{0}]{1}", quest.Define.Type == QuestType.Main ? "主线" : "支线", quest.Define.Name);
        //设置任务的信息
        //如果info为null，则表示是新任务，则显示概述
        if (quest.Info == null)
        {
            this.description.text = quest.Define.Dialog;
        }
        else
        {
            //如果info不为null，并且任务状态是已经完成的，则显示任务完成的对话
            if (quest.Info.Status == SkillBridge.Message.QuestStatus.Complated)
            {
                this.description.text = quest.Define.DialogFinish;
            }
        }
        if (rewardItems[0] != null && quest.Define.RewardItem1 > 0)
        {
            rewardItems[0].gameObject.SetActive(true);
            rewardItems[0].SetMainIcon(DataManager.Instance.Items[quest.Define.RewardItem1].Icon, quest.Define.RewardItem1Count.ToString());
        }
        else
        {
            rewardItems[0].gameObject.SetActive(false);
        }

        if (rewardItems[1] != null && quest.Define.RewardItem2 > 0)
        {
            rewardItems[1].gameObject.SetActive(true);
            rewardItems[1].SetMainIcon(DataManager.Instance.Items[quest.Define.RewardItem2].Icon, quest.Define.RewardItem2Count.ToString());
        }
        else
        {
            rewardItems[1].gameObject.SetActive(false);
        }

        if (rewardItems[2] != null && quest.Define.RewardItem3 > 0)
        {
            rewardItems[2].gameObject.SetActive(true);
            rewardItems[2].SetMainIcon(DataManager.Instance.Items[quest.Define.RewardItem3].Icon, quest.Define.RewardItem3Count.ToString());
        }
        else
        {
            rewardItems[2].gameObject.SetActive(false);
        }
        //设置奖励金币和经验
        this.rewarGold.text = quest.Define.RewardGold.ToString();
        this.rewarExp.text = quest.Define.RewardExp.ToString();

        foreach (var fitter in GetComponentsInChildren<ContentSizeFitter>())
        {
            fitter.SetLayoutVertical();
        }
    }
}
