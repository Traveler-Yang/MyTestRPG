using GameServer.Entities;
using GameServer.Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class MonsterManager
    {
        private Map map;

        /// <summary>
        /// 所有的怪物
        /// </summary>
        public Dictionary<int, Monster> Monsters = new Dictionary<int, Monster>();

        public void Init(Map map)
        {
            this.map = map;
        }

        /// <summary>
        /// 创建一只怪物
        /// </summary>
        /// <param name="spawnMonID">怪物ID（什么怪物）</param>
        /// <param name="spawnLevel">怪物等级</param>
        /// <param name="position">怪物生成地点</param>
        /// <param name="direction">怪物生成方向</param>
        /// <returns></returns>
        internal Monster Create(int spawnMonID, int spawnLevel, NVector3 position, NVector3 direction)
        {
            Monster monster = new Monster(spawnMonID, spawnLevel, position, direction);//构建一个Monster
            EntityManager.Instance.AddEntity(map.ID, monster);//添加到EntityManager里面所维护的实体列表中
            monster.Info.Id = monster.entityId;//设置Monster的基本信息ID
            monster.Info.EntityId = monster.entityId;//设置Monster的EntityId
            monster.Info.mapId = map.ID;//地图ID
            Monsters[monster.Id] = monster;//并添加到MonsterManager中所维护的Monsters字典中

            this.map.MonsterEnter(monster);
            return monster;
        }
    }
}
