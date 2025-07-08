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
        private void OnGuildCreate(NetConnection<NetSession> sender, GuildCreatRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("GuildService OnGuildCreate : : GuildName:{0} : Character:[{1}] {2}", request.GuildName, character.Info.Id, character.Info.Name);
            sender.Session.Response.guildCreat = new GuildCreatResponse();
            //判断是否有公会
            if (character.guild != null)
            {
                sender.Session.Response.guildCreat.Result = Result.Failed;
                sender.Session.Response.guildCreat.Errormsg = "你已经已经有公会了";
                sender.SendResPonse();
                return;
            }
            //判断是否有重名的公会
            if (GuildManager.Instance.CheckNameIsExisted(request.GuildName))
            {
                sender.Session.Response.guildCreat.Result = Result.Failed;
                sender.Session.Response.guildCreat.Errormsg = "已存在相同名称的公会";
                sender.SendResPonse();
                return;
            }
            //创建公会
            GuildManager.Instance.CerateGuild(request.GuildName, request.GuildNotice, request.GuildIcon, character);
            sender.Session.Response.guildCreat.guildInfo = character.guild.GuildInfo(character);
            sender.Session.Response.guildCreat.Result = Result.Success;
            sender.SendResPonse();
        }

        /// <summary>
        /// 接收公会列表请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnGuildList(NetConnection<NetSession> sender, GuildListRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("GuildService OnGuildList: Character :[{0}] {1}", character.Id, character.Name);
            sender.Session.Response.guildList = new GuildListResponse();
            sender.Session.Response.guildList.Guilds.AddRange(GuildManager.Instance.GetGuildsInfo());
            sender.Session.Response.guildList.Result = Result.Success;
            sender.SendResPonse();
        }

        /// <summary>
        /// 接收公会加入申请
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void OnGuildJoinRequest(NetConnection<NetSession> sender, GuildJoinRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("GuildService OnGuildJoinRequest: Guild:[{0}] : Character :[{1}] {2}", request.Apply.GuildId ,request.Apply.characterId, request.Apply.characterName);
            //查找有无此公会
            var guild = GuildManager.Instance.GetGuild(request.Apply.GuildId);
            if (guild == null)
            {
                sender.Session.Response.guildJoinRes = new GuildJoinResponse();
                sender.Session.Response.guildJoinRes.Result = Result.Failed;
                sender.Session.Response.guildJoinRes.Errormsg = "公会不存在";
                sender.SendResPonse();
                return;
            }
            request.Apply.characterId = character.TChar.ID;
            request.Apply.characterName = character.TChar.Name;
            request.Apply.Class = character.TChar.Class;
            request.Apply.Level = character.TChar.Level;

            //加入公会申请是否同意
            if (guild.JoinApply(request.Apply))
            {
                var leader = SessionManager.Instance.GetSession(guild.Data.LeaderID);
                //如果会长在线，则给会长分发一份
                if (leader != null)
                {
                    leader.Session.Response.guildJoinReq = request;
                    leader.SendResPonse();
                }
            }
            else
            {
                sender.Session.Response.guildJoinRes = new GuildJoinResponse();
                sender.Session.Response.guildJoinRes.Result = Result.Failed;
                sender.Session.Response.guildJoinRes.Errormsg = "请勿重复提交申请";
                sender.SendResPonse();
            }
        }

        /// <summary>
        /// 接收公会申请是否同意响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnGuildJoinResponse(NetConnection<NetSession> sender, GuildJoinResponse response)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("GuildService OnGuildJoinResponse: Guild:[{0}] : Character :[{1}] {2}", response.Apply.GuildId, response.Apply.characterId, response.Apply.characterName);

            var guild = GuildManager.Instance.GetGuild(response.Apply.GuildId);
            if (response.Result == Result.Success)
            {
                //同意申请
                guild.JoinAppove(response.Apply);
            }

            var requester = SessionManager.Instance.GetSession(response.Apply.characterId);
            if (requester != null)//申请的人是否还在线，在线的话，发送给他
            {
                //把他加到公会里面
                requester.Session.Character.guild = guild;

                requester.Session.Response.guildJoinRes = response;
                requester.Session.Response.guildJoinRes.Result = Result.Success;
                requester.Session.Response.guildJoinRes.Errormsg = "加入公会成功";
                requester.SendResPonse();
            }
        }

        /// <summary>
        /// 接收成员离开请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void OnGuildLeave(NetConnection<NetSession> sender, GuildLeaveRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("GuildService OnGuildLeave : Character:[{0}]", character.Id);
            sender.Session.Response.guildLeave = new GuildLeaveResponse();

            character.guild.Leave(character);
            sender.Session.Response.guildLeave.Result = Result.Success;

            DBService.Instance.Save();

            sender.SendResPonse();
        }
    }
}
