using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Services;

public class UITemp : MonoBehaviour
{
    public Text tempTitle;//标题
    public UITempItem[] members;//队伍成员列表
    public ListView list;//列表
   
    void Start()
    {
        //如果当前队伍界面打开了，判断当前角色身上是否有队伍
        if (User.Instance.TempInfo == null)
        {
            //如果没有队伍，则隐藏
            this.gameObject.SetActive(false);
            return;
        }
        //start时，将队伍的成员添加到List列表中
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
    /// 显示组队界面
    /// </summary>
    /// <param name="show">是否显示</param>
    public void ShowTeam(bool show)
    {
        this.gameObject.SetActive(show);
        if (show)
            UpdateTempUI();
    }

    /// <summary>
    /// 更新队伍信息
    /// </summary>
    public void UpdateTempUI()
    {
        if (User.Instance.TempInfo == null) return;
        //设置标题
        this.tempTitle.text = string.Format("我的队伍({0}/5)", User.Instance.TempInfo.Members.Count);
        //显示队伍成员
        for (int i = 0; i < 5; i++)
        {
            //每次进来判断是否小于当前队伍的最大数量
            //如果队伍数量大于了 i ，则说明队伍有多少人
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
    /// 退出队伍点击事件
    /// </summary>
    public void OnClickLeave()
    {
        MessageBox.Show("确定要离开队伍吗？", "退出队伍", MessageBoxType.Confirm, "离开", "取消").OnYes = () =>
        {
            TempService.Instance.SendTempLeaveRequest(User.Instance.TempInfo.Id);
        };
    }
}
