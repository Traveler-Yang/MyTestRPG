using Common;
using GameServer.Entities;
using GameServer.Models;
using GameServer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class TempManager : Singleton<TempManager>
    {
        /// <summary>
        /// 所有的队伍
        /// </summary>
        public List<Temp> Temps = new List<Temp>();
        /// <summary>
        /// 用来查询
        /// </summary>
        public Dictionary<int, Temp> CharacterTemps = new Dictionary<int, Temp>();

        public void Init()
        {

        }

        public Temp GetTempByCharacter(int characterId)
        {
            Temp temp = null;
            this.CharacterTemps.TryGetValue(characterId, out temp);
            return temp;
        }

        /// <summary>
        /// 加入队伍
        /// </summary>
        /// <param name="leader">队长</param>
        /// <param name="member">成员</param>
        public void AddTempMember(Character leader, Character member)
        {
            //如果发起者（队长）没有队伍
            //则是第一次组队
            if (leader.temp == null)
            {
                //则将创建一个新队伍
                leader.temp = CreateTemp(leader);
            }
            //有队伍，将角色添加进去
            if (member != null) leader.temp.AddMember(member);
        }

        /// <summary>
        /// 创建新队伍
        /// </summary>
        /// <param name="leader"></param>
        /// <returns></returns>
        private Temp CreateTemp(Character leader)
        {
            Temp temp = null;
            for (int i = 0; i < this.Temps.Count; i++)
            {
                temp = this.Temps[i];
                //判断队伍是否是空的
                if (temp.Members.Count == 0)
                {
                    //如果是空的，则就把自己添加进去，
                    //并用这个队伍
                    temp.AddMember(leader);
                    return temp;
                }
            }
            //如果遍历完了，没有找到空的可用的队伍，就创建新队伍
            temp = new Temp(leader);
            this.Temps.Add(temp);
            temp.id = this.Temps.Count;
            return temp;
        }
    }
}
