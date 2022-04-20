using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Models;
using GameServer.Entities;
using SkillBridge.Message;

namespace GameServer.Managers   
{
    class MonsterManager
    {
        private Map Map;

        public Dictionary<int, Monster> Monsters = new Dictionary<int, Monster>();

        /// <summary>
        /// 初始化怪物管理器
        /// </summary>
        /// <param name="map">返回是哪一张地图</param>
        public void Init(Map map)
        {
            this.Map = map;
        }

        /// <summary>
        /// 创建怪物（逻辑类似创建角色）
        /// </summary>
        /// <param name="spawnMonID"></param>
        /// <param name="spawnLevel"></param>
        /// <param name="position"></param>
        /// <param name="directrion"></param>
        /// <returns></returns>
        internal Monster Create(int spawnMonID,int spawnLevel, NVector3 position, NVector3 directrion)
        {
            Monster monster = new Monster(spawnMonID, spawnLevel, position, directrion);
            EntityManager.Instance.AddEntity(this.Map.ID, monster);
            monster.Id = monster.entityId;
            monster.Info.EntityId = monster.entityId;
            monster.Info.mapId = this.Map.ID;
            Monsters[monster.Id] = monster;
            this.Map.MonsterEnter(monster);
            return monster;
        }
    }
}
