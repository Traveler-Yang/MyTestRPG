using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SkillBridge.Message;
using Assets.Scripts.UI.Temp;

public class UITempItem : ListView.ListViewItem
{
    public Text nickName;//�ǳ�
    public Image classIcon;//ְҵͼ��
    public TextMeshProUGUI level;//�ȼ�
    public Image leaderIcon;//�ӳ�ͼ��

    public Image backGround;//����
    public override void onSelected(bool selected)
    {
        //�Ƿ���ʾ
        this.backGround.enabled = selected ? true : false;
    }

    public int idx;
    public NCharacterInfo Info;

    void Start()
    {
        
    }

    /// <summary>
    /// ���ö����Ա����Ϣ
    /// </summary>
    /// <param name="idx">����</param>
    /// <param name="info">��ɫ��Ϣ</param>
    /// <param name="isLeader">�Ƿ��Ƕӳ�</param>
    public void SetMemberInfo(int idx, NCharacterInfo info, bool isLeader)
    {
        this.idx = idx;
        this.Info = info;
        if (this.nickName != null) this.nickName.text = this.Info.Name;
        if (this.classIcon != null) this.classIcon.overrideSprite = SpriteManager.Instance.classIcons[(int)this.Info.Class - 1];
        if (this.level != null) this.level.text = this.Info.Level.ToString();
        this.leaderIcon.gameObject.SetActive(isLeader ? true : false);
    }
}
