using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkillBridge.Message;

namespace Common.Data
{
    public class ShopItemDefine
    {
        public int ItemID { get; set; }
        public int Count { get; set; }//商店道具数量
        public int Price { get; set; }//商店道具价格
        public int Status { get; set; }//状态
    }
}
