using Common;
using Common.Data;
using GameServer.Entities;
using GameServer.Managers;
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

        /// <summary>
        /// 地图中的角色 以CharacterId为Key
        /// </summary>
        Dictionary<int, MapCharacter> MapCharacters = new Dictionary<int, MapCharacter>();

        /// <summary>
        /// 刷怪管理器
        /// </summary>
        SpawnManager spawnManager = new SpawnManager();

        public MonsterManager MonsterManager = new MonsterManager();

        /// <summary>
        /// 接收MapManager发送过来的数据
        /// </summary>
        /// <param name="define">数据</param>
        internal Map(MapDefine define)
        {
            this.Define = define;
            this.spawnManager.Init(this);
            this.MonsterManager.Init(this);
        }

        internal void Update()
        {
            spawnManager.Update();
        }
        /// <summary>
        /// 角色进入地图
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="character"></param>
        internal  void CharacterEnter(NetConnection<NetSession> conn, Character character)
        {
            Log.InfoFormat("[GameServer] Map CharacterEnter: Map:{0} CharacterID:{1}", this.Define.ID, character.Id);
            //将当前地图的id赋值给此角色所在的地图的id
            character.Info.mapId = this.ID;
            this.MapCharacters[character.Id] = new MapCharacter(conn, character);

            conn.Session.Response.mapCharacterEnter = new MapCharacterEnterResponse();
            conn.Session.Response.mapCharacterEnter.mapId = this.Define.ID;//将读取到的地图ID赋值给要进入地图的角色的地图ID

            //当角色进入游戏时同时也通知其他角色
            foreach (var kv in this.MapCharacters)
            {
                conn.Session.Response.mapCharacterEnter.Characters.Add(kv.Value.character.Info);
                if (kv.Value.character != character)
                    //进入地图通知玩家
                    this.AddCharacterEnterMap(kv.Value.connection, character.Info);
            }
            //Monster进入地图通知其他角色
            foreach (var kv in this.MonsterManager.Monsters)
            {
                conn.Session.Response.mapCharacterEnter.Characters.Add(kv.Value.Info);
            }
            conn.SendResPonse();
        }

        /// <summary>
        /// 角色离开
        /// </summary>
        /// <param name="cha"></param>
        internal void CharacterLeave(Character cha)
        {
            Log.InfoFormat("[GameServer] Map CharacterLeave: Map{0} characterId{1}", this.Define.ID, cha.Id);

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
        void AddCharacterEnterMap(NetConnection<NetSession> conn, NCharacterInfo character)
        {
            Log.InfoFormat("[GameServer] Map AddCharacterEnterMap To {0}:{1} : Map {2} : Character {3}", conn.Session.Character.Id, conn.Session.Character.Info.Name, this.Define.ID, character.Id, character.Name);
            if (conn.Session.Response.mapCharacterEnter == null)
            {
                conn.Session.Response.mapCharacterEnter = new MapCharacterEnterResponse();
                conn.Session.Response.mapCharacterEnter.mapId = this.Define.ID;//得到配置表中读取到的地图ID发送到客户端
            }
            conn.Session.Response.mapCharacterEnter.Characters.Add(character);
            conn.SendResPonse();
        }
        /// <summary>
        /// 离开地图通知
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="character"></param>
        private void SendCharacterLeaveMap(NetConnection<NetSession> conn, Character character)
        {
            Log.InfoFormat("[GameServer] Map SendCharacterLeaveMap: To {0}:{1} : Map {2} : Character {3}", conn.Session.Character.Id,conn.Session.Character.Info.Name,this.Define.ID, character.Id, character.Info.Name);
            conn.Session.Response.mapCharacterLeave = new MapCharacterLeaveResponse();
            conn.Session.Response.mapCharacterLeave.entityId = character.entityId;
            conn.SendResPonse();
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity"></param>
        public void UpdateEntity(NEntitySync entity)
        {
            //Log.InfoFormat("[GameServer] Map UpdateEntity: entity {0} : Event {1}", entity.Id, entity.Event);
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
                    if (entity.Event == EntityEvent.Ride)
                    {
                        kv.Value.character.Ride = entity.Param;
                    }
                }
                else//如果不是我自己，则将自己的信息发送给其他角色
                {
                    MapService.Instance.SendEntityUpdate(kv.Value.connection, entity);
                }
            }
        }

        /// <summary>
        /// 怪物进入地图
        /// </summary>
        /// <param name="monster">需要进入的怪物</param>
        internal void MonsterEnter(Monster monster)
        {
            Log.InfoFormat("[GameServer] Map MonsterEnter：Map:{0} MonsterID:{1}", this.Define.ID, monster.Id);
            foreach (var kv in this.MapCharacters)
            {
                this.AddCharacterEnterMap(kv.Value.connection, monster.Info);
            }
        }
    }
}
