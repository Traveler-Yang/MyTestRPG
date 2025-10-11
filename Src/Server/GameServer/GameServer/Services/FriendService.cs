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
    public class FriendService : Singleton<FriendService>
    {
        public FriendService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendAddRequest>(this.OnFriendAddReq);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendAddResponse>(this.OnFriendAddRes);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendRemoveRequest>(this.OnFriendRemoveReq);
        }

        public void Init()
        {

        }
        /// <summary>
        /// 接收添加好友的请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void OnFriendAddReq(NetConnection<NetSession> sender, FriendAddRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnFriendAddReq : : fromId:{0} : fromName:{1} : toId{2} : toName{3}", request.FromId, request.FromName, request.ToId, request.ToName);
            NetConnection<NetSession> toRequest = SessionManager.Instance.GetSession(request.ToId);
            if (toRequest == null)
            {
                sender.Session.Response.friendAddRes = new FriendAddResponse();
                sender.Session.Response.friendAddRes.Result = Result.Failed;
                sender.Session.Response.friendAddRes.Errormsg = "对方不在线";
                sender.SendResPonse();
                return;
            }
            var toFriend = character.TChar.Friends.Where(f => f.FriendID == request.ToId).FirstOrDefault();
            if (toFriend != null)
            {
                sender.Session.Response.friendAddRes = new FriendAddResponse();
                sender.Session.Response.friendAddRes.Result = Result.Failed;
                sender.Session.Response.friendAddRes.Errormsg = "对方已经是好友了";
                sender.SendResPonse();
                return;
            }
            sender.Session.Response.friendAddRes = new FriendAddResponse();
            sender.Session.Response.friendAddRes.Result = Result.Success;
            sender.Session.Response.friendAddRes.Errormsg = "请求发送成功！";
            sender.SendResPonse();
            //消息转发
            toRequest.Session.Response.friendAddReq = request;
            toRequest.SendResPonse();
        }

        /// <summary>
        /// 接收好友添加是否同意的请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="response"></param>
        private void OnFriendAddRes(NetConnection<NetSession> sender, FriendAddResponse response)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnFriendAddRes : : Result:{0} : ErrorMsg:{1}", response.Result, response.Errormsg);
            NetConnection<NetSession> fromCharacter = SessionManager.Instance.GetSession(response.Request.FromId);
            if (fromCharacter != null)
            {
                if (response.Result == Result.Success)
                {
                    //同意
                    //互相加好友
                    character.FriendManager.AddFriend(response.Request.FromId);
                    fromCharacter.Session.Character.FriendManager.AddFriend(response.Request.ToId);
                    DBService.Instance.Save();
                    sender.Session.Response.friendAddRes = new FriendAddResponse();
                    sender.Session.Response.friendAddRes.Result = Result.Success;
                    sender.Session.Response.friendAddRes.Errormsg = "添加好友成功！";
                    sender.Session.Response.friendAddRes.Request = response.Request;
                    sender.SendResPonse();
                    //给发起方回消息
                    fromCharacter.Session.Response.friendAddRes = new FriendAddResponse();
                    fromCharacter.Session.Response.friendAddRes.Result = Result.Success;
                    fromCharacter.Session.Response.friendAddRes.Errormsg = "对方同意了你的请求";
                    fromCharacter.Session.Response.friendAddRes.Request = response.Request;
                    fromCharacter.SendResPonse();
                    return;

                }
                //拒绝
                sender.Session.Response.friendAddRes = new FriendAddResponse();
                sender.Session.Response.friendAddRes.Result = Result.Failed;
                sender.Session.Response.friendAddRes.Errormsg = "添加好友失败！";
                sender.Session.Response.friendAddRes.Request = response.Request;
                sender.SendResPonse();

                fromCharacter.Session.Response.friendAddRes = new FriendAddResponse();
                fromCharacter.Session.Response.friendAddRes.Result = Result.Failed;
                fromCharacter.Session.Response.friendAddRes.Errormsg = "对方拒绝了你的请求";
                fromCharacter.Session.Response.friendAddRes.Request = response.Request;
                fromCharacter.SendResPonse();
            }

        }

        private void OnFriendRemoveReq(NetConnection<NetSession> sender, FriendRemoveRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnFriendRemoveReq : CharacterId [{0}] : ToId [{1}]", character.Id, request.Id);
            //查看我的好友列表中是否有对方
            var toFriend = character.TChar.Friends.Where(tf => tf.FriendID == request.Id);
            if (toFriend == null)
            {
                sender.Session.Response.friendRemoveRes = new FriendRemoveResponse();
                sender.Session.Response.friendRemoveRes.Result = Result.Failed;
                sender.Session.Response.friendRemoveRes.Errormsg = "该好友不存在，请联系管理员！";
                sender.SendResPonse();
                return;
            }
            
            NetConnection<NetSession> toSession = SessionManager.Instance.GetSession(request.Id);
            if (toSession != null)
            {
                //查看对方的好友列表中是否有我
                var fromFriend = toSession.Session.Character.TChar.Friends.Where(ff => ff.FriendID == character.Info.Id);
                if (fromFriend == null)
                {
                    sender.Session.Response.friendRemoveRes = new FriendRemoveResponse();
                    sender.Session.Response.friendRemoveRes.Result = Result.Failed;
                    sender.Session.Response.friendRemoveRes.Errormsg = "双方好友列表不一致，请联系管理员！";
                    sender.SendResPonse();
                    return;
                }
            }
            //双向删除好友操作
            character.FriendManager.RemoveFriend(request.Id);
            toSession.Session.Character.FriendManager.RemoveFriend(character.Info.Id);
            DBService.Instance.Save();
            //返回自己
            sender.Session.Response.friendRemoveRes = new FriendRemoveResponse();
            sender.Session.Response.friendRemoveRes.Id = request.Id;
            sender.Session.Response.friendRemoveRes.Result = Result.Success;
            sender.Session.Response.friendRemoveRes.Errormsg = "成功将对方删除！";
            sender.SendResPonse();
            //对方如果在线，则通知对方
            if (toSession != null)
            {
                toSession.Session.Response.friendRemoveNotify = new FriendRemoveNotify();
                toSession.Session.Response.friendRemoveNotify.Id = character.Info.Id;
                toSession.Session.Response.friendRemoveNotify.Result = Result.Success;
                toSession.Session.Response.friendRemoveNotify.Errormsg = string.Format("{0} 将你从好友列表删除了！", CharacterManager.Instance.GetCharacter(character.Info.Id).Name);
                toSession.SendResPonse();
            }
        }
    }
}
