

using Entities;
using SkillBridge.Message;
using System.Collections.Generic;

namespace Managers
{
    interface IEntityNotify
    {
        void OnEntityRemoved();
        void OnEntityChanged(Entity entity);
        void OnEntityEvent(EntityEvent @event, int param);
    }
    class EntityManager : Singleton<EntityManager>
    {
        //本地的实体列表
        Dictionary<int, Entity> entities = new Dictionary<int, Entity>();

        Dictionary<int, IEntityNotify> notifiers = new Dictionary<int, IEntityNotify>();

        public void RegisterEntityChangeNotify(int eneityId, IEntityNotify notify)
        {
            this.notifiers[ eneityId ] = notify;
        }

        public void AddEntity(Entity entity)
        {
            entities[entity.entityId] = entity;
        }

        public void RemoveEntity(Entity entity)
        {
            this.entities.Remove(entity.entityId);
            if (notifiers.ContainsKey(entity.entityId))
            {
                notifiers[entity.entityId].OnEntityRemoved();
                notifiers.Remove(entity.entityId);
            }
        }

        public void OnEntitySync(NEntitySync data)
        {
            Entity entity = null;
            //找一下本地实体列表中是否有这个(data)角色，并将此角色取出来
            entities.TryGetValue(data.Id, out entity);
            if (entity != null)
            {
                if (data.Entity != null)
                    entity.EntityData = data.Entity;
                //找到了这个角色，并且给这个角色赋上值了
                //就可以通知了
                if (notifiers.ContainsKey(data.Id))
                {
                    notifiers[entity.entityId].OnEntityChanged(entity);//通知entity发生改变
                    notifiers[entity.entityId].OnEntityEvent(data.Event, data.Param);//通知entity的状态发生变化
                }
            }
        }

    }
}
