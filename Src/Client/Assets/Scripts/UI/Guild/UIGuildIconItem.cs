using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildIconItem : ListView.ListViewItem
{
    public int id;
    public Image Icon;

    public override void onSelected(bool selected)
    {
        base.onSelected(selected);
    }

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        if (Icon != null)
        {
            this.Icon.overrideSprite = Resources.Load<Sprite>(DataManager.Instance.Icons[this.id].Icon);
            this.Icon.color = Color.white;
        }

    }
}
