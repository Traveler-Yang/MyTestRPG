using GameServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    /// <summary>
    /// 刷怪管理器
    /// </summary>
    class SpawnManager
    {
        private List<Spawner> Rules = new List<Spawner>();

        private Map Map;

        public void Init(Map map)
        {
            //读取刷怪规则
            //得到配置表中的某个地图的规则
            this.Map = map;
            if (DataManager.Instance.SpawnRules.ContainsKey(map.Define.ID))
            {
                //生成刷怪器
                foreach (var define in DataManager.Instance.SpawnRules[map.Define.ID].Values)
                {
                    this.Rules.Add(new Spawner(define, this.Map));
                }
            }

        }

        public void Update()
        {
            if (Rules.Count == 0)
                return;

            for (int i = 0; i < this.Rules.Count; i++)
            {
                Rules[i].Update();
            }
        }
    }
}
