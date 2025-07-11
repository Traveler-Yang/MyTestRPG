using Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Services
{
    class GuildService : Singleton<GuildService>, IDisposable
    {
        public UnityAction OnGuildUpdate;//公会更新事件
        public UnityAction<List<NGuildInfo>> OnGuildListResult;//公会列表请求事件

        public UnityAction<bool> OnGuildCreateResult;//公会创建成功事件

        public UnityAction<NGuildInfo> OnGuildSearchResult;//公会搜索事件

        public void Init()
        {

        }

        public GuildService()
        {
            MessageDistributer.Instance.Subscribe<GuildCreatResponse>(this.OnGuildCreate);
            MessageDistributer.Instance.Subscribe<GuildListResponse>(this.OnGuildList);
            MessageDistributer.Instance.Subscribe<GuildJoinRequest>(this.OnGuildJoinRequest);
            MessageDistributer.Instance.Subscribe<GuildJoinResponse>(this.OnGuildJoinResponse);
            MessageDistributer.Instance.Subscribe<GuildResponse>(this.OnGuild);
            MessageDistributer.Instance.Subscribe<GuildLeaveResponse>(this.OnGuildLeave);
            MessageDistributer.Instance.Subscribe<GuildSearchResponse>(this.OnGuildSearch);
            MessageDistributer.Instance.Subscribe<GuildAdminResponse>(this.OnGuildAdmin);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<GuildCreatResponse>(this.OnGuildCreate);
            MessageDistributer.Instance.Unsubscribe<GuildListResponse>(this.OnGuildList);
            MessageDistributer.Instance.Unsubscribe<GuildJoinRequest>(this.OnGuildJoinRequest);
            MessageDistributer.Instance.Unsubscribe<GuildJoinResponse>(this.OnGuildJoinResponse);
            MessageDistributer.Instance.Unsubscribe<GuildResponse>(this.OnGuild);
            MessageDistributer.Instance.Unsubscribe<GuildLeaveResponse>(this.OnGuildLeave);
            MessageDistributer.Instance.Unsubscribe<GuildSearchResponse>(this.OnGuildSearch);
            MessageDistributer.Instance.Unsubscribe<GuildAdminResponse>(this.OnGuildAdmin);
        }

        /// <summary>
        /// 发送公会创建请求
        /// </summary>
        /// <param name="guildName"></param>
        /// <param name="guildNotice"></param>
        public void SendGuildCreate(string guildName, string guildNotice, string icon)
        {
            Debug.Log("SendGuildCreate");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildCreat = new GuildCreatRequest();
            message.Request.guildCreat.GuildName = guildName;
            message.Request.guildCreat.GuildNotice = guildNotice;
            message.Request.guildCreat.GuildIcon = icon;
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 接收公会创建响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="response"></param>
        private void OnGuildCreate(object sender, GuildCreatResponse response)
        {
            Debug.LogFormat("OnGuildCreateResponse: {0}", response.Result);
            if (this.OnGuildCreateResult != null)//判断创建事件有没有通知到
            {
                this.OnGuildCreateResult(response.Result == Result.Success);
            }
            if (response.Result == Result.Success)
            {
                GuildManager.Instance.Init(response.guildInfo);
                MessageBox.Show(string.Format("[{0}] 公会创建成功", response.guildInfo.GuildName), "创建公会");
            }
            else
                MessageBox.Show(string.Format("[{0}] 公会创建失败", response.guildInfo.GuildName), "创建公会", MessageBoxType.Error);
        }

        /// <summary>
        /// 接收加入公会申请
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnGuildJoinRequest(object sender, GuildJoinRequest request)
        {
            var confirm = MessageBox.Show(string.Format("[{0}] 申请加入公会，是否同意？", request.Apply.characterName), "公会申请", MessageBoxType.Confirm, "同意", "拒绝");
            confirm.OnYes = () =>
            {
                this.SendGuildJoinResponse(true, request);
            };
            confirm.OnNo = () =>
            {
                this.SendGuildJoinResponse(false, request);
            };
        }


        /// <summary>
        /// 发送加入公会请求
        /// </summary>
        /// <param name="guildId">公会ID</param>
        public void SendGuildJoinRequest(int guildId)
        {
            Debug.Log("SendGuildJoinRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildJoinReq = new GuildJoinRequest();
            message.Request.guildJoinReq.Apply = new NGuildApplyInfo();
            message.Request.guildJoinReq.Apply.GuildId = guildId;
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 发送是否同意请求
        /// </summary>
        /// <param name="accept"></param>
        /// <param name="request"></param>
        public void SendGuildJoinResponse(bool accept, GuildJoinRequest request)
        {
            Debug.Log("SendGuildJoinResponse");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildJoinRes = new GuildJoinResponse();
            message.Request.guildJoinRes.Result = Result.Success;
            message.Request.guildJoinRes.Apply = request.Apply;
            message.Request.guildJoinRes.Apply.Result = accept ? ApplyResult.Accept : ApplyResult.Reject;
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 接收加入公会是否成功响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnGuildJoinResponse(object sender, GuildJoinResponse response)
        {
            Debug.LogFormat("OnGuildJoinResponse: {0}", response.Result);
            if (response.Result == Result.Success)
            {
                MessageBox.Show("加入公会成功", "公会");
            }
            else
                MessageBox.Show("加入公会失败", "公会", MessageBoxType.Error);
        }
        /// <summary>
        /// 接收后处理更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnGuild(object sender, GuildResponse message)
        {
            Debug.LogFormat("OnGuild: {0} {1}：{2}", message.Result, message.guildInfo.Id, message.guildInfo.GuildName);
            GuildManager.Instance.Init(message.guildInfo);
            if (this.OnGuildUpdate != null)
                this.OnGuildUpdate();
        }

        /// <summary>
        /// 发送退出公会请求
        /// </summary>
        public void SendGuildLeaveRequest()
        {
            Debug.LogFormat("SendGuildLeaveRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildLeave = new GuildLeaveRequest();
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 接收退出公会响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnGuildLeave(object sender, GuildLeaveResponse message)
        {
            Debug.LogFormat("OnGuildLeave");
            if (message.Result == Result.Success)
            {
                GuildManager.Instance.Init(null);
                MessageBox.Show(message.Errormsg, "公会");
            }
            else
                MessageBox.Show(message.Errormsg, "公会", MessageBoxType.Error);
        }

        /// <summary>
        /// 发送列表刷新请求
        /// </summary>
        public void SendGuildListRequest()
        {
            Debug.LogFormat("SendGuildListRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildList = new GuildListRequest();
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 接收列表刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnGuildList(object sender, GuildListResponse message)
        {
            if (this.OnGuildListResult != null)
                this.OnGuildListResult(message.Guilds);
        }

        /// <summary>
        /// 好友搜索请求
        /// </summary>
        /// <param name="input"></param>
        public void SendGuildSearch(string input)
        {
            Debug.LogFormat("SendGuildSearch");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildSearch = new GuildSearchRequest();
            message.Request.guildSearch.Input = input;
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 接收好友搜索响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="response"></param>
        private void OnGuildSearch(object sender, GuildSearchResponse response)
        {
            Debug.LogFormat("OnGuildSearch");
            if (response.Result == Result.Success)
            {
                if (this.OnGuildSearchResult != null)
                    this.OnGuildSearchResult(response.guildInfo);
            }
        }

        /// <summary>
        /// 加入公会审批请求
        /// </summary>
        /// <param name="accept"></param>
        /// <param name="apply"></param>
        public void SendGuilJoinApply(bool accept, NGuildApplyInfo apply)
        {
            Debug.Log("SendGuilJoinApply");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildJoinRes = new GuildJoinResponse();
            message.Request.guildJoinRes.Result = Result.Success;
            message.Request.guildJoinRes.Apply = apply;
            message.Request.guildJoinRes.Apply.Result = accept ? ApplyResult.Accept : ApplyResult.Reject;
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 公会权限请求
        /// </summary>
        /// <param name="command"></param>
        /// <param name="characterId"></param>
        public void SendAdminCommand(GuildAdminCommand command, string guildnotice, string guildIcon, int characterId)
        {
            Debug.LogFormat("SendAdminCommand : Command: [{0}] : Character: [{1}]", command, characterId);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildAdmin = new GuildAdminRequest();
            message.Request.guildAdmin.Command = command;
            message.Request.guildAdmin.guildNotice = guildnotice;
            message.Request.guildAdmin.guildIcon = guildIcon;
            message.Request.guildAdmin.Target = characterId;
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 接收权限响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnGuildAdmin(object sender, GuildAdminResponse response)
        {
            Debug.LogFormat("OnGuildAdmin : {0} {1}", response.Command, response.Result);
            string target = "";
            switch (response.Command)
            {
                case GuildAdminCommand.Kickout:
                    target = "踢出公会";
                    break;
                case GuildAdminCommand.Promote:
                    target = "晋升职务";
                    break;
                case GuildAdminCommand.Depost:
                    target = "罢免职务";
                    break;
                case GuildAdminCommand.Transfer:
                    target = "转让会长";
                    break;
                case GuildAdminCommand.ChangeInfo:
                    target = "修改公会信息";
                    break;
            }
            MessageBox.Show(string.Format("执行操作: {0}\n结果: {1}", target, response.Errormsg));
        }
    }
}
