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

    public UIIconItem[] rewardItems;//������Ʒ�б�

    public TextMeshProUGUI rewarGold;//���������
    public TextMeshProUGUI rewarExp;//����������

    private void Start()
    {
        for (int i = 0; i < rewardItems.Length; i++)
        {
            rewardItems[i].gameObject.SetActive(false);
        }
    }

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
            this.description.text = quest.Define.Dialog;
        }
        else
        {
            //���info��Ϊnull����������״̬���Ѿ���ɵģ�����ʾ������ɵĶԻ�
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
        //���ý�����Һ;���
        this.rewarGold.text = quest.Define.RewardGold.ToString();
        this.rewarExp.text = quest.Define.RewardExp.ToString();

        foreach (var fitter in GetComponentsInChildren<ContentSizeFitter>())
        {
            fitter.SetLayoutVertical();
        }
    }
}
