using Common;
using Common.Data;
using GameServer.Entities;
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

            character.Info.mapId = this.ID;

            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.mapCharacterEnter = new  MapCharacterEnterResponse();

            message.Response.mapCharacterEnter.mapId = this.Define.ID;//将读取到的地图ID赋值给要进入地图的角色的地图ID
            message.Response.mapCharacterEnter.Characters.Add(character.Info);
            //当角色进入游戏时同时也通知其他角色
            foreach (var kv in this.MapCharacters)
            {
                message.Response.mapCharacterEnter.Characters.Add(kv.Value.character.Info);
                this.SendCharacterEnterMap(kv.Value.connection, character.Info);
            }

            this.MapCharacters[character.Id] = new MapCharacter(conn, character);

            byte[] data = PackageHandler.PackMessage(message);//将创建成功的消息打包成字节流，发送给客户端
            conn.SendData(data, 0, data.Length);
        }

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
    }
}
