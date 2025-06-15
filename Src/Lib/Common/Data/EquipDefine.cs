using SkillBridge.Message;

namespace Common.Data
{
    public class EquipDefine
    {
        public int ID { get; set; }
        public EquipSlot Slot { get; set; }//装备类型
        public string Category { get; set; }//类别
        public float STR { get; set; }//力量
        public float INT { get; set; }//智力
        public float DEX { get; set; }//敏捷
        public float HP { get; set; }//生命
        public float MP { get; set; }//魔法
        public float AD { get; set; }//物理攻击
        public float AP { get; set; }//魔法攻击
        public float DEF { get; set; }//物理防御
        public float MDEF { get; set; }//魔法防御
        public float SPD { get; set; }//攻速
        public float CRI { get; set; }//暴击率
    }
}
