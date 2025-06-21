using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIQuestDialog : UIWindow
{
    public UIQuestInfo questInfo;

    public Quest quest;

    public GameObject openButtons;//可接任务按钮
    public GameObject submitButton;//提交任务按钮

    void Start()
    {
        
    }

    public void SetQuest(Quest quest)
    {
        this.quest = quest;
        this.UpdateQuest();
        //判断当前任务是否是新任务，如果是新任务，则显示可接任务按钮，否则显示提交任务按钮
        //info为null，则表示是新任务
        if (this.quest.Info == null) 
        {
            openButtons.SetActive(true);
            submitButton.SetActive(false);
        }
        else
        {
            //如果当前任务是已经完成状态，则将提交任务的按钮显示
            if (this.quest.Info.Status == SkillBridge.Message.QuestStatus.Complated)
            {
                openButtons.SetActive(false);
                submitButton.SetActive(true);
            }
            else
            {
                openButtons.SetActive(false);
                submitButton.SetActive(false);
                MessageBox.Show("任务出现问题", "错误", MessageBoxType.Error);
            }
        }
    }

    void UpdateQuest()
    {
        if (this.quest != null)
        {
            if (this.questInfo != null)
            {
                questInfo.SetQuestInfo(this.quest);
            }
        }
    }
}
