using GameServer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Models
{
    class Guild
    {
        /// <summary>
        /// 公会Id
        /// </summary>
        public int Id { get { return this.Data.Id; } }

        public Character leader;//会长
        /// <summary>
        /// 公会名字
        /// </summary>
        public string Name { get { return this.Data.Name; } }

        /// <summary>
        /// 公会成员列表
        /// </summary>
        public List<Character> Members = new List<Character>();

        //时间戳
        public double timestape;

        public TGuild Data;

         public Guild(TGuild guild)
        {
            this.Data = guild;
        }
    }
}
