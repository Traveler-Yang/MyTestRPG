using GameServer.Entities;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class FriendManager
    {
        /// <summary>
        /// 所有者
        /// </summary>
        Character Owner;
        /// <summary>
        /// 好友列表
        /// </summary>
        public List<NFriendInfo> friends = new List<NFriendInfo>();
        /// <summary>
        /// 是否变化
        /// </summary>
        bool friendChanged = false;

        public FriendManager(Character owner)
        {
            this.Owner = owner;
            InitFriends();
        }

        public void GetFriendInfos(List<NFriendInfo> friends)
        {
            foreach (var f in this.friends)
            {
                friends.Add(f);
            }
        }

        public void InitFriends()
        {
            friends.Clear();
            foreach (var friend in this.Owner.TChar.Friends)
            {
                friends.Add(GetFriendInfo(friend));
            }
        }

        /// <summary>
        /// 更新状态信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="status"></param>
        public void UpdateFriendStatus(NCharacterInfo info, bool status)
        {
            foreach (var f in friends)
            {
                if (f.Info.Id == info.Id)
                {
                    f.Status = status;
                }
            }
            this.friendChanged = true;
        }

        /// <summary>
        /// 下线通知，自己下线，通知自己好友列表中的其他人列表中的自己的状态
        /// </summary>
        public void OffLineNotify()
        {
            foreach (var f in friends)
            {
                var friendInfo = CharacterManager.Instance.GetCharacter(f.Info.Id);
                if (friendInfo != null)
                {
                    friendInfo.FriendManager.UpdateFriendStatus(this.Owner.Info, false);
                }
            }
            friendChanged = true;
        }

        /// <summary>
        /// 数据库信息转换为网络信息
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private NFriendInfo GetFriendInfo(TCharacterFriend friend)
        {
            //定义返回数据
            NFriendInfo nFriendInfo = new NFriendInfo();
            Character character = CharacterManager.Instance.GetCharacter(friend.FriendID);
            nFriendInfo.Info = new NCharacterInfo();
            nFriendInfo.Id = friend.Id;
            if (character == null)//不在线
            {
                //从数据库拿数据
                nFriendInfo.Info.Id = friend.FriendID;
                nFriendInfo.Info.Name = friend.FriendName;
                nFriendInfo.Info.Level = friend.Level;
                nFriendInfo.Info.Class = (CharacterClass)friend.Class;
                nFriendInfo.Status = false;
            }
            else//在线
            {
                nFriendInfo.Info = character.GetBasicInfo();
                nFriendInfo.Info.Name = character.Name;
                nFriendInfo.Info.Level = character.Info.Level;
                nFriendInfo.Info.Class = character.Info.Class;
                character.FriendManager.UpdateFriendStatus(this.Owner.Info, true);
                if (friend.Level != character.Info.Level)
                {
                    friend.Level = character.Info.Level;
                }
                nFriendInfo.Status = true;
            }
            return nFriendInfo;
        }

        /// <summary>
        /// 添加好友
        /// </summary>
        /// <param name="toId">目标id</param>
        public void AddFriend(int toId)
        {
            TCharacterFriend friend = new TCharacterFriend()
            {
                CharacterID = this.Owner.Info.Id,
                FriendID = CharacterManager.Instance.GetCharacter(toId).Id,
                FriendName = CharacterManager.Instance.GetCharacter(toId).Name,
                Level = CharacterManager.Instance.GetCharacter(toId).Info.Level,
                Class = (int)CharacterManager.Instance.GetCharacter(toId).Info.Class
            };
            this.Owner.TChar.Friends.Add(friend);
            friendChanged = true;
        }

        /// <summary>
        /// 删除好友
        /// </summary>
        /// <param name="toId"></param>
        public void RemoveFriend(int toId)
        {
            var remFriend = this.Owner.TChar.Friends.FirstOrDefault(f => f.FriendID == toId);
            if (remFriend != null)
            {
                DBService.Instance.Entities.TCharacterFriends.Remove(remFriend);
                friendChanged = true;
            }
        }

        /// <summary>
        /// 后处理器
        /// </summary>
        /// <param name="message"></param>
        public void PostProcess(NetMessageResponse message)
        {
            if (friendChanged)
            {
                InitFriends();
                if (message.friendList == null)
                {
                    message.friendList = new FriendListResponse();
                    message.friendList.Friends.AddRange(friends);
                }
                friendChanged = false;
            }
        }
    }
}
