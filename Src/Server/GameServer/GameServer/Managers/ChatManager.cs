using Common;
using Common.Utils;
using GameServer.Entities;
using SkillBridge.Message;
using System.Collections.Generic;

namespace GameServer.Managers
{
    class ChatManager : Singleton<ChatManager>
    {
        public List<ChatMessage> System = new List<ChatMessage>();//系统消息
        public List<ChatMessage> World = new List<ChatMessage>();//世界消息
        public Dictionary<int, List<ChatMessage>> Local = new Dictionary<int, List<ChatMessage>>();//本地消息
        public Dictionary<int, List<ChatMessage>> Temp = new Dictionary<int, List<ChatMessage>>();//队伍消息
        public Dictionary<int, List<ChatMessage>> Guild = new Dictionary<int, List<ChatMessage>>();//公会消息

        public void Init()
        {

        }

        /// <summary>
        /// 添加消息
        /// </summary>
        /// <param name="from">消息的来源</param>
        /// <param name="message">消息信息</param>
        public void AddMessage(Character from, ChatMessage message)
        {
            //填充消息
            message.FromId = from.Id;
            message.FromName = from.Name;
            message.Time = TimeUtil.timestamp;
            switch (message.Channel)
            {
                case ChatChannel.Local:
                    this.AddLocalMessage(from.Info.mapId, message);
                    break;
                case ChatChannel.World:
                    this.AddWorldMessage(message);
                    break;
                case ChatChannel.System:
                    this.AddSystemMessage(message);
                    break;
                case ChatChannel.Temp:
                    this.AddTempMessage(from.temp.id, message);
                    break;
                case ChatChannel.Guild:
                    this.AddGuildMessage(from.guild.Id, message);
                    break;
            }
        }

        /// <summary>
        /// 添加本地消息
        /// </summary>
        /// <param name="mapId"></param>
        /// <param name="message"></param>
        public void AddLocalMessage(int mapId, ChatMessage message)
        {
            //查询有无此消息
            if (!this.Local.TryGetValue(mapId, out List<ChatMessage> messages))
            {
                //如果没有，则构建新消息
                messages = new List<ChatMessage>();
                this.Local[mapId] = messages;
            }
            messages.Add(message);
        }

        /// <summary>
        /// 添加世界消息
        /// </summary>
        /// <param name="message"></param>
        public void AddWorldMessage(ChatMessage message)
        {
            this.World.Add(message);
        }

        /// <summary>
        /// 添加系统消息
        /// </summary>
        /// <param name="message"></param>
        public void AddSystemMessage(ChatMessage message)
        {
            this.System.Add(message);
        }

        /// <summary>
        /// 添加队伍消息
        /// </summary>
        /// <param name="tempId"></param>
        /// <param name="message"></param>
        public void AddTempMessage(int tempId, ChatMessage message)
        {
            //查询有无此消息
            if (!this.Temp.TryGetValue(tempId, out List<ChatMessage> messages))
            {
                //如果没有，则构建新消息
                messages = new List<ChatMessage>();
                this.Temp[tempId] = messages;
            }
            messages.Add(message);
        }

        /// <summary>
        /// 添加公会消息
        /// </summary>
        /// <param name="guildId"></param>
        /// <param name="message"></param>
        public void AddGuildMessage(int guildId, ChatMessage message)
        {
            //查询有无此消息
            if (!this.Guild.TryGetValue(guildId, out List<ChatMessage> messages))
            {
                //如果没有，则构建新消息
                messages = new List<ChatMessage>();
                this.Guild[guildId] = messages;
            }
            messages.Add(message);
        }

        public int GetLocalMessage(int mapId, int idx, List<ChatMessage> result)
        {
            if (!this.Local.TryGetValue(mapId, out List<ChatMessage> messages))
            {
                return 0;
            }
            return GetNewMessage(idx, result, messages);
        }

        public int GetWorldMessage(int idx, List<ChatMessage> result)
        {
            return GetNewMessage(idx, result, this.World);
        }

        public int GetSystemdMessage(int idx, List<ChatMessage> result)
        {
            return GetNewMessage(idx, result, this.System);
        }

        public int GetTempMessage(int tempId, int idx, List<ChatMessage> result)
        {
            if (!this.Temp.TryGetValue(tempId, out List<ChatMessage> messages))
            {
                return 0;
            }
            return GetNewMessage(idx, result, messages);
        }

        public int GetGuildMessage(int guildId, int idx, List<ChatMessage> result)
        {
            if (!this.Guild.TryGetValue(guildId, out List<ChatMessage> messages))
            {
                return 0;
            }
            return GetNewMessage(idx, result, messages);
        }

        private int GetNewMessage(int idx, List<ChatMessage> result, List<ChatMessage> messages)
        {
            if (idx == 0)
            {
                //如果当前消息总数超过最大限制
                if (messages.Count > GameDefine.MaxChatRecoredNums)
                {
                    //则，将总数减去最大限制数，得到索引
                    idx = messages.Count - GameDefine.MaxChatRecoredNums;
                }
            }
            //得到最新的消息
            for (; idx < messages.Count; idx++)
            {
                result.Add(messages[idx]);
            }
            return idx;
        }
    }
}
