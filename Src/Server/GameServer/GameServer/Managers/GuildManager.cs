using Common;
using Common.Utils;
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
        public Dictionary<int, Guild> Guilds = new Dictionary<int, Guild>();
        /// <summary>
        /// 所有的公会名称
        /// </summary>
        public HashSet<string> GuildNames = new HashSet<string>();

        public void Init()
        {
            this.Guilds.Clear();
            foreach (var guild in DBService.Instance.Entities.TGuilds)
            {
                this.AddGuild(new Guild(guild));
            }
        }

        /// <summary>
        /// Add公会
        /// </summary>
        /// <param name="guild"></param>
        private void AddGuild(Guild guild)
        {
            this.Guilds.Add(guild.Id, guild);
            this.GuildNames.Add(guild.Name);
            guild.timestape = TimeUtil.timestamp;
        }

        /// <summary>
        /// 检查名称是否存在
        /// </summary>
        /// <param name="guildName"></param>
        /// <returns></returns>
        public bool CheckNameIsExisted(string guildName)
        {
            return GuildNames.Contains(guildName);
        }

        /// <summary>
        /// 创建公会
        /// </summary>
        /// <param name="guildName"></param>
        /// <param name="guildNotice"></param>
        /// <param name="leader"></param>
        public bool CerateGuild(string guildName, string guildNotice, string icon, Character leader)
        {
            DateTime now = DateTime.Now;
            //构建DB公会
            TGuild dbGuild = DBService.Instance.Entities.TGuilds.Create();
            dbGuild.Name = guildName;
            dbGuild.Icon = icon;
            dbGuild.Notice = guildNotice;
            dbGuild.LeaderID = leader.Id;
            dbGuild.LeaderName = leader.Name;
            dbGuild.CreateTime = now;
            //Add到DB数据库中
            DBService.Instance.Entities.TGuilds.Add(dbGuild);

            //构建公会Model
            Guild guild = new Guild(dbGuild);
            guild.AddMember(leader.Id, leader.Name, leader.TChar.Class, leader.TChar.Level, GuildDuty.President);
            leader.Gold -= 50000;//扣钱
            leader.guild = guild;
            DBService.Instance.Save();
            leader.TChar.GuildId = guild.Id;
            DBService.Instance.Save();
            this.AddGuild(guild);

            return true;
        }

        internal Guild GetGuild(int guildId)
        {
            if (guildId == 0)
                return null;
            Guild guild = null;
            this.Guilds.TryGetValue(guildId, out guild);
            return guild;
        }

        public List<NGuildInfo> GetGuildsInfo()
        {
            List<NGuildInfo> result = new List<NGuildInfo>();
            foreach (var kv in this.Guilds)
            {
                result.Add(kv.Value.GuildInfo(null));
            }
            return result;
        }
    }
}
