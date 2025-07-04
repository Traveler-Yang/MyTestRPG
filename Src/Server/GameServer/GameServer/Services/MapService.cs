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
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapTeleportRequest>(this.OnMapTeleport);

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
            if (character != null)
            {
                //这行中最后的.string()可以把角色的信息完整的打印出来，位置、方向、速度
                //Log.InfoFormat("OnMapEntitySync: characterID:{0} : {1} Entity.Id:{2} Evt:{3} Entity:{4}", character.Id, character.Info.Name, request.entitySync.Id, request.entitySync.Event, request.entitySync.Entity.String());
                //通过地图管理器来找到这个角色所在的当前地图
                MapManager.Instance[character.Info.mapId].UpdateEntity(request.entitySync);

            }
        }

        /// <summary>
        /// 接收从地图那里传过来的当前角色信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="entity"></param>
        public void SendEntityUpdate(NetConnection<NetSession> conn, NEntitySync entity)
        {
            conn.Session.Response.mapEntitySync = new MapEntitySyncResponse();
            conn.Session.Response.mapEntitySync.entitySyncs.Add(entity);
            conn.SendResPonse();
        }

        /// <summary>
        /// 接受客户端传过来的地图传送请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnMapTeleport(NetConnection<NetSession> sender, MapTeleportRequest request)
        {
            //获取当前角色
            Character character = sender.Session.Character;
            Log.InfoFormat("OnMapTeleport：CharacterID：{0}：{1} TeleporterID：{2}", character.Id, character.TChar, request.teleporterId);
            //如果传送点的ID在传送点列表中不存在，则不进行传送
            //ContainsKey 该方法判断字典中是否包含指定的键，如果包含则返回true，否则返回false
            if (!DataManager.Instance.Teleporters.ContainsKey(request.teleporterId))
            {
                Log.WarningFormat("Source TeleporterID:{0} not existed", request.teleporterId);
                return;
            }
            TeleporterDefine source = DataManager.Instance.Teleporters[request.teleporterId];//获取传送点信息
            //如果传送点的LinkTo为0，或者传送点的LinkTo在传送点列表中存在，则可以进行传送
            if (source.LinkTo == 0 || DataManager.Instance.Teleporters.ContainsKey(source.LinkTo))
            {
                Log.WarningFormat("Source TeleporterID: [{0}] LinkTo ID [{1}] not existed", request.teleporterId, source.LinkTo);
            }

            TeleporterDefine target = DataManager.Instance.Teleporters[source.LinkTo];//获取目标传送点信息

            MapManager.Instance[source.MapID].CharacterLeave(character);//让角色离开当前地图
            character.Position = target.Position;//将角色的位置设置为目标传送点的位置
            character.Direction = target.Direction;//将角色的方向设置为目标传送点的方向
            MapManager.Instance[target.MapID].CharacterEnter(sender, character);//让角色进入目标地图
        }

    }
}
