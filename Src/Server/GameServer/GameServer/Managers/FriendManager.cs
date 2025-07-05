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
            foreach (var friend in this.Owner.TChar.Friends)
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
                FriendName = friend.TChar.Name,
                Class = friend.TChar.Class,
                Level = friend.TChar.Level,
            };
            //再Add到数据库中
            this.Owner.TChar.Friends.Add(tf);
            friendChanged = true;
        }

        /// <summary>
        /// 删除好友（删除别人好友中的自己）
        /// </summary>
        /// <param name="friendId">我的id（别人好友中的friendid）</param>
        /// <returns></returns>
        public bool RemoveFriendByFriendId(int friendId)
        {
            //查找所有好友列表中的FriendID为我自己的好友实体（如果是我自己，说明这个人的好友列表中有我自己）
            var removeItem = this.Owner.TChar.Friends.FirstOrDefault(v => v.FriendID == friendId);
            if (removeItem != null)
            {
                DBService.Instance.Entities.TCharacterFriends.Remove(removeItem);
            }
            friendChanged = true;
            return true;
        }

        /// <summary>
        /// 删除好友（删除自己好友列表中的好友）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool RemoveFriendById(int id)
        {
            var removeItem = this.Owner.TChar.Friends.FirstOrDefault(v => v.Id == id);
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
            //创建一个返回对象
            NFriendInfo friendInfo = new NFriendInfo();
            //查找有无这个角色信息
            var character = CharacterManager.Instance.GetCharacter(friend.FriendID);
            friendInfo.friendInfo = new NCharacterInfo();
            //将数据库中的唯一id赋值给网络信息
            friendInfo.Id = friend.Id;
            //如果没有，则不在线
            if (character == null)
            {
                //直接将数据中的信息进行赋值
                friendInfo.friendInfo.Id = friend.FriendID;
                friendInfo.friendInfo.Name = friend.FriendName;
                friendInfo.friendInfo.Class = (CharacterClass)friend.Class;
                friendInfo.friendInfo.Level = friend.Level;
                friendInfo.Status = false;
            }
            else//如果在线
            {
                //将这个角色在线信息进行赋值
                friendInfo.friendInfo = character.GetBasicInfo();
                friendInfo.friendInfo.Name = character.Info.Name;
                friendInfo.friendInfo.Class = character.Info.Class;
                friendInfo.friendInfo.Level = character.Info.Level;
                //如果当前等级和数据库中的等级不一致，则重新更新
                if (friend.Level != character.Info.Level)
                {
                    friend.Level = character.Info.Level;
                }
                character.FriendManager.UpdateFriendInfo(this.Owner.Info, true);
               friendInfo.Status = true;
            }
            Log.InfoFormat("{0}:{1} GetFriendInfo : {2}:{3} Status:{4}", this.Owner.Id, this.Owner.Info.Name, friendInfo.friendInfo.Id, friendInfo.friendInfo.Name, friendInfo.Status);
            return friendInfo;
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

        /// <summary>
        /// 下线通知
        /// </summary>
        public void OfflineNotify()
        {
            //遍历自己的所有好友
            foreach (var friendInfo in this.friends)
            {
                var friend = CharacterManager.Instance.GetCharacter(friendInfo.friendInfo.Id);
                //如果好友在线
                if (friend != null)
                {
                    //则将好友更新，设置为离线
                    friend.FriendManager.UpdateFriendInfo(this.Owner.Info, false);
                }
            }
        }

        /// <summary>
        /// 后处理（暂时只是更新好友列表信息）
        /// </summary>
        /// <param name="message"></param>
        public void PostProcess(NetMessageResponse message)
        {
            //好友信息是否变化
            if (friendChanged)
            {
                Log.InfoFormat("FriendManager > PostProcess ： Character：{0}:{1}", this,Owner.Id, this.Owner.Info.Name);
                //如果变化则更新好友列表
                this.InitFriends();
                if (message.friendList == null)
                {
                    //如果这个列表为空，则new一个
                    //并将更新的列表添加到消息列表中
                    message.friendList = new FriendListResponse();
                    message.friendList.Friends.AddRange(this.friends);
                }
                //复位变化，防止发送相同数据
                friendChanged = false;
            }
        }
    }
}
