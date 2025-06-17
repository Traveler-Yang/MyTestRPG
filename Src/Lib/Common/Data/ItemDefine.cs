using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkillBridge.Message;

namespace Common.Data
{

    public enum ItemFunction
    {
        None = 0,
        RecoverHP = 1, //恢复生命值
        RecoverMP = 2, //恢复魔法值
        AddBuff = 3, //添加Buff
        AddExp = 4, //添加经验
        AddMoney = 5, //添加金钱
        AddItem = 6, //添加物品
        AddSkillPoint = 7, //添加技能点
    }

    public class ItemDefine
    {
        public int ID { get; set; }
        public string Name { get; set; }//物品名称
        public string Description { get; set; }//物品描述
        public ItemType Type { get; set; }//物品类型
        public string Category { get; set; }//物品分类
        public int Level { get; set; }//等级
        public CharacterClass LimitClass { get; set; }//限制职业
        public bool CanUse { get; set; }//是否可以使用
        public float UseCD { get; set; }//使用冷却时间
        public int Price { get; set; }//物品价格
        public int SellPrice { get; set; }//物品出售价格
        public int StackLimit { get; set; }//堆叠限制
        public string Icon { get; set; }//道具图标
        public ItemFunction Function { get; set; }//物品功能
        public int Param { get; set; }//物品参数，具体含义根据Function不同而不同
        public List<int> Params { get; set; }//物品参数列表
    }
}
