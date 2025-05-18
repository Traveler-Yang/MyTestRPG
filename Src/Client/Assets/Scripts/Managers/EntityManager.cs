

using Entities;
using System.Collections.Generic;

namespace Managers
{
    interface IEntityNotify
    {
        void OnEntityRemoved();
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
    }
}
