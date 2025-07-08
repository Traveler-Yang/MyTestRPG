using Common;
using GameServer.Entities;
using GameServer.Models;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class GuildManager : Singleton<GuildManager>
    {
        public List<NGuildInfo> Guilds = new List<NGuildInfo>();

        public void Init()
        {

        }

        /// <summary>
        /// 检查名称是否存在
        /// </summary>
        /// <param name="guildName"></param>
        /// <returns></returns>
        public bool CheckNameIsExisted(string guildName)
        {
            return true;
        }

        /// <summary>
        /// 创建公会
        /// </summary>
        /// <param name="guildName"></param>
        /// <param name="guildNotice"></param>
        /// <param name="leader"></param>
        public void CerateGuild(string guildName, string guildNotice, Character leader)
        {

        }

        internal Guild GetGuild(int guildId)
        {
            throw new NotImplementedException();
        }

        public List<NGuildInfo> GetGuildsInfo()
        {
            return this.Guilds;
        }
    }
}
