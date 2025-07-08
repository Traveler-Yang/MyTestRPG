using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildIconItem : ListView.ListViewItem
{
    public Image Icon;
    public string path;
    void Start()
    {
        
    }

    public void SetIcon()
    {
        if (Icon != null) this.Icon.overrideSprite = Resources.Load<Sprite>(path);
    }
}
