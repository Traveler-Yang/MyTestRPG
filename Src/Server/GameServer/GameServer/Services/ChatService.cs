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
    class ChatService : Singleton<ChatService>
    {
        public ChatService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ChatRequest>(this.OnChat);
        }

        public void Init()
        {
            ChatManager.Instance.Init();
        }

        private void OnChat(NetConnection<NetSession> sender, ChatRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("CharService OnChat : : Character:{0};Channel:{1};Message:{2}", character.Id, request.Message.Channel, request.Message.Message);
            //如果是私聊消息
            if (request.Message.Channel == ChatChannel.Private)
            {
                //查看对方在不在线
                var chatTo = SessionManager.Instance.GetSession(request.Message.ToId);
                if (chatTo == null)
                {
                    //不在线
                    sender.Session.Response.Chat = new ChatResponse();
                    sender.Session.Response.Chat.Result = Result.Failed;
                    sender.Session.Response.Chat.Errormsg = "对方不在线";
                    sender.Session.Response.Chat.privateMessages.Add(request.Message);
                    sender.SendResPonse();
                }
                else
                {
                    //在线
                    //构建消息，发送给他
                    if (chatTo.Session.Response.Chat == null)
                    {
                        chatTo.Session.Response.Chat= new ChatResponse();
                    }
                    request.Message.FromId = character.Id;
                    request.Message.FromName = character.Name;
                    chatTo.Session.Response.Chat.Result = Result.Success;
                    chatTo.Session.Response.Chat.privateMessages.Add(request.Message);
                    chatTo.SendResPonse();

                    //再发回给自己
                    if (sender.Session.Response.Chat == null)
                    {
                        sender.Session.Response.Chat = new ChatResponse();
                    }
                    sender.Session.Response.Chat.Result = Result.Success;
                    sender.Session.Response.Chat.privateMessages.Add(request.Message);
                    sender.SendResPonse();
                }
            }
            else
            {
                //不是私聊
                sender.Session.Response.Chat = new ChatResponse();
                sender.Session.Response.Chat.Result = Result.Success;
                ChatManager.Instance.AddMessage(character, request.Message);
                sender.SendResPonse();
            }
        }
    }
}
