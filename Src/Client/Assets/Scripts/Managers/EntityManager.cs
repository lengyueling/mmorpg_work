using Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Managers
{
    interface IEntityNotify
    {
        /// <summary>
        /// 移除实体
        /// </summary>
        void OnEntityRemoved();
        /// <summary>
        /// 设置实体数据变化
        /// </summary>
        /// <param name="entity"></param>
        void OnEntityChange(Entity entity);
        /// <summary>
        /// 设置实体状态变化
        /// </summary>
        /// <param name="event"></param>
        void OnEntityEvent(EntityEvent @event);
    }
    class EntityManager : Singleton<EntityManager>
    {
        Dictionary<int, Entity> entities = new Dictionary<int, Entity>();
        Dictionary<int, IEntityNotify> notifiers = new Dictionary<int, IEntityNotify>();

        public void RegisterEntityChangeNotify(int entityId, IEntityNotify notify)
        {
            this.notifiers[entityId] = notify;
        }

        /// <summary>
        /// 增加实体
        /// </summary>
        /// <param name="entity"></param>
        public void AddEntity(Entity entity)
        {
            entities[entity.entityId] = entity;
        }
        
        /// <summary>
        /// 移除实体
        /// </summary>
        /// <param name="entity"></param>
        public void RemoveEntity(NEntity entity)
        {
            this.entities.Remove(entity.Id);
            if (notifiers.ContainsKey(entity.Id))
            {
                notifiers[entity.Id].OnEntityRemoved();
                notifiers.Remove(entity.Id);
            }
        }

        /// <summary>
        /// 当实体同步时
        /// </summary>
        /// <param name="data"></param>
        internal void OnEntitySync(NEntitySync data)
        {
            Entity entity = null;
            entities.TryGetValue(data.Id, out entity);
            if (entity != null)
            {
                if (data.Entity != null)
                {
                    entity.EntityData = data.Entity;
                }
                if (notifiers.ContainsKey(data.Id))
                {
                    notifiers[entity.entityId].OnEntityChange(entity);
                    notifiers[entity.entityId].OnEntityEvent(data.Event);
                }
            }
        }
    }
}