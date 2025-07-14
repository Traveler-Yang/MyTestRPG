using GameServer.Entities;
using GameServer.Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Models
{
    class Chat
    {
        Character Owner;

        //当前的最新消息索引
        public int localIdx;
        public int worldIdx;
        public int systemIdx;
        public int tempIdx;
        public int guildIdx;

        public Chat(Character owner)
        {
            this.Owner = owner;
        }

        public void PostProcess(NetMessageResponse message)
        {
            //构建网络消息
            if (message.Chat == null)
            {
                message.Chat = new ChatResponse();
                message.Chat.Result = Result.Success;
            }
            this.localIdx = ChatManager.Instance.GetLocalMessage(this.Owner.Info.mapId, this.localIdx, message.Chat.localMessages);
            this.worldIdx = ChatManager.Instance.GetWorldMessage(this.worldIdx, message.Chat.worldMessages);
            this.systemIdx = ChatManager.Instance.GetSystemdMessage(this.systemIdx, message.Chat.systemMessages);
            if (this.Owner.temp != null)
            {
                this.tempIdx = ChatManager.Instance.GetTempMessage(this.Owner.temp.id, this.tempIdx, message.Chat.tempMessages);
            }
            if (this.Owner.guild != null)
            {
                this.guildIdx = ChatManager.Instance.GetGuildMessage(this.Owner.guild.Id, this.guildIdx, message.Chat.guildMessages);
            }
        }
    }
}
