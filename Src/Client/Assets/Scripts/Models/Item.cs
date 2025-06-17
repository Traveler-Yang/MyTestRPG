using Common.Data;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Models
{
    public class Item 
    {
        public int Id;

        public int Count;
        public ItemDefine Define;
        public EquipDefine EquipInfo;
        public Item(NItemInfo item) : 
            this(item.Id, item.Count)
        {

        }
        public Item(int id, int count)
        {
            this.Id = id;
            this.Count = count;
            DataManager.Instance.Items.TryGetValue(this.Id, out Define);
            DataManager.Instance.Equips.TryGetValue(this.Id, out EquipInfo);
        }

        public override string ToString()
        {
            return String.Format("ID£º{0}£¬Count£º{1}", this.Id, this.Count);
        }
    }
}
