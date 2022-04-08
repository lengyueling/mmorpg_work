using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Common;

namespace GameServer.Services
{
    class DBService : Singleton<DBService>
    {
        ExtremeWorldEntities entities;

        public ExtremeWorldEntities Entities
        {
            get { return this.entities; }
        }

        public void Init()
        {
            entities = new ExtremeWorldEntities();
        }

        /// <summary>
        /// 异步/同步保存DB
        /// 异步保存执行保存命令以后继续做其他的事情
        /// 可以加一个计时器，几秒钟统一保存一次
        /// </summary>
        internal void Save(bool async = false)
        {
            if (async)
            {
                entities.SaveChangesAsync();
            }
            else
            {
                entities.SaveChanges();
            }
            
        }
    }
}
