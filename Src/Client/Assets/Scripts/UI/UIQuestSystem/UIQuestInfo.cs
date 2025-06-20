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
    public Text title;

    public Text[] targets;

    public Text description;

    public UIIconItem rewardItem;

    public TextMeshProUGUI rewarGold;
    public TextMeshProUGUI rewarExp;

    /// <summary>
    /// 设置任务面板的信息
    /// </summary>
    /// <param name="quest"></param>
    internal void SetQuestInfo(Quest quest)
    {
        this.title.text = string.Format("[{0}]{1}", quest.Define.Type, quest.Define.Name);
        if(quest.Info == null)
        {
            this.description.text = quest.Define.Dialog;
        }
        else
        {
            if (quest.Info.Status == SkillBridge.Message.QuestStatus.Complated)
            {
                this.description.text = quest.Define.DialogFinish;
            }
        }

        this.rewarGold.text = quest.Define.RewardGold.ToString();
        this.rewarExp.text = quest.Define.RewardExp.ToString();

        foreach (var fitter in GetComponentsInChildren<ContentSizeFitter>())
        {
            fitter.SetLayoutVertical();
        }
    }
}
