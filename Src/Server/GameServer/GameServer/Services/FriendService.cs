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
    class FriendService : Singleton<FriendService>
    {
        public FriendService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendAddRequest>(this.OnFriendAddRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendAddResponse>(this.OnFriendAddResponse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendRemoveRequest>(this.OnFriendRemove);
        }


        public void Init()
        {

        }

        /// <summary>
        /// 收到添加好友请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void OnFriendAddRequest(NetConnection<NetSession> sender, FriendAddRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("UserService OnFriendAddRequest: :FromID:[{0}],FromName:[{1}],ToID:[{2}],ToName:[{3}]", request.FromId, request.FromName, request.ToId, request.ToName);

            if (request.ToId == 0)
            {
                //如果Id为0(没有输入ID)，则用名字查找
                foreach (var cha in CharacterManager.Instance.Characters)
                {
                    if (request.ToName == cha.Value.TChar.Name)
                    {
                        request.ToId = cha.Key;
                        break;
                    }
                }
            }
            //需要加的那个人的对象
            NetConnection<NetSession> friend = null;
            if (request.ToId > 0)
            {
                //检查当前好友列表中有没有这个好友，如果有，则已经是好友了
                if (character.FriendManager.GetFriendInfo(request.ToId) != null)
                {
                    sender.Session.Response.friendAddRes = new FriendAddResponse();
                    sender.Session.Response.friendAddRes.Result = Result.Failed;
                    sender.Session.Response.friendAddRes.Errormsg = "已经是好友了";
                    sender.SendResPonse();
                    return;
                }
                //如果当前好友列表中没有找到，则从Session中拉取出来这个好友
                friend = SessionManager.Instance.GetSession(request.ToId);
            }
            //如果在Session中没有找到，则这个角色不在线或者不存在
            if (friend == null)
            {
                sender.Session.Response.friendAddRes = new FriendAddResponse();
                sender.Session.Response.friendAddRes.Result = Result.Failed;
                sender.Session.Response.friendAddRes.Errormsg = "角色不存在或已离线";
                sender.SendResPonse();
                return;
            }

            Log.InfoFormat("UserService OnFriendAddRequest: :FromID:[{0}],FromName:[{1}],ToID:[{2}],ToName:[{3}]", request.FromId, request.FromName, request.ToId, request.ToName);
            //消息转发
            //将此消息，发送给另一个角色
            friend.Session.Response.friendAddReq = request;
            friend.SendResPonse();
        }

        /// <summary>
        /// 收到加好友响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="response"></param>
        private void OnFriendAddResponse(NetConnection<NetSession> sender, FriendAddResponse response)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("UserService OnFriendAddResponse: :Character:[{0}],Result:[{1}],FromID:[{2}],ToID:[{3}]", character.Id , response.Result, response.Request.FromId, response.Request.ToId);
            sender.Session.Response.friendAddRes = response;
            var requester = SessionManager.Instance.GetSession(response.Request.FromId);
            if (response.Result == Result.Success)
            {
                //接受好友请求
                if (requester == null)
                {
                    sender.Session.Response.friendAddRes.Result = Result.Failed;
                    sender.Session.Response.friendAddRes.Errormsg = "请求者已下线";
                }
                else
                {
                    //互相加好友
                    character.FriendManager.AddFriend(requester.Session.Character);
                    requester.Session.Character.FriendManager.AddFriend(character);
                    DBService.Instance.Save();
                    requester.Session.Response.friendAddRes = response;
                    requester.Session.Response.friendAddRes.Result = Result.Success;
                    requester.Session.Response.friendAddRes.Errormsg = "添加好友成功";
                    requester.SendResPonse();
                }
            }
            requester.Session.Response.friendAddRes = response;
            requester.Session.Response.friendAddRes.Result = Result.Failed;
            requester.SendResPonse();
        }

        /// <summary>
        /// 删除好友响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void OnFriendRemove(NetConnection<NetSession> sender, FriendRemoveRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("UserService OnFriendRemove: :character:[{0}] FriendReletionID:[{1}]", character.Id, request.Id);
            sender.Session.Response.friendRemove = new FriendRemoveResponse();
            sender.Session.Response.friendRemove.Id = request.Id;

            //删除自己的好友
            if (character.FriendManager.RemoveFriendById(request.Id))
            {
                sender.Session.Response.friendRemove.Result = Result.Success;
                //删除别人好友中的自己
                //得到别人的角色Session信息
                var friend = SessionManager.Instance.GetSession(request.friendId);
                //Session如果不为null，则在线，否则不在线
                if (friend != null)
                {
                    //好友在线
                    friend.Session.Character.FriendManager.RemoveFriendByFriendId(character.Id);
                }
                else
                {
                    //不在线
                    this.RemoveFriend(request.friendId, character.Id);
                }
            }
            else
                sender.Session.Response.friendRemove.Result = Result.Failed;
        }

        /// <summary>
        /// 直接删除数据中的好友实体（不在线也可以用）
        /// </summary>
        /// <param name="charId">当前角色id</param>
        /// <param name="friendId">对方角色id</param>
        private void RemoveFriend(int charId, int friendId)
        {
            Log.InfoFormat("UserService RemoveFriend character {0} friend {1)", charId, friendId);
            //查找数据库中好友列表中的characterid和friendid的好友实体
            var removeItem = DBService.Instance.Entities.TCharacterFriends.FirstOrDefault(v => v.CharacterID == charId && v.FriendID == friendId);
            if (removeItem != null)
            {
                DBService.Instance.Entities.TCharacterFriends.Remove(removeItem);
            }
        }
    }
}
