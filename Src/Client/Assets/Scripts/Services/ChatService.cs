using Common.Utils;
using Managers;
using Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine;

namespace Services
{
    class ChatService : Singleton<ChatService>, IDisposable
    {

        public ChatService()
        {
            MessageDistributer.Instance.Subscribe<ChatResponse>(this.OnChat);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<ChatResponse>(this.OnChat);
        }

        public void Init()
        {

        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="sendChannel"></param>
        /// <param name="content"></param>
        /// <param name="toId"></param>
        /// <param name="toName"></param>
        public void SendChat(ChatChannel channel, string content, int toId, string toName)
        {
            Debug.LogFormat("ChatService SenedChat : : Channel:{0} ToCharacter:{1}{2}",channel, toId, toName);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.Chat = new ChatRequest();
            message.Request.Chat.Message = new ChatMessage();
            message.Request.Chat.Message.Channel = channel;
            message.Request.Chat.Message.FromId = User.Instance.CurrentCharacter.Id;
            message.Request.Chat.Message.FromName = User.Instance.CurrentCharacter.Name;
            message.Request.Chat.Message.ToId = toId;
            message.Request.Chat.Message.ToName = toName;
            message.Request.Chat.Message.Message = content;
            message.Request.Chat.Message.Time = TimeUtil.timestamp;
            NetClient.Instance.SendMessage(message);
        }

        private void OnChat(object sender, ChatResponse response)
        {
            Debug.LogFormat("ChatService OnChat : : Result:{0} Errormsg:{1}", response.Result, response.Errormsg);
            if (response.Result == Result.Success)
            {
                ChatManager.Instance.AddMessage(ChatChannel.Local, response.localMessages);
                ChatManager.Instance.AddMessage(ChatChannel.World, response.worldMessages);
                ChatManager.Instance.AddMessage(ChatChannel.System, response.systemMessages);
                ChatManager.Instance.AddMessage(ChatChannel.Private, response.privateMessages);
                ChatManager.Instance.AddMessage(ChatChannel.Temp, response.tempMessages);
                ChatManager.Instance.AddMessage(ChatChannel.Guild, response.guildMessages);
            }
            else
            {
                ChatManager.Instance.AddSystemMessage(response.Errormsg);
            }
        }
    }
}
