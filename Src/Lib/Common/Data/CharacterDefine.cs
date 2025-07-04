﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkillBridge.Message;

namespace Common.Data
{
    public class CharacterDefine
    {
        public int TID { get; set; }
        public string Name { get; set; }
        public CharacterClass Class { get; set; }
         public string Icon { get; set; }
        public string Resource { get; set; }
        public string Description { get; set; }

        //基本属性
        public int Speed { get; set; }
    }
}
