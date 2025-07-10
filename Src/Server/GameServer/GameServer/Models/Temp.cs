using Common;
using Common.Utils;
using GameServer.Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Models
{
    class Temp
    {
        /// <summary>
        /// 队伍Id
        /// </summary>
        public int id;
        public Character leader;//队长

        /// <summary>
        /// 队伍成员列表
        /// </summary>
        public List<Character> Members = new List<Character>();

        /// <summary>
        /// 队伍信息的变更时间
        /// </summary>
        public double timestamp;

        public Temp(Character leader)
        {
            this.AddMember(leader);
        }

        /// <summary>
        /// 加入队伍
        /// </summary>
        /// <param name="member">成员</param>
        public void AddMember(Character member)
        {
            Log.InfoFormat("Temp > AddMember Character:{0}:{1}", member.Id, member.Info.Name);
            //如果队伍列表没有人的情况下，第一个添加进来的人就是队长
            if (this.Members.Count == 0)
            {
                this.leader = member;
            }
            //添加到队伍成员列表
            this.Members.Add(member);
            //将当前队伍指定给此成员
            member.temp = this;
            //记录时间戳
            timestamp = TimeUtil.timestamp;
        }

        /// <summary>
        /// 离开队伍
        /// </summary>
        /// <param name="member"></param>
        public void Leave(Character member)
        {
            Log.InfoFormat("Temp > Leave Character:{0}:{1}", member.Id, member.Info.Name);
            if (this.Members.Contains(member))
                this.Members.Remove(member);
            //判断这个人如果是队长
            if (this.leader == member)
            {
                //队伍中是否还有成员
                if (this.Members.Count > 0)
                    //如果还有，则将下一个人，设置为队长
                    this.leader = Members[0];
                else//否则，队长设置为null
                    this.leader = null;
            }
            //这个人的队伍信息为null
            member.temp = null;
            //记录时间戳
            timestamp = TimeUtil.timestamp;
        }

        /// <summary>
        /// Temp后处理
        /// </summary>
        /// <param name="message"></param>
        public void PostProcess(NetMessageResponse message)
        {
            if (message.tempInfo == null)
            {
                message.tempInfo = new TempInfoResponse();
                message.tempInfo.Result = Result.Success;
                message.tempInfo.Team = new NTempInfo();
                message.tempInfo.Team.Id = this.id;
                message.tempInfo.Team.Leader = this.leader.Id;
                foreach (var member in this.Members)
                {
                    message.tempInfo.Team.Members.Add(member.GetBasicInfo());
                }
            }
        }
    }
}
