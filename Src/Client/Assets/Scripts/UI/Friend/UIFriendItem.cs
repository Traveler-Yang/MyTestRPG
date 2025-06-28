using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SkillBridge.Message;

public class UIFriendItem : ListView.ListViewItem
{
    public Image icon;//ͷ��ͼ��
    public Text nickName;//�ǳ�
    public TextMeshProUGUI @class;//ְҵ
    public TextMeshProUGUI Level;//�ȼ�
    public Text status;//״̬

    public Image backGround;//����
    public Sprite normalBg;//��������
    public Sprite selectBg;//ѡ�б���
    public override void onSelected(bool selected)
    {
        this.backGround.overrideSprite = selected ? selectBg : normalBg;
    }

    public NFriendInfo info;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetFriendInfo(NFriendInfo item)
    {
        this.info = item;
        if (this.nickName != null) this.nickName.text = this.info.friendInfo.Name;
        if (this.@class != null) this.@class.text = this.info.friendInfo.Class.ToString();
        if (this.Level != null) this.Level.text = this.info.friendInfo.Level.ToString();
        if (this.status != null) this.status.text = this.info.Status == true ? "����" : "����";
    }
}
