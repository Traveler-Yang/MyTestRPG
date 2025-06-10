using Managers;
using Models;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
class BagManager : Singleton<BagManager>
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
            Analyze(info.Items);
        }
        else
        {
            info.Items = new byte[sizeof(BagItem) * this.Unlocked];
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
                this.items[i].ItemId = (ushort)kv.Key;
                this.items[i].Count = (ushort)kv.Value.Count;
            }
            else
            {
                int count = kv.Value.Count;
                while (count > kv.Value.define.StackLimit)
                {

                }
            }
        }
    }

    unsafe void Analyze(byte[] data)
    {
        //声明一个byte类型的指针，并指向data数组的第一位的内存地址
        fixed (byte* pt = data)
        {
            //遍历背包的所有格子
            for (int i = 0; i < Unlocked; i++)
            {
                //sizeof(BagItem)是当前物品在内存中所占的字节数
                //i * sizeof(BagItem) 所计算的是第i个物品在这块内存中的起始偏移
                //也就是需要跳多大的内存
                //和前面的(BagItem*)(pt + i * sizeof(BagItem))组合起来
                //pt就是指向背包数组的第一位的地址
                //pt + i是要跳过多少个字节块，而后面的i * sizeof(BagItem)就是需要跳过的字节大小
                //组合起来就是从从第一位地址开始，跳过多少个 BagItem大小的 字节块
                //并将得到的这块内存，转换(强转)成 BagItem* 类型，存储起来
                BagItem* item = (BagItem*)(pt + i * sizeof(BagItem));
                //这行是将上面存储的内存，放入背包的格子
                items[i] = *item;
            }
        }
    }

    unsafe public NBagInfo GetBagInfo()
    {
        //这行同上
        fixed (byte* pt = Info.Items)
        {
            for (int i = 0; i < Unlocked; i++)
            {
                BagItem* item = (BagItem*)(pt + i * sizeof(BagItem));
                items[i] = *item;
            }
        }
        return this.Info;
    }
}
