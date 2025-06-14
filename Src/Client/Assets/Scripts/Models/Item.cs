using Common.Data;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Models
{
    public class Item : MonoBehaviour
    {
        public int Id;

        public int Count;
        public ItemDefine define;
        public Item(NItemInfo item) : 
            this(item.Id, item.Count)
        {

        }
        public Item(int id, int count)
        {
            this.Id = id;
            this.Count = count;
            this.define = DataManager.Instance.Items[id];//获取当前道具的所有信息
        }

        public override string ToString()
        {
            return String.Format("ID：{0}，Count：{1}", this.Id, this.Count);
        }
    }
}
