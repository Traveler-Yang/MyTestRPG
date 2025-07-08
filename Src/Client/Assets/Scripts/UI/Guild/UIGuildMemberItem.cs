using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildMemberItem : ListView.ListViewItem
{
    public Text nickName;//�ǳ�
    public TextMeshProUGUI @class;//ְҵ
    public TextMeshProUGUI level;//�ȼ�
    public Text duty;//ְ��
    public Text status;//״̬

    public Image backGround;//����
    public Sprite normalBg;//��������
    public Sprite selectBg;//ѡ�б���
    public override void onSelected(bool selected)
    {
        this.backGround.overrideSprite = selected ? selectBg : normalBg;
    }

    public NGuildMemberInfo info;
    public void SetGuildMemberInfo(NGuildMemberInfo info)
    {
        if (this.nickName != null) this.nickName.text = this.info.charInfo.Name;
        if (this.@class != null) this.@class.text = this.info.charInfo.Class.ToString();
        if (this.level != null) this.level.text = this.info.charInfo.Level.ToString();
        if (this.duty != null) this.duty.text = this.info.Duty.ToString();
        if (this.status != null) this.status.text = this.info.Status.ToString();
    }

    /// <summary>
    /// �����ϸ��Ϣ
    /// </summary>
    public void OnclickDetails()
    {

    }
}
