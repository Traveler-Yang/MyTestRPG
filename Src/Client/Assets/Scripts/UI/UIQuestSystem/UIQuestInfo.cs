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
    public Text title;//�������

    public Text[] targets;//����Ŀ���б�

    public Text description;//��������

    public UIIconItem rewardItem;

    public TextMeshProUGUI rewarGold;//���������
    public TextMeshProUGUI rewarExp;//����������

    /// <summary>
    /// ��������������Ϣ
    /// </summary>
    /// <param name="quest"></param>
    internal void SetQuestInfo(Quest quest)
    {
        //����������Ϣ���ı���
        this.title.text = string.Format("[{0}]{1}", quest.Define.Type == QuestType.Main ? "����" : "֧��", quest.Define.Name);
        //�����������Ϣ
        //���infoΪnull�����ʾ������������ʾ����
        if (quest.Info == null)
        {
            this.description.text = quest.Define.Overview;
        }
        else
        {
            //���info��Ϊnull����������״̬���Ѿ���ɵģ�����ʾ������ɵĶԻ�
            if (quest.Info.Status == SkillBridge.Message.QuestStatus.Complated)
            {
                this.description.text = quest.Define.DialogFinish;
            }
        }
        //���ý�����Һ;���
        this.rewarGold.text = quest.Define.RewardGold.ToString();
        this.rewarExp.text = quest.Define.RewardExp.ToString();

        foreach (var fitter in GetComponentsInChildren<ContentSizeFitter>())
        {
            fitter.SetLayoutVertical();
        }
    }
}
