using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Data;
using GameServer.Models;

namespace GameServer.Managers
{
    class Spawner
    {
        public SpawnRuleDefine Define { get; set; }

        private Map Map;
        /// <summary>
        /// 刷新时间
        /// </summary>
        private float SpawnTime = 0;

        /// <summary>
        /// 消失时间
        /// </summary>
        private float unSpawnTime = 0;

        /// <summary>
        /// 是否刷过了
        /// </summary>
        private bool spawned = false;

        private SpawnPointDefine spawnPoint = null;

        public Spawner(SpawnRuleDefine define, Map map)
        {
            this.Define = define;
            this.Map = map;
            //判断表中是否有这个地图
            if (DataManager.Instance.SpawnPoints.ContainsKey(this.Map.ID))
            {
                //再判断这个地图中是否有这个刷怪点
                if (DataManager.Instance.SpawnPoints[this.Map.ID].ContainsKey(this.Define.SpawnPoint))
                {
                    //如果有，则直接将刷怪点取出来
                    spawnPoint = DataManager.Instance.SpawnPoints[this.Map.ID][this.Define.SpawnPoint];
                }
                else
                {
                    Log.ErrorFormat("SpawnRule[{0}] SpawnPoint[{1}] not existed", this.Define.ID, this.Define.SpawnPoint);
                }
            }
        }

        public void Update()
        {
            //每一帧都判断一下可不可以刷怪
            if (this.CanSpawn())
                this.Spawn();
        }

        /// <summary>
        /// 是否可以刷怪
        /// </summary>
        /// <returns></returns>
        bool CanSpawn()
        {
            //判断刷过怪物了吗
            if (this.spawned)
                return false;
            //消失时间加上周期时间是否大于当前时间，如果大于则表示它已经被杀死了
            if (this.unSpawnTime + this.Define.SpawnPeriod > Time.time)
                return false;

            return true;
        }

        /// <summary>
        /// 刷怪
        /// </summary>
        public void Spawn()
        {
            this.spawned = true;
            Log.InfoFormat("Map[{0}]Spawn[{1}:Mon:{2},Lv[{3}]] At Point:{4}", this.Define.MapID, this.Define.ID, this.Define.SpawnMonID, this.Define.SpawnLevel, this.Define.SpawnPoint);
            this.Map.MonsterManager.Create(this.Define.SpawnMonID, this.Define.SpawnLevel, this.spawnPoint.Position, this.spawnPoint.Direction);
        }
    }
}
