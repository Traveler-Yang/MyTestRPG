using Common;
using GameServer.Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;

namespace GameServer.Managers
{
    internal class EntityManager : Singleton<EntityManager>
    {
        //列表索引
        private int idx = 0;
        /// <summary>
        /// 所有的实体列表
        /// </summary>
        public List<Entity> AllEntitieys = new List<Entity>();
        /// <summary>
        /// 实体字典，是指每个地图中的所有实体
        /// </summary>
        public Dictionary<int, List<Entity>> MapEntities = new Dictionary<int, List<Entity>>();

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="mapId">添加到哪个地图</param>
        /// <param name="entity">所添加的实体</param>
        public void AddEntity(int mapId, Entity entity)
        {
            //添加实体到列表
            AllEntitieys.Add(entity);
            //实体加入到管理器中，生成唯一ID
            entity.EntityData.Id = ++this.idx;

            //实体列表
            List<Entity> entities = null;
            //判断地图字典中的哪张地图
            //如果已经存在，则直接添加
            //如果这个地图不存在，则创建一个新的实体列表，并添加进去
            if (!MapEntities.TryGetValue(mapId, out entities))
            {
                //在堆中开辟一个内存，存储这个实体列表，并存储到地图字典中
                entities = new List<Entity>();
                MapEntities[mapId] = entities;
            }
            entities.Add(entity);
        }

        /// <summary>
        /// 移除实体
        /// </summary>
        /// <param name="mapId">从哪个地图中移除</param>
        /// <param name="entity">移除哪个实体</param>
        public void RemoveEntity(int mapId, Entity entity)
        {
            AllEntitieys.Remove(entity);
            MapEntities[mapId].Remove(entity);
        }

        internal void OnEntitySync(NEntitySync entity)
        {
            
        }
    }
}
