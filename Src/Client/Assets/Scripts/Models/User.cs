﻿using Common.Data;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Models
{
    class User : Singleton<User>
    {
        SkillBridge.Message.NUserInfo userInfo;


        public SkillBridge.Message.NUserInfo Info
        {
            get { return userInfo; }
        }


        public void SetupUserInfo(SkillBridge.Message.NUserInfo info)
        {
            this.userInfo = info;
        }

        internal void AddGold(int gold)
        {
            this.CurrentCharacter.Gold += gold;
        }

        public MapDefine CurrentMapData { get; set; }

        public SkillBridge.Message.NCharacterInfo CurrentCharacter { get; set; }

        public GameObject CurrentCharacterObject { get; set; }
        public NTempInfo TempInfo { get; set; }
    }
}
