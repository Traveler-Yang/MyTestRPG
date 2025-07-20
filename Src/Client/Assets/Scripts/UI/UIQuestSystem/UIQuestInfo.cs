using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Common.Data;
using Managers;

public class UIQuestInfo : MonoBehaviour
{
    public Text title;//任务标题

    public Text[] targets;//任务目标列表
    public GameObject target;

    public Text description;//任务描述

    public UIIconItem[] rewardItems;//奖励物品列表
    public GameObject rewardItem;

    public TextMeshProUGUI rewarGold;//任务奖励金币
    public TextMeshProUGUI rewarExp;//任务奖励经验

    public Button navButton;//导航按钮
    private int npc = 0;

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
        if (quest.Define.Target1ID == 0)
        {
            this.target.SetActive(false);
        }
        else
        {
            this.target.SetActive(true);
            if (quest.Define.Target1 == QuestTraget.Kill)
            {
                this.targets[0].text = string.Format("消灭{0}只{1} {2}/{3}", quest.Define.Target1Num, DataManager.Instance.Characters[quest.Define.Target1ID].Name, 1, quest.Define.Target1Num);
            }
            if (quest.Define.Target1 == QuestTraget.Item)
            {
                this.targets[0].text = "将信件送到琴那里";
            }
            this.targets[1].text = "";
            this.targets[2].text = "";
        }
        SetRewardItems(quest);
        //设置奖励金币和经验
        this.rewarGold.text = quest.Define.RewardGold.ToString();
        this.rewarExp.text = quest.Define.RewardExp.ToString();

        if (quest.Info == null)
        {
            //如果info为null，则表示是新任务，则显示接受NPC
            this.npc = quest.Define.AcceptNPC;
        }
        else if (quest.Info.Status == SkillBridge.Message.QuestStatus.Complated)
        {
            //如果info不为null，并且任务状态是已经完成的，则显示提交NPC
            this.npc = quest.Define.SubmitNPC;
        }
        this.navButton.gameObject.SetActive(this.npc > 0);

        foreach (var fitter in GetComponentsInChildren<ContentSizeFitter>())
        {
            fitter.SetLayoutVertical();
        }
    }

    public void OnClickAbandon()
    {

    }

    public void OnClickNav()
    {
        Vector3 pos = NPCManager.Instance.GetNpcPosition(this.npc);
        User.Instance.CurrentCharacterObject.StartNav(pos);
        UIManager.Instance.Close<UIQuestSystem>();
    }

    /// <summary>
    /// 设置任务奖励信息
    /// </summary>
    private void SetRewardItems(Quest quest)
    {
        int[] rewardItemIds = new int[] {
        quest.Define.RewardItem1,
        quest.Define.RewardItem2,
        quest.Define.RewardItem3
    };
        int[] rewardItemCounts = new int[] {
        quest.Define.RewardItem1Count,
        quest.Define.RewardItem2Count,
        quest.Define.RewardItem3Count
    };

        for (int i = 0; i < rewardItems.Length; i++)
        {
            if (rewardItems[i] == null) continue;

            if (i < rewardItemIds.Length && rewardItemIds[i] > 0)
            {
                var itemDefine = DataManager.Instance.Items[rewardItemIds[i]];
                rewardItems[i].gameObject.SetActive(true);
                rewardItems[i].SetMainIcon(itemDefine.Icon, rewardItemCounts[i].ToString());
            }
            else
            {
                rewardItems[i].gameObject.SetActive(false);
            }
        }
    }

}
