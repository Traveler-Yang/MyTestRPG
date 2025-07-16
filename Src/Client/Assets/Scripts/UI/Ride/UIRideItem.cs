using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIRideItem : ListView.ListViewItem
{
    public Image icon;//坐骑图标
    public Text title;//名字
    public TextMeshProUGUI level;//等级

    public Image backGround;//背景
    public Sprite normalBg;//正常背景
    public Sprite selectBg;//选中背景
    public override void onSelected(bool selected)
    {
        this.backGround.overrideSprite = selected ? selectBg : normalBg;
    }


    void Start()
    {
        
    }

    public Item item;

    public void SetRideItem(Item item, UIRide owner, bool equiped)
    {
        this.item = item;

        if (this.title != null) this.title.text = item.Define.Name;
        if (this.level != null) this.level.text = item.Define.Level.ToString();
        if (this.icon != null) this.icon.overrideSprite = Resources.Load<Sprite>(item.Define.Icon);
    }
}
