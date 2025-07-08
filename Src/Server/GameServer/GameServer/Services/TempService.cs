using Common;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Linq;

namespace GameServer.Services
{
    class TempService : Singleton<TempService>
    {
        public TempService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TempInviteRequest>(this.OnTempInviteRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TempInviteResponse>(this.OnTempInviteResponse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TempLeaveRequest>(this.OnTempLeave);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TempDisbandTempRequest>(this.OnTempDisband);
        }


        public void Init()
        {
            TempManager.Instance.Init();
        }

        /// <summary>
        /// 接受组队请求响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void OnTempInviteRequest(NetConnection<NetSession> sender, TempInviteRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("TempService OnTempInviteRequest FromID:[{0}],FromName:[{1}],ToID:[{2}],ToName:[{3}]", request.FromId, request.FromName, request.ToId, request.ToName);


            NetConnection<NetSession> target = SessionManager.Instance.GetSession(request.ToId);
            if (target == null)
            {
                sender.Session.Response.tempInviteRes = new TempInviteResponse();
                sender.Session.Response.tempInviteRes.Result = Result.Failed;
                sender.Session.Response.tempInviteRes.Errormsg = "当前好友不在线";
                sender.SendResPonse();
                return;
            }
            //在线，再判断他是否有队伍
            if (target.Session.Character.temp != null)
            {
                sender.Session.Response.tempInviteRes = new TempInviteResponse();
                sender.Session.Response.tempInviteRes.Result = Result.Failed;
                sender.Session.Response.tempInviteRes.Errormsg = "当前好友已经有队伍";
                sender.SendResPonse();
                return;
            }

            //转发请求
            Log.InfoFormat("TempService OnTempInviteRequest FromID:[{0}],FromName:[{1}],ToID:[{2}],ToName:[{3}]", request.FromId, request.FromName, request.ToId, request.ToName);
            target.Session.Response.tempInviteReq = request;
            target.SendResPonse();
        }

        /// <summary>
        /// 接收组队是否接收响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="response"></param>
        private void OnTempInviteResponse(NetConnection<NetSession> sender, TempInviteResponse response)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("TempService OnTempInviteResponse ");
            sender.Session.Response.tempInviteRes = response;
            if (response.Result == Result.Success)
            {
                //接受了组队的请求
                //获得发起者的Session
                var requester = SessionManager.Instance.GetSession(response.Request.FromId);
                if (requester == null)
                {
                    //发起者如果为空，则已经下线
                    sender.Session.Response.tempInviteRes.Result = Result.Failed;
                    sender.Session.Response.tempInviteRes.Errormsg = "请求者已经下线";
                }
                else
                {
                    //如果在线，则直接加入队伍
                    TempManager.Instance.AddTempMember(requester.Session.Character, character);
                    //将此消息发送到发起者
                    requester.Session.Response.tempInviteRes = response;
                    requester.SendResPonse();
                }
            }
            sender.SendResPonse();
        }

        /// <summary>
        /// 接收退出队伍响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void OnTempLeave(NetConnection<NetSession> sender, TempLeaveRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("TempService OnTempLeave ");
            //判断当前角色有没有队伍
            //如果当前队伍不为空，则有队伍
            if (character.temp != null)
            {
                //再判断要退出的队伍与当前所在的队伍一致
                if (request.TeamId == character.temp.id)
                {
                    //则可以退出队伍
                    character.temp.Leave(character);
                    sender.Session.Response.TempLeave = new TempLeaveResponse();
                    sender.Session.Response.TempLeave.Result = Result.Success;
                    sender.Session.Response.TempLeave.Errormsg = "退出队伍成功！";
                    sender.Session.Response.TempLeave.Characterid = request.Characterid;
                    sender.SendResPonse();
                    return;
                }
            }
            sender.Session.Response.TempLeave = new TempLeaveResponse();
            sender.Session.Response.TempLeave.Result = Result.Failed;
            sender.Session.Response.TempLeave.Errormsg = "当前角色没有队伍";
            sender.SendResPonse();
        }

        /// <summary>
        /// 接收队伍解散响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void OnTempDisband(NetConnection<NetSession> sender, TempDisbandTempRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("TempService OnTempDisband ");
            sender.Session.Response.tempDisband = new TempDisbandTempResponse();
            //判断当前角色有没有队伍
            if (character.temp != null)
            {
                //再次判断队伍是否相等
                if (character.temp.id == request.Temp.Id)
                {
                    //判断是否是队长，是队长才可解散队伍
                    if (character.Id == request.Temp.Leader)
                    {
                        var members = character.temp.Members.ToList();
                        foreach (var item in members)
                        {
                            character.temp.Leave(item);
                        }
                        sender.Session.Response.tempDisband.Result = Result.Success;
                        sender.Session.Response.tempDisband.Errormsg = "队伍解散成功";
                        sender.Session.Response.tempDisband.Temp = request.Temp;
                    }
                    else//不是队长
                    {
                        sender.Session.Response.tempDisband.Result = Result.Failed;
                        sender.Session.Response.tempDisband.Errormsg = "您不是队长，无权限解散队伍";
                        sender.SendResPonse();
                    }
                }
            }
        }
    }
}
