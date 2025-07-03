using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Services;

public class UITemp : MonoBehaviour
{
    public Text tempTitle;//����
    public UITempItem[] members;//�����Ա�б�
    public ListView list;//�б�
   
    void Start()
    {
        //�����ǰ���������ˣ��жϵ�ǰ��ɫ�����Ƿ��ж���
        if (User.Instance.TempInfo == null)
        {
            //���û�ж��飬������
            this.gameObject.SetActive(false);
            return;
        }
        //startʱ��������ĳ�Ա��ӵ�List�б���
        foreach (var item in members)
        {
            this.list.AddItem(item);
        }
    }

    private void OnEnable()
    {
        UpdateTempUI();
    }

    /// <summary>
    /// ��ʾ��ӽ���
    /// </summary>
    /// <param name="show">�Ƿ���ʾ</param>
    public void ShowTeam(bool show)
    {
        this.gameObject.SetActive(show);
        if (show)
            UpdateTempUI();
    }

    /// <summary>
    /// ���¶�����Ϣ
    /// </summary>
    public void UpdateTempUI()
    {
        if (User.Instance.TempInfo == null) return;
        //���ñ���
        this.tempTitle.text = string.Format("�ҵĶ���({0}/5)", User.Instance.TempInfo.Members.Count);
        //��ʾ�����Ա
        for (int i = 0; i < 5; i++)
        {
            //ÿ�ν����ж��Ƿ�С�ڵ�ǰ������������
            //����������������� i ����˵�������ж�����
            if (i < User.Instance.TempInfo.Members.Count)
            {
                this.members[i].SetMemberInfo(i, User.Instance.TempInfo.Members[i], User.Instance.TempInfo.Members[i].Id == User.Instance.TempInfo.Leader);
                this.members[i].gameObject.SetActive(true);
            }
            else
            {
                this.members[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// �˳��������¼�
    /// </summary>
    public void OnClickLeave()
    {
        MessageBox.Show("ȷ��Ҫ�뿪������", "�˳�����", MessageBoxType.Confirm, "�뿪", "ȡ��").OnYes = () =>
        {
            TempService.Instance.SendTempLeaveRequest(User.Instance.TempInfo.Id);
        };
    }
}
