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
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildSearchRequest>(this.OnGuildSearch);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildAdminRequest>(this.OnGuildAdmin);
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
            //character.guild.leader = character;
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

            if (!character.guild.Leave(character))
            {
                //如果是false，则代表是会长，不允许离开
                sender.Session.Response.guildLeave.Result = Result.Failed;
                sender.Session.Response.guildLeave.Errormsg = "您是会长，不可以直接退出公会，请转让会长后才可退出";
                sender.SendResPonse();
                return;
            }
            sender.Session.Response.guildLeave.Result = Result.Success;
            sender.Session.Response.guildLeave.Errormsg = string.Format("退出 [{0}] 公会成功", character.guild.Name);

            sender.SendResPonse();
        }

        /// <summary>
        /// 接收公会搜索请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void OnGuildSearch(NetConnection<NetSession> sender, GuildSearchRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("GuildService OnGuildSearch : Input: {0}", request.Input);
            string guildName = "";
            int guildId = 0;
            //检查是否可以转换为ID，如果可以，说明搜索的是ID
            if (!int.TryParse(request.Input, out guildId))
            {
                guildName = request.Input;
            }
            foreach (var guild in GuildManager.Instance.Guilds)
            {
                if (guild.Value.Name == guildName)
                {
                    sender.Session.Response.guildSearch = new GuildSearchResponse();
                    sender.Session.Response.guildSearch.Result = Result.Success;
                    sender.Session.Response.guildSearch.guildInfo = guild.Value.GuildInfo(null);
                    sender.SendResPonse();
                }
            }
        }

        /// <summary>
        /// 接收公会权限请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnGuildAdmin(NetConnection<NetSession> sender, GuildAdminRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("GuildService OnGuildAdmin : Commend: {0} : Character: {1}", request.Command, request.Target);
            sender.Session.Response.guildAdmin = new GuildAdminResponse();
            if (character.guild == null)//判断这个角色有无公会
            {
                sender.Session.Response.guildAdmin.Result = Result.Failed;
                sender.Session.Response.guildAdmin.Errormsg = "你没有公会不要乱来";
                sender.SendResPonse();
                return;
            }
            if (character.guild.Data.LeaderID == character.TChar.ID)//判断当前执行管理行为的角色是否是会长
            {
                //判断是否传过来了公会的信息或者图标，则代表需要修改公会信息
                if (request.guildNotice != null && request.guildNotice != "" || 
                    request.guildIcon != null && request.guildIcon != "")
                {
                    character.guild.ExecuteAdmin(request.Command, request.guildNotice, request.guildIcon, character.Id);
                    sender.Session.Response.guildAdmin = new GuildAdminResponse();
                    sender.Session.Response.guildAdmin.Result = Result.Success;
                    sender.Session.Response.guildAdmin.Errormsg = "修改公会信息成功";
                    sender.Session.Response.guildAdmin.Command = request.Command;
                    sender.SendResPonse();
                    return;
                }
                //执行管理操作
                string errormsg = character.guild.ExecuteAdmin(request.Command, request.Target, character.Id);

                //查看目标角色是否在线，并且对方不是自己
                var target = SessionManager.Instance.GetSession(request.Target);
                if (target != null && target != SessionManager.Instance.GetSession(character.TChar.ID))
                {
                    //如果在线，则给他发送一个消息，告诉他一声
                    target.Session.Response.guildAdmin = new GuildAdminResponse();
                    target.Session.Response.guildAdmin.Result = Result.Success;
                    target.Session.Response.guildAdmin.Errormsg = errormsg;
                    target.Session.Response.guildAdmin.Command = request.Command;
                    target.SendResPonse();
                }
                sender.Session.Response.guildAdmin.Result = Result.Success;
                sender.Session.Response.guildAdmin.Errormsg = errormsg;
                sender.Session.Response.guildAdmin.Command = request.Command;
                sender.SendResPonse();
            }
            else
            {
                sender.Session.Response.guildAdmin.Result = Result.Failed;
                sender.Session.Response.guildAdmin.Errormsg = "您不是会长，无法进行此操作";
                sender.SendResPonse();
            }
        }
    }
}
