using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UITempItem : ListView.ListViewItem
{
    public Text nickName;//�ǳ�
    public Image classIcon;//ְҵͼ��
    public TextMeshProUGUI level;//�ȼ�
    public Image leaderIcon;//�ӳ�ͼ��

    public Image backGround;//����
    public override void onSelected(bool selected)
    {
        this.backGround.enabled = selected ? true : false;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
