using Common;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    class GuildService : Singleton<GuildService>
    {
        public GuildService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildCreatRequest>(this.OnGuildCreate);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildListRequest>(this.OnGuildList);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildJoinRequest>(this.OnGuildJoinRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildJoinResponse>(this.OnGuildJoinResponse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildLeaveRequest>(this.OnGuildLeave);
        }

        public void Init()
        {
            GuildManager.Instance.Init();
        }

        /// <summary>
        /// 接收公会创建请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnGuildCreate(NetConnection<NetSession> sender, GuildCreatRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("GuildService OnGuildCreate : : GuildName:{0} : Character:[{1}] {2}", message.GuildName, character.Info.Id, character.Info.Name);

        }

        private void OnGuildList(NetConnection<NetSession> sender, GuildListRequest message)
        {
            
        }

        private void OnGuildJoinRequest(NetConnection<NetSession> sender, GuildJoinRequest message)
        {
            
        }

        private void OnGuildJoinResponse(NetConnection<NetSession> sender, GuildJoinResponse message)
        {
            
        }

        private void OnGuildLeave(NetConnection<NetSession> sender, GuildLeaveRequest message)
        {
            
        }

    }
}
