using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIQuestDialog : UIWindow
{
    public UIQuestInfo questInfo;

    public Quest quest;

    public GameObject openButtons;//�ɽ�����ť
    public GameObject submitButton;//�ύ����ť

    void Start()
    {
        
    }

    public void SetQuest(Quest quest)
    {
        this.quest = quest;
        this.UpdateQuest();
        //���
        if (this.quest.Info == null) 
        {
            openButtons.SetActive(true);
            submitButton.SetActive(false);
        }
        else
        {
            //�����ǰ�������Ѿ����״̬�����ύ����İ�ť��ʾ
            if (this.quest.Info.Status == SkillBridge.Message.QuestStatus.Complated)
            {
                openButtons.SetActive(false);
                submitButton.SetActive(true);
            }
            else
            {
                openButtons.SetActive(false);
                submitButton.SetActive(false);
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
