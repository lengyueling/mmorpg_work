using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Models;
using GameServer.Entities;
using SkillBridge.Message;
using Common.Data;
using Common;

namespace GameServer.Managers
{
    /// <summary>
    /// 刷怪器
    /// 每个刷怪器对应一个地图中的一个刷怪规则
    /// </summary>
    class Spawner
    {
        public SpawnRuleDefine Define { get; set; }

        private Map Map;

        /// <summary>
        /// 刷新时间
        /// </summary>
        private float spawnTime = 0;

        /// <summary>
        /// 怪物上一次被杀死的时间
        /// </summary>
        private float unspawnTime = 0;

        private bool spawned = false;

        private SpawnPointDefine spawnPoint = null;

        /// <summary>
        /// 构造函数
        /// 初始化地图、配置表以初始化刷怪点
        /// </summary>
        /// <param name="define"></param>
        /// <param name="map"></param>
        public Spawner(SpawnRuleDefine define, Map map)
        {
            this.Define = define;
            this.Map = map;

            if (DataManager.Instance.SpawnPoints.ContainsKey(this.Map.ID))
            {
                if (DataManager.Instance.SpawnPoints[this.Map.ID].ContainsKey(this.Define.SpawnPoint))
                {
                    spawnPoint = DataManager.Instance.SpawnPoints[this.Map.ID][this.Define.SpawnPoint];
                }
                else
                {
                    Log.ErrorFormat("MapID[{0}] SpawnRule[{1}] SpawnPoint[{2}] not existed", this.Map.ID, this.Define.ID, this.Define.SpawnPoint);
                }
            }
        }

        public void Update()
        {
            if (this.CanSpawn())
            {
                this.Spawn();
            }
        }

        /// <summary>
        /// 是否可以刷怪
        /// </summary>
        /// <returns></returns>
        private bool CanSpawn()
        {
            if (this.spawned)
            {
                return false;
            }
            if (this.unspawnTime + this.Define.SpawnPeriod > Time.time)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 刷怪
        /// </summary>
        /// <returns></returns>
        public void Spawn()
        {
            this.spawned = true;
            Log.InfoFormat("Map[{0}]Spawn[{1}]:Mon:{2},lv:{3} At Point:{4}", this.Define.MapID, this.Define.ID, this.Define.SpawnMonID, this.Define.SpawnLevel, this.Define.SpawnPoint);
            this.Map.MonsterManager.Create(this.Define.SpawnMonID, this.Define.SpawnLevel, this.spawnPoint.Position, this.spawnPoint.Direction);
        }
    }
}
