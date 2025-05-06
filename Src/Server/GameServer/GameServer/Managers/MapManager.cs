using Common;
using GameServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class MapManager : Singleton<MapManager>
    {
        Dictionary<int, Map> Maps = new Dictionary<int, Map>();//地图列表

        public void Init()
        {
            //将读取配置表中得到的所有地图信息打印输出到服务器控制台
            foreach (var mapdefine in DataManager.Instance.Maps.Values)
            {
                Map map = new Map(mapdefine);//在执行Map的构造函数的时候，直接将读取到的地图表发送给Map
                Log.InfoFormat("MapManager.Init > Map:{0}:{1}", map.Define.ID, map.Define.Name);
                this.Maps[mapdefine.ID] = map;
            }
        }

        public Map this[int Key]
        {
            get 
            {
                return this.Maps[Key]; 
            }
        }
        /// <summary>
        /// 实时更新地图
        /// </summary>
        public void Update()
        {
            //遍历所有的地图，进行更新
            foreach (var map in this.Maps.Values)
            {
                map.Update();
            }
        }
    }
}
