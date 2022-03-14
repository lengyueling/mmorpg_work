using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Models;

namespace GameServer.Managers
{
    class MapManager : Singleton<MapManager>
    {
        public Dictionary<int, Map> Maps = new Dictionary<int, Map>();

        /// <summary>
        /// 将地图信息从Json文件传过来进行初始化
        /// </summary>
        public void Init()
        {
            foreach (var mapdefine in DataManager.Instance.Maps.Values)
            {
                Map map = new Map(mapdefine);
                Log.InfoFormat("MapManager.Init > Map:{0}:{1}", map.Define.ID, map.Define.Name);
                this.Maps[mapdefine.ID] = map;
            }
        }

        /// <summary>
        /// this 作为索引器
        /// (MapManager.Instance[dbchar.MapID].CharacterEnter(sender, character); ==
        /// MapManager.Instance.Maps[dbchar.MapID].CharacterEnter(sender, character);)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Map this[int key]
        {
            get
            {
                return this.Maps[key];
            }
        }

        /// <summary>
        /// 更新地图信息
        /// 地图中可能会有一些变化比如刷怪之类
        /// 地图管理器存在自主服务
        /// </summary>
        public void Update()
        {
            foreach(var map in this.Maps.Values)
            {
                map.Update();
            }
        }
    }
}
