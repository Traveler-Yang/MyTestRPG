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

    public void AddItem(int itemId, int count)
    {
        ushort addCount = (ushort)count;
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].ItemId == itemId)
            {
                //可以增加的数量
                ushort canAdd = (ushort)(DataManager.Instance.Items[itemId].StackLimit - this.items[i].Count);
                if (canAdd >= addCount)
                {
                    this.items[i].Count += addCount;
                    addCount = 0;
                    break;
                }
                else
                {
                    this.items[i].Count += canAdd;
                    addCount -= canAdd;
                }
            }
        }
        if (addCount > 0)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (this.items[i].ItemId == 0)
                {
                    this.items[i].ItemId = (ushort)itemId;
                    this.items[i].Count = addCount;
                    break;
                }
            }
        }
    }

    public void RemoveItem(int itemId, int count)
    {

    }

    /// <summary>
    /// 整理
    /// </summary>
    public void Reset()
    {
        int i = 0;
        foreach (var kv in ItemManager.Instance.Items)
        {
            //如果当前道具的数量小于当前物品的最大堆叠数量
            //那就直接将当前物品的id（也就是键值）和数量填充到背包里面
            if (kv.Value.Count <= kv.Value.Define.StackLimit)
            {
                this.items[i].ItemId = (ushort)kv.Key;
                this.items[i].Count = (ushort)kv.Value.Count;
            }
            else//如果大于最大堆叠数量
            {
                //记录当前物品的数量
                int count = kv.Value.Count;
                //循环判断当前数量是否大于最大堆叠数量
                while (count > kv.Value.Define.StackLimit)
                {
                    //填充当前格子，填充的数量就是最大堆叠限制的数量（如：99个）
                    this.items[i].ItemId = (ushort)kv.Key;
                    this.items[i].Count = (ushort)kv.Value.Define.StackLimit;
                    //填充完成后，进入下一个格子，并将上面记录的数量减去最大堆叠数量
                    i++;
                    count -= kv.Value.Define.StackLimit;
                }
                //直到最后减完之后上面记录的数量减到小于最大堆叠数量
                //最后将剩余的物品数量全部填充到格子里面
                this.items[i].ItemId = (ushort)kv.Key;
                this.items[i].Count = (ushort)count;
            }
            i++;
        }
    }

    unsafe void Analyze(byte[] data)
    {
        //声明一个byte类型的指针，并指向data数组的第一位的内存地址
        //如果在C#中要用指针，必须要这么写，因为在C#中内存是动态管理的
        //这个是让我们的内存指定一块不会去改变
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
                //在将让得到的地址数据，存储下来的时候，只是改变他的值，不会修改他的地址
                //这也是为什么要用结构体的原因
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
                *item = items[i];
            }
        }
        return this.Info;
    }
}
