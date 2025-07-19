using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GameServer;
using GameServer.Entities;
using GameServer.Services;


//using GameServer.Entities;
using SkillBridge.Message;

namespace Network
{
    class NetSession : INetSession
    {
        public TUser User { get; set; }
        public Character Character { get; set; }
        public NEntity Entity { get; set; }
        /// <summary>
        /// 后处理器
        /// </summary>
        public IPostResponser PostResponser { get; set; }

        public void Disconnected()
        {
            this.PostResponser = null;
            if (this.Character != null && GameServer.Managers.CharacterManager.Instance.Characters.ContainsKey(Character.TChar.ID))
                UserService.Instance.CharacterLeave(this.Character);
        }


        NetMessage response;

        public NetMessageResponse Response
        {
            get
            {
                if (response == null)
                {
                    response = new NetMessage();
                }
                if (response.Response == null)
                    response.Response = new NetMessageResponse();
                return response.Response;
            }
        }

        public byte[] GetResponse()
        {
            if (response != null)
            {
                //如果当前会话中有后处理对象，就让他执行他的PostProcess()方法
                if (PostResponser != null)
                    this.PostResponser.PostProcess(Response);
                //打包成字节流
                byte[] data = PackageHandler.PackMessage(response);
                //将response置空，防止发送相同数据或发送残留旧数据
                response = null;
                return data;
            }
            return null;
        }
    }
}
