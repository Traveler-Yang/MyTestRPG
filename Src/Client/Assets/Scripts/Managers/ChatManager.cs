using Models;
using Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Managers
{
    class ChatManager : Singleton<ChatManager>
    {
        public enum LocalChannel
        {
            All = 0,    //所有
            Local = 1,  //本地
            World = 2,  //世界
            Temp = 3,   //队伍
            Guild = 4,  //公会
            Private = 5,//私聊
        }

        /// <summary>
        /// 本地的所有频道类型
        /// </summary>
        public ChatChannel[] ChannelFilter = new ChatChannel[6]
        {
            ChatChannel.Local | ChatChannel.World | ChatChannel.Guild | ChatChannel.Temp | ChatChannel.Private | ChatChannel.System,//所有频道
            ChatChannel.Local,
            ChatChannel.World,
            ChatChannel.Temp,
            ChatChannel.Guild,
            ChatChannel.Private,
        };

        /// <summary>
        /// 发起私聊
        /// </summary>
        /// <param name="targetId"></param>
        /// <param name="targetName"></param>
        public void StartPrivateChat(int targetId, string targetName)
        {
            PrivateID = targetId;
            PrivateName = targetName;

            this.sendChannel = LocalChannel.Private;
            if (this.OnChat != null)
                this.OnChat();
        }

        /// <summary>
        /// 本地的所有聊天信息
        /// </summary>
        public List<ChatMessage>[] Messages = new List<ChatMessage>[6]
        {
            new List<ChatMessage>(),
            new List<ChatMessage>(),
            new List<ChatMessage>(),
            new List<ChatMessage>(),
            new List<ChatMessage>(),
            new List<ChatMessage>(),
        };

        /// <summary>
        /// 当前显示的频道
        /// </summary>
        public LocalChannel displayChannel;

        /// <summary>
        /// 本地的当前频道类型
        /// </summary>
        public LocalChannel sendChannel;

        public int PrivateID = 0;
        public string PrivateName = "";

        public ChatChannel SendChannel
        {
            get
            {
                switch (sendChannel)
                {
                    case LocalChannel.Local:
                        return ChatChannel.Local;
                    case LocalChannel.World:
                        return ChatChannel.World;
                    case LocalChannel.Temp:
                        return ChatChannel.Temp;
                    case LocalChannel.Guild:
                        return ChatChannel.Guild;
                    case LocalChannel.Private:
                        return ChatChannel.Private;
                    default:
                        break;
                }
                return ChatChannel.Local;
            }
        }

        public Action OnChat { get; internal set; }

        public void Init()
        {
            foreach (var message in Messages)
            {
                message.Clear();
            }
        }

        public void SendChat(string content, int toId = 0, string toName = "")
        {
            ChatService.Instance.SendChat(this.SendChannel, content, toId, toName);
        }

        /// <summary>
        /// 设置频道
        /// </summary>
        /// <param name="channel">要设置的频道类型</param>
        /// <returns>是否设置成功</returns>
        internal bool SetSendChannel(LocalChannel channel)
        {
            //如果是组队频道
            if (channel == LocalChannel.Temp)
            {
                if (User.Instance.TempInfo == null)
                {
                    this.AddSystemMessage("你没有加入任何队伍");
                    return false;
                }
            }
            //公会频道
            if (channel == LocalChannel.Guild)
            {
                if (User.Instance.CurrentCharacter.Guild == null)
                {
                    this.AddSystemMessage("你没有加入任何公会");
                    return false;
                }
            }
            this.sendChannel = channel;
            Debug.LogFormat("Set Channel：{0}", this.sendChannel);
            return true;
        }

        public void AddMessage(ChatChannel channel, List<ChatMessage> messages)
        {
            for (int ch = 0; ch < 6; ch++)
            {
                //循环遍历所有的频道与要添加的消息的类型进行判断，如果遇到相同的，则Add进去
                if ((this.ChannelFilter[ch] & channel) == channel)
                {
                    this.Messages[ch].AddRange(messages);
                }
            }
            if (this.OnChat != null)
                this.OnChat();//添加完成系统消息，并刷新聊天
        }

        /// <summary>
        /// Add系统信息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="from"></param>
        public void AddSystemMessage(string message, string from = "")
        {
            this.Messages[(int)LocalChannel.All].Add(new ChatMessage
            {
                Channel = ChatChannel.System,
                Message = message,
                FromName = from,
            });
            if (this.OnChat != null)
                this.OnChat();//添加完成系统消息，并刷新聊天
        }

        /// <summary>
        /// 获取消息信息
        /// </summary>
        /// <returns></returns>
        public string GetCurrentMassage()
        {
            StringBuilder sb = new StringBuilder();
            //遍历所有的消息列表
            foreach (var message in this.Messages[(int)displayChannel])
            {
                sb.AppendLine(FormatMessage(message));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 消息转换格式化文本
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private string FormatMessage(ChatMessage message)
        {
            switch (message.Channel)
            {
                case ChatChannel.Local:
                    return string.Format("【本地】{0}:{1}", FormatFromPlayer(message), message.Message);
                case ChatChannel.World:
                    return string.Format("<#FF8000>【世界】{0}:{1}</color>", FormatFromPlayer(message), message.Message);
                case ChatChannel.System:
                    return string.Format("<#FFFF00>【系统】{0}</color>", message.Message);
                case ChatChannel.Private:
                    return string.Format("<#FF00FF>【私聊】{0}:{1}</color>", FormatFromPlayer(message), message.Message);
                case ChatChannel.Temp:
                    return string.Format("<#00FF00>【队伍】{0}:{1}</color>", FormatFromPlayer(message), message.Message);
                case ChatChannel.Guild:
                    return string.Format("<#0000FF>【公会】{0}:{1}</color>", FormatFromPlayer(message), message.Message);
            }
            return "";
        }

        /// <summary>
        /// 转换格式化文本（发送消息的角色）
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private string FormatFromPlayer(ChatMessage message)
        {
            //如果这个消息的信息来源是我自己发的，则设置为我自己的标头
            if (message.FromId == User.Instance.CurrentCharacter.Id)
            {
                //return "<a name=\"\" class=\"player\">[我]</a>";
                return "<link=\"\"><#00FFE0><u>我</u></color></link>";
            }
            else
                //return string.Format("<a name=\"c:{0}:{1}\" class=\"player\">[{1}]</a>", message.FromId, message.FromName);
                return string.Format("<link=\"{0}:{1}\"><#00FFE0><u>{1}</u></color></link>", message.FromId, message.FromName);
        }

    }
}
