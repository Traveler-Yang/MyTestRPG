using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkillBridge.Message;

namespace Common.Data
{

    public class ShopDefine
    {
        public int ID { get; set; }
        public string Name { get; set; }//商店名字
        public string Icon { get; set; }//商店图标
        public string Description { get; set; }//商店描述信息
        public int Status { get; set; }//状态
    }
}
