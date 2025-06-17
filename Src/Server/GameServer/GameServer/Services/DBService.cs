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
        /// 保存信息到数据库
        /// </summary>
        /// <param name="async">是否是同步保存</param>
        public void Save(bool async = false)
        {
            if(async)
                entities.SaveChangesAsync();
            else
                entities.SaveChanges();
        }
    }
}
