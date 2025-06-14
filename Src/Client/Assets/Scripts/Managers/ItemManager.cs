using Common.Data;
using Models;
using SkillBridge.Message;
using Services;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class ItemManager : Singleton<ItemManager>
    {
        public Dictionary<int, Item> Items = new Dictionary<int, Item>();
        internal void Init(List<NItemInfo> items)
        {
            this.Items.Clear();
            foreach (var info in items)
            {
                Item item = new Item(info);
                this.Items.Add(item.Id, item);

                Debug.LogFormat("ItemManager Init: [{0}]", item);
            }
            StatusService.Instance.RegisterStatusNotify(StatusType.Item, OnItemNotify);
        }

        public ItemDefine GetItem(int itemId)
        {
            return null;
        }

        bool OnItemNotify(NStatus status)
        {
            if (status.Action == StatusAction.Add)
            {
                this.AddItem(status.Id, status.Value);
            }
            if (status.Action == StatusAction.Delete)
            {
                this.RemoveItem(status.Id, status.Value);
            }
            return true;
        }

        /// <summary>
        /// 添加物品
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="count"></param>
        void AddItem(int itemId, int count)
        {
            Item item = null;
            //判断道具表里有没有这个物品
            //如果有，则直接增加数量
            //如果没有，则构建一个新的，并添加到表中
            if (this.Items.TryGetValue(itemId, out item))
            {
                item.Count += count;
            }
            else
            {
                item = new Item(itemId, count);
                this.Items.Add(itemId, item);
            }
            //添加完成后，最后要添加到背包中
            BagManager.Instance.AddItem(itemId, count);
        }
        /// <summary>
        /// 移除物品
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="count"></param>
        void RemoveItem(int itemId, int count)
        {
            //判断表中有没有这个物品
            //如果没有，则直接返回，什么都不做
            if (!this.Items.ContainsKey(itemId))
            {
                return;
            }
            //如果有则将这个物品拉取出来
            //并判断是否是小于要移除的数量
            //如果小于，则返回
            Item item = this.Items[itemId];
            if (item.Count < count)
                return;
            //如果不小于，则减去要移除的数量
            item.Count -= count;
            //添加完成后，最后要添加到背包中
            BagManager.Instance.RemoveItem(itemId, count);
        }

        public bool UseItem(int itemId)
        {
            return false;
        }

        public bool UseItem(ItemDefine item)
        {
            return false;
        }
    }
}
