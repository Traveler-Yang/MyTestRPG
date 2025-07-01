using Common;
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
        /// 属于谁的好友列表
        /// </summary>
        Character Owner;

        /// <summary>
        /// 好友列表
        /// </summary>
        List<NFriendInfo> friends = new List<NFriendInfo>();

        /// <summary>
        /// 好友是否变化
        /// </summary>
        bool friendChanged = false;

        public FriendManager(Character owner)
        {
            this.Owner = owner;
            InitFriends();
        }

        public void GetFriendInfos(List<NFriendInfo> list)
        {
            foreach (var f in friends)
            {
                list.Add(f);
            }
        }

        /// <summary>
        /// 初始化好友列表
        /// </summary>
        public void InitFriends()
        {
            //清空一次列表
            this.friends.Clear();
            //再从数据库中的好友列表加载进来
            foreach (var friend in this.Owner.Data.Friends)
            {
                this.friends.Add(GetFriendInfo(friend));
            }
        }

        /// <summary>
        /// 添加好友（给数据库中添加）
        /// </summary>
        /// <param name="friend">需要Add的角色</param>
        public void AddFriend(Character friend)
        {
            //new一个好友，并将信息添加进去
            TCharacterFriend tf = new TCharacterFriend
            {
                FriendID = friend.Id,
                FriendName = friend.Data.Name,
                Class = friend.Data.Class,
                Level = friend.Data.Level,
            };
            //再Add到数据库中
            this.Owner.Data.Friends.Add(tf);
            friendChanged = true;
        }

        /// <summary>
        /// 删除好友
        /// </summary>
        /// <param name="friendId">好友id</param>
        /// <returns></returns>
        public bool RemoveFriendByFriendId(int friendId)
        {
            var removeItem = this.Owner.Data.Friends.FirstOrDefault(v => v.FriendID == friendId);
            if (removeItem != null)
            {
                DBService.Instance.Entities.TCharacterFriends.Remove(removeItem);
            }
            friendChanged = true;
            return true;
        }

        /// <summary>
        /// 删除好友（直接将数据库中的好友实体移除掉）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool RemoveFriendById(int id)
        {
            var removeItem = this.Owner.Data.Friends.FirstOrDefault(v => v.Id == id);
            if (removeItem != null)
            {
                DBService.Instance.Entities.TCharacterFriends.Remove(removeItem);
            }
            friendChanged = true;
            return true;
        }

        /// <summary>
        /// 将数据库的信息转换为网络信息
        /// </summary>
        /// <param name="friend"></param>
        /// <returns></returns>
        public NFriendInfo GetFriendInfo(TCharacterFriend friend)
        {
            NFriendInfo friendInfo = new NFriendInfo();
            var character = CharacterManager.Instance.GetCharacter(friend.FriendID);
            friendInfo.friendInfo = new NCharacterInfo();
            friendInfo.Id = friend.Id;
            if (character == null)
            {
                friendInfo.friendInfo.Id = friend.FriendID;
                friendInfo.friendInfo.Name = friend.FriendName;
                friendInfo.friendInfo.Class = (CharacterClass)friend.Class;
                friendInfo.friendInfo.Level = friend.Level;
                friendInfo.Status = false;
            }
            else
            {
                friendInfo.friendInfo = GetBasicInfo(character.Info);
                friendInfo.friendInfo.Name = character.Info.Name;
                friendInfo.friendInfo.Class = character.Info.Class;
                friendInfo.friendInfo.Level = character.Info.Level;
                character.FriendManager.UpdateFriendInfo(this.Owner.Info, true);
                friendInfo.Status = false;
            }
            return friendInfo;
        }

        NCharacterInfo GetBasicInfo(NCharacterInfo info)
        {
            return new NCharacterInfo()
            {
                Id = info.Id,
                Name = info.Name,
                Class = info.Class,
                Level = info.Level,
            };
        }

        /// <summary>
        /// 获取好友信息（根据ID获得当前好友信息）
        /// </summary>
        /// <param name="friendId"></param>
        /// <returns></returns>
        public NFriendInfo GetFriendInfo(int friendId)
        {
            foreach (var f in this.friends)
            {
                if (f.friendInfo.Id == friendId)
                {
                    return f;
                }
            }
            return null;
        }

        /// <summary>
        /// 更新好友状态
        /// </summary>
        /// <param name="friendInfo"></param>
        /// <param name="status">状态（在线或离线）</param>
        public void UpdateFriendInfo(NCharacterInfo friendInfo, bool status)
        {
            //循环遍历好友列表
            foreach (var f in friends)
            {
                //找到一样的角色
                if (f.friendInfo.Id == friendInfo.Id)
                {
                    //改变状态
                    f.Status = status;
                    break;
                }
            }
            this.friendChanged = true;
        }

        public void PostProcess(NetMessageResponse message)
        {
            if (friendChanged)
            {
                this.InitFriends();
                if (message.friendList == null)
                {
                    message.friendList = new FriendListResponse();
                    message.friendList.Friends.AddRange(this.friends);
                }
                friendChanged = false;
            }
        }
    }
}
