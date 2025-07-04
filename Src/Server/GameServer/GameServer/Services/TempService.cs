using Common;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System.Linq;

namespace GameServer.Services
{
    class TempService : Singleton<FriendService>
    {
        public TempService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TempInviteRequest>(this.OnTempInviteRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TempInviteResponse>(this.OnTempInviteResponse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TempLeaveResponse>(this.OnTempLeave);
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
            Log.InfoFormat("[GameServer] UserService OnTempInviteRequest FromID:[{0}],FromName:[{1}],ToID:[{2}],ToName:[{3}]", request.FromId, request.FromName, request.ToId, request.ToName);


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
            Log.InfoFormat("[GameServer] UserService OnTempInviteRequest FromID:[{0}],FromName:[{1}],ToID:[{2}],ToName:[{3}]", request.FromId, request.FromName, request.ToId, request.ToName);
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
            Log.InfoFormat("[GameServer] UserService OnTempInviteResponse ");
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
        /// <param name="response"></param>
        private void OnTempLeave(NetConnection<NetSession> sender, TempLeaveResponse response)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("[GameServer] UserService OnTempLeave ");
            sender.Session.Response.TempLeave = new TempLeaveResponse();
            sender.Session.Response.TempLeave.Result = Result.Success;
            sender.Session.Response.TempLeave.Characterid = response.Characterid;
            sender.SendResPonse();
        }

    }
}
