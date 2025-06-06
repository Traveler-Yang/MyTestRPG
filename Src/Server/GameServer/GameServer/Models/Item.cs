using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Models
{
    class Item
    {
        #region 将数据库中的信息拉取出来，存储到内存中

        TCharacterItem dbItem;

        public int ItemID;

        public int Count;

        public Item(TCharacterItem item)
        {
            this.dbItem = item;

            this.ItemID = item.ItemID;
            this.Count = item.ItemCount;
        }


        #endregion

        /// <summary>
        /// 物品数量的增加
        /// </summary>
        /// <param name="count">要增加的数量</param>
        public void Add(int count)
        {
            //给内存中的物品数量增加
            this.Count += count;
            //给数据库中的物品数量增加
            dbItem.ItemCount = this.Count;
        }

        /// <summary>
        /// 物品数量的减少
        /// </summary>
        /// <param name="count">要减少的数量</param>
        public void Remove(int count)
        {
            //给内存中的物品数量减少
            this.Count -= count;
            //给数据库中的物品数量减少
            dbItem.ItemCount = this.Count;
        }

        public bool Use(int count = 1)
        {
            return false;
        }

        public override string ToString()
        {
            return String.Format("ID：{0}，Count：{1}", this.ItemID, this.Count);
        }
    }
}
