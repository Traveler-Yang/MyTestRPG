using SeverCommon;
using SeverCommon.Data;
using GameServer.Entities;
using GameServer.Services;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Models
{
    class Map
    {
        internal class MapCharacter
        {
            public NetConnection<NetSession> connection;
            public Character character;

            public MapCharacter(NetConnection<NetSession> conn, Character cha)
            {
                this.connection = conn;
                this.character = cha;
            }
        }

        public int ID
        {
            get { return this.Define.ID; }
        }
        internal MapDefine Define;

        Dictionary<int, MapCharacter> MapCharacters = new Dictionary<int, MapCharacter>();

        /// <summary>
        /// 接收MapManager发送过来的数据
        /// </summary>
        /// <param name="define">数据</param>
        internal Map(MapDefine define)
        {
            this.Define = define;
        }

        internal void Update()
        {

        }
        /// <summary>
        /// 角色进入地图
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="character"></param>
        internal  void CharacterEnter(NetConnection<NetSession> conn, Character character)
        {
            Log.InfoFormat("CharacterEnter: Map:{0} CharacterID:{1}", this.Define.ID, character.Id);
            //将当前地图的id赋值给此角色所在的地图的id
            character.Info.mapId = this.ID;

            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.mapCharacterEnter = new MapCharacterEnterResponse();

            //告诉客户端，这个角色成功进入地图了，地图的ID是XX，角色是XX
            message.Response.mapCharacterEnter.mapId = this.Define.ID;//将读取到的地图ID赋值给要进入地图的角色的地图ID
            message.Response.mapCharacterEnter.Characters.Add(character.Info);
            //当角色进入游戏时同时也通知其他角色
            foreach (var kv in this.MapCharacters)
            {
                message.Response.mapCharacterEnter.Characters.Add(kv.Value.character.Info);
                //进入地图通知玩家
                this.SendCharacterEnterMap(kv.Value.connection, character.Info);
            }

            this.MapCharacters[character.Id] = new MapCharacter(conn, character);

            byte[] data = PackageHandler.PackMessage(message);//将创建成功的消息打包成字节流，发送给客户端
            conn.SendData(data, 0, data.Length);
        }

        /// <summary>
        /// 角色离开
        /// </summary>
        /// <param name="cha"></param>
        internal void CharacterLeave(Character cha)
        {
            Log.InfoFormat("CharacterLeave: Map{0} characterId{1}", this.Define.ID, cha.Id);//打印日志

            //角色离开，通知其他所有在线玩家
            foreach (var kv in this.MapCharacters)
            {
                this.SendCharacterLeaveMap(kv.Value.connection, cha);
            }
            this.MapCharacters.Remove(cha.Id);
        }

        /// <summary>
        /// 进入地图通知
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="character"></param>
        void SendCharacterEnterMap(NetConnection<NetSession> conn, NCharacterInfo character)
        {
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.mapCharacterEnter = new MapCharacterEnterResponse();
            message.Response.mapCharacterEnter.mapId = this.Define.ID;//得到配置表中读取到的地图ID发送到客户端
            message.Response.mapCharacterEnter.Characters.Add(character);

            byte[] data = PackageHandler.PackMessage(message);//将创建成功的消息打包成字节流，发送给客户端
            conn.SendData(data, 0, data.Length);
        }
        /// <summary>
        /// 离开地图通知
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="character"></param>
        private void SendCharacterLeaveMap(NetConnection<NetSession> conn, Character character)
        {
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.mapCharacterLeave = new MapCharacterLeaveResponse();

            message.Response.mapCharacterLeave.characterId = character.Id;
            byte[] data = PackageHandler.PackMessage(message);//将创建成功的消息打包成字节流，发送给客户端
            conn.SendData(data, 0, data.Length);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity"></param>
        public void UpdateEntity(NEntitySync entity)
        {
            //得到信息后，遍历发送给地图中的所有角色
            foreach (var kv in this.MapCharacters)
            {
                //判断这个地图中的所有角色，是否是自己，如果是自己
                //则将从客户端那里得到的自己的位置，更新到服务器
                if (kv.Value.character.entityId == entity.Id)
                {
                    kv.Value.character.Position = entity.Entity.Position;
                    kv.Value.character.Direction = entity.Entity.Direction;
                    kv.Value.character.Speed = entity.Entity.Speed;
                }
                else//如果不是我自己，则将自己的信息发送给其他角色
                {
                    MapService.Instance.SendEntityUpdate(kv.Value.connection, entity);
                }
            }
        }
    }
}
