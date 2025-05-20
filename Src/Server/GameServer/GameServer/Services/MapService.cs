using Common;
using Common.Data;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    class MapService : Singleton<MapService>
    {
        public MapService()
        {
            //注册消息
            //MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapCharacterEnterRequest>(this.OnMapCharacterEnter);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapEntitySyncRequest>(this.OnMapEntitySync);

        }
    
        public void Init()
        {
            MapManager.Instance.Init();
        }


        //private void OnMapCharacterEnter(NetConnection<NetSession> sender, MapCharacterEnterRequest message)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// 接收客户端发送过来的角色信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void OnMapEntitySync(NetConnection<NetSession> sender, MapEntitySyncRequest request)
        {
            Character character = sender.Session.Character;
            //这行中最后的.string()可以把角色的信息完整的打印出来，位置、方向、速度
            Log.InfoFormat("OnMapEntitySync: characterID:{0} : {1} Entity.Id:{2} Evt:{3} Entity:{4}", character.Id, character.Info.Name, request.entitySync.Id, request.entitySync.Event, request.entitySync.Entity.String());
            //通过地图管理器来找到这个角色所在的当前地图
            MapManager.Instance[character.Info.mapId].UpdateEntity(request.entitySync);
        }

        /// <summary>
        /// 接收从地图那里传过来的当前角色信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="entity"></param>
        public void SendEntityUpdate(NetConnection<NetSession> conn, NEntitySync entity)
        {
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.mapEntitySync = new MapEntitySyncResponse();
            message.Response.mapEntitySync.entitySyncs.Add(entity);

            byte[] data = PackageHandler.PackMessage(message);//将创建成功的消息打包成字节流，发送给客户端
            conn.SendData(data, 0, data.Length);
        }
    }
}
