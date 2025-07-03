using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UITempItem : ListView.ListViewItem
{
    public Text nickName;//昵称
    public Image classIcon;//职业图标
    public TextMeshProUGUI level;//等级
    public Image leaderIcon;//队长图标

    public Image backGround;//背景
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
