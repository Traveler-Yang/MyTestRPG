using GameServer.Entities;
using GameServer.Models;
using GameServer.Services;
using Common;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class ItemManager
    {
        Character Owner;

        public Dictionary<int, Item> Items = new Dictionary<int, Item>();

        public ItemManager(Character owner)
        {
            this.Owner = owner;

            foreach (var item in Owner.TChar.Items)
            {
                this.Items.Add(item.ItemID, new Item(item));
            }
        }

        /// <summary>
        /// 使用物品
        /// </summary>
        /// <param name="itemId">使用的物品</param>
        /// <param name="count">使用的数量(默认1个)</param>
        /// <returns></returns>
        public bool UseItem(int itemId, int count = 1)
        {
            Log.InfoFormat("[{0}]UseItem[{1}：{2}]", Owner.TChar.ID, itemId, count);
            //声明一个物品来存储字典中查找的结果
            Item item = null;
            if (this.Items.TryGetValue(itemId, out item))
            {
                if (item.Count < count)
                    return false;

                //使用物品的逻辑

                //使用完物品后，减少相对于数量
                item.Remove(count);

                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断背包中是否有指定的物品
        /// </summary>
        /// <param name="itemId">指定物品</param>
        /// <returns>返回结果</returns>
        public bool HasItem(int itemId)
        {
            Item item = null;
            if (Items.TryGetValue(itemId, out item))
                return item.Count > 0;
            return false;
        }

        /// <summary>
        /// 获取背包中的物品
        /// </summary>
        /// <param name="itemId">要获取的物品</param>
        /// <returns></returns>
        public Item GetItem(int itemId)
        {
            Item item = null;
            Items.TryGetValue(itemId, out item);
            Log.InfoFormat("[{0}]GetItem[{1}：{2}]", this.Owner.TChar.ID, itemId, item);
            return item;
        }

        /// <summary>
        /// 添加物品到背包
        /// </summary>
        /// <param name="itemId">要添加的物品</param>
        /// <param name="count">要添加的数量</param>
        /// <returns></returns>
        public bool AddItem(int itemId, int count)
        {
            Item item = null;
            //如果字典中有这个物品，则直接增加数量
            if (this.Items.TryGetValue(itemId, out item))
            {
                item.Add(count);
            }
            else//如果字典中没有这个物品，则创建一个新物品
            {
                TCharacterItem dbItem = new TCharacterItem();
                dbItem.CharacterID = Owner.TChar.ID;
                dbItem.Owner = Owner.TChar;
                dbItem.ItemID = itemId;
                dbItem.ItemCount = count;
                Owner.TChar.Items.Add(dbItem);
                item = new Item(dbItem);
                Items.Add(itemId, item);
            }
            this.Owner.StatusManager.AddItemChange(itemId, count,StatusAction.Add);
            Log.InfoFormat("[{0}]AddItem[{1}] addCount：[{2}]", this.Owner.TChar.ID, itemId, count);
            return true;
        }

        /// <summary>
        /// 移除物品
        /// </summary>
        /// <param name="itemId">要移除的物品</param>
        /// <param name="count">要移除的数量</param>
        /// <returns></returns>
        public bool RemoveItem(int itemId, int count)
        {
            //如果字典中没有这个物品，则直接返回false
            if (!this.Items.ContainsKey(itemId))
            {
                return false;
            }
            //将这个物品从字典中取出来
            Item item = this.Items[itemId];
            //再次判断这个物品的数量是否足够移除
            //如果数量不够，则返回false
            if (item.Count < count)
                return false;
            //如果这个物品的数量足够，则从物品中移除指定数量
            item.Remove(count);
            this.Owner.StatusManager.AddItemChange(itemId, count, StatusAction.Delete);
            Log.InfoFormat("[{0}]RemoveItem[{1}] removeCount[{2}]", this.Owner.TChar.ID, item, count);
            return true;
        }

        public void GetItemInfos(List<NItemInfo> list)
        {
            foreach (var item in this.Items)
            {
                list.Add(new NItemInfo() { Id = item.Value.ItemID, Count = item.Value.Count });
            }
        }
    }
}
