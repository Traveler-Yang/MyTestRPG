using Common.Data;
using Managers;
using Models;
using Network;
using SkillBridge.Message;
using System;
using UnityEngine;

namespace Services
{
    class MapService : Singleton<MapService>, IDisposable
    {
        public MapService()
        {
            MessageDistributer.Instance.Subscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);//角色进入地图的消息
            MessageDistributer.Instance.Subscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);//离开地图
            MessageDistributer.Instance.Subscribe<MapEntitySyncResponse>(this.OnMapEntitySync);
        }

        
        public int CurrentMapId = 0;

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);//取消订阅
            MessageDistributer.Instance.Unsubscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);
        }

        public void Init()
        {

        }

        /// <summary>
        /// 角色进入地图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="response"></param>
        private void OnMapCharacterEnter(object sender, MapCharacterEnterResponse response)
        {
            Debug.LogFormat("OnMapCharacterEnter:{0} [{1}]", response.mapId, response.Characters.Count);
            foreach (var cha in response.Characters)
            {
                //判断当前列表中的角色是否是自己 或者 是否等于null
                if (User.Instance.CurrentCharacter == null || (cha.Type == CharacterType.Player && User.Instance.CurrentCharacter.Id == cha.Id))
                {
                    //当前角色切换地图
                    User.Instance.CurrentCharacter = cha;
                }
                CharacterManager.Instance.AddCharacter(cha);//将进入地图的所有角色，发送给角色管理器
            }
            if (CurrentMapId != response.mapId)
            {
                this.EnterMap(response.mapId);
                this.CurrentMapId = response.mapId;
            }
        }

        /// <summary>
        /// 角色离开地图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="response"></param>
        private void OnMapCharacterLeave(object sender, MapCharacterLeaveResponse response)
        {
            Debug.LogFormat("OnMapCharacterLeave: characterID:{0}", response.entityId);//打印哪个角色离开地图的日志
            //判断从服务端发送过来的要离开的角色，是不是我自己
            //如果不是我自己，则移除那个角色
            //如果是我自己，则清除所有角色
            if (response.entityId != User.Instance.CurrentCharacter.EntityId)
                CharacterManager.Instance.RemoveCharacter(response.entityId);
            else
                CharacterManager.Instance.Clear();
        }

        private void EnterMap(int mapId)
        {
            if (DataManager.Instance.Maps.ContainsKey(mapId))
            {
                MapDefine map = DataManager.Instance.Maps[mapId];
                User.Instance.CurrentMapData = map;//在加载地图前，将地图资源赋值给map
                SceneManager.Instance.LoadScene(map.Resource);//加载地图
                SoundManager.Instance.PlayMusic(map.Music);//播放地图音乐
            }
            else
            {
                Debug.LogErrorFormat("EnterMap: Map {0} not extsted",mapId);
            }
        }

        /// <summary>
        /// 发送实体的同步信息
        /// </summary>
        /// <param name="entityEvent"></param>
        /// <param name="entity"></param>
        public void SendMapEntitySync(EntityEvent entityEvent, NEntity entity, int param)
        {
            //Debug.LogFormat("MapEntityUpdateRequest :ID{0} POS:{1} DIR:{2} SPD:{3}", entity.Id, entity.Position.ToString(), entity.Direction.ToString(), entity.Speed);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.mapEntitySync = new MapEntitySyncRequest();
            //将id、Event、Entity发送给服务器
            //告诉服务器我当前的状态和位置信息等
            message.Request.mapEntitySync.entitySync = new NEntitySync()
            {
                Id = entity.Id,//id信息
                Event = entityEvent,//状态信息：移动、停止、跳跃。。。
                Entity = entity,//id、位置、方向、速度
                Param = param//坐骑信息
            };
            //发送给服务器
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 接收从服务端发送过来的所有得到 当前角色信息 的 所有角色信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="response"></param>
        private void OnMapEntitySync(object sender, MapEntitySyncResponse response)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendFormat("MapEntityUpdateResponse: Entity:{0}", response.entitySyncs.Count);
            sb.AppendLine();
            foreach (var entity in response.entitySyncs)
            {
                //告诉所有entity，有角色同步了
                EntityManager.Instance.OnEntitySync(entity);
                sb.AppendFormat("    [{0}]evt:{1} entity{2}", entity.Id, entity.Event, entity.Entity.String());
                sb.AppendLine();
            }
            Debug.Log(sb.ToString());
        }

        /// <summary>
        /// 发送传送请求到服务器
        /// </summary>
        /// <param name="teleporterID"></param>
        internal void SendMapTeleport(int teleporterID)
        {
            Debug.LogFormat("MapTeleportRequest: TeleporterID:{0}", teleporterID);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.mapTeleport = new MapTeleportRequest();
            message.Request.mapTeleport.teleporterId = teleporterID; //传送点ID
            NetClient.Instance.SendMessage(message); //发送给服务器
        }
    }
}
