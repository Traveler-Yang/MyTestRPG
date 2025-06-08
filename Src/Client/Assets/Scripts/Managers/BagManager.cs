using Managers;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagManager : Singleton<BagManager>
{

    public int Unlocked;

    public BagItem[] items;

    NBagInfo Info;

    unsafe public void Init(NBagInfo info)
    {
        this.Info = info;
        this.Unlocked = info.Unlocked;
        items = new BagItem[this.Unlocked];
        if (info.Items != null && info.Items.Length >= this.Unlocked)
        {
            //Analyze(info.Items);
        }
        else
        {
            //info.Items = new byte[sizeof(BagItem) * this.Unlocked];
            Reset();
        }
    }

    private void Reset()
    {
        int i = 0;
        foreach (var kv in ItemManager.Instance.Items)
        {
            if (kv.Value.Count <= kv.Value.define.StackLimit)
            {
                //this.items[i].
            }
        }
    }

}
