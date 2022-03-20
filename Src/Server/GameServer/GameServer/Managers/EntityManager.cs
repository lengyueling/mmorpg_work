using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Entities;
using SkillBridge.Message;

namespace GameServer.Managers
{
    class EntityManager : Singleton<EntityManager>
    {
        private int idx = 0;
        /// <summary>
        /// 所有实体的集合
        /// </summary>
        public List<Entity> AllEntities = new List<Entity>();
        /// <summary>
        /// 每个地图为单位的实体集合
        /// </summary>
        public Dictionary<int, List<Entity>> MapEntities = new Dictionary<int, List<Entity>>();

        public void AddEntity(int mapid,Entity entity)
        {
            AllEntities.Add(entity);
            //加入管理器生成唯一ID
            entity.EntityData.Id = ++this.idx;

            List<Entity> entities = null;
            if (!MapEntities.TryGetValue(mapid,out entities))
            {
                entities = new List<Entity>();
                MapEntities[mapid] = entities;
            }
            entities.Add(entity);
        }

        public void RemoveEntity(int mapId, Entity entity)
        {
            this.AllEntities.Remove(entity);
            this.MapEntities[mapId].Remove(entity);
        }

    }
}
