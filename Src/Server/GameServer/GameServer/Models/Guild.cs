using GameServer.Entities;
using GameServer.Managers;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Models
{
    class Guild
    {
        /// <summary>
        /// 公会Id
        /// </summary>
        public int Id { get { return this.Data.Id; } }

        public Character leader;//会长
        /// <summary>
        /// 公会名字
        /// </summary>
        public string Name { get { return this.Data.Name; } }

         public string Icon { get { return this.Data.Icon; } }

        /// <summary>
        /// 公会成员列表
        /// </summary>
        public List<Character> Members = new List<Character>();

        /// <summary>
        /// 更新变化时间戳
        /// </summary>
        public double timestape;

        public TGuild Data;

         public Guild(TGuild guild)
        {
            this.Data = guild;
        }

        /// <summary>
        /// 加入公会申请信息保存
        /// </summary>
        /// <param name="apply"></param>
        /// <returns></returns>
        public bool JoinApply(NGuildApplyInfo apply)
        {
            var oldApply = this.Data.Applies.FirstOrDefault(v => v.CharacterID == apply.characterId);
            //查找数据库中有无相同的申请信息
            if (oldApply != null)
            {
                //如果有，则返回false
                return false;
            }
            //如果没有此申请
            //构建一个新的
            var dbApply = DBService.Instance.Entities.TGuildApplies.Create();
            dbApply.GuildId = apply.GuildId;
            dbApply.CharacterID = apply.characterId;
            dbApply.Name = apply.characterName;
            dbApply.Class = apply.Class;
            dbApply.Level = apply.Level;
            dbApply.ApplyTime = DateTime.Now;//当前时间
            //添加 保存到数据库
            DBService.Instance.Entities.TGuildApplies.Add(dbApply);
            this.Data.Applies.Add(dbApply);
            DBService.Instance.Save();

            this.timestape = Time.timestamp;
            return true;

        }

        /// <summary>
        /// 加入公会效验
        /// </summary>
        /// <param name="apply"></param>
        /// <returns></returns>
        public bool JoinAppove(NGuildApplyInfo apply)
        {
            var oldApply = this.Data.Applies.FirstOrDefault(v => v.CharacterID == apply.characterId && v.Result == 0);
            //查找数据库中有无相同的申请信息
            if (oldApply == null)
            {
                //如果没有查到，则返回false
                return false;
            }

            oldApply.Result = (int)apply.Result;
            //如果是接收
            if (apply.Result == ApplyResult.Accept)
            {
                //把此角色，添加到公会中
                this.AddMember(apply.characterId, apply.characterName, apply.Class, apply.Level, GuildDuty.None);
            }
            //并保存
            DBService.Instance.Save();

            this.timestape = Time.timestamp;
            return true;
        }

        /// <summary>
        /// 添加成员
        /// </summary>
        /// <param name="characterId"></param>
        /// <param name="characterName"></param>
        /// <param name="class"></param>
        /// <param name="level"></param>
        /// <param name="duty"></param>
        private void AddMember(int characterId, string characterName, int @class, int level, GuildDuty duty)
        {
            DateTime now = new DateTime();
            TGuildMember dbMemeber = new TGuildMember
            {
                CharacterID = characterId,
                Name = characterName,
                Class = @class,
                Level = level,
                Duty = (int)duty,
                JoinTime = now,
                LastTime = now
            };
            this.Data.Members.Add(dbMemeber);
            this.timestape = Time.timestamp;
        }

        /// <summary>
        /// 成员离开
        /// </summary>
        /// <param name="character"></param>
        public void Leave(Character character)
        {
            
        }

        /// <summary>
        /// 公会后处理
        /// </summary>
        /// <param name="character"></param>
        /// <param name="message"></param>
        public void Posprocess(Character character, NetMessageResponse message)
        {
            if (message.Guild == null)
            {
                message.Guild = new GuildResponse();
                message.Guild.Result = Result.Success;
                message.Guild.guildInfo = this.GuildInfo(character);
            }
        }

        public NGuildInfo GuildInfo(Character character)
        {
            NGuildInfo info =  new NGuildInfo()
            {
                Id = this.Id,
                GuildName = this.Name,
                GuildIcon = this.Icon,
                Notice = this.Data.Notice,
                leaderId = this.Data.LeaderID,
                leaderName = this.Data.LeaderName,
                createTime = (long)Time.GetTimestamp(this.Data.CreateTime),
                memberCount = this.Data.Members.Count,
            };
            //在character有值时，他才会是公会成员
            if (character != null)
            {
                //只有是成员才可以查看公会信息
                info.Members.AddRange(GetMemberInfos());
                //判断这个人是否是队长，只有是队长才可以查看申请信息
                if (character.Id == this.Data.LeaderID)
                    info.Applies.AddRange(GetApplyInfos());
            }
            return info;
        }

        private List<NGuildMemberInfo> GetMemberInfos()
        {
            List<NGuildMemberInfo> members = new List<NGuildMemberInfo>();

            foreach (var member in this.Data.Members)
            {
                var memberInfo = new NGuildMemberInfo
                {
                    Id = member.Id,
                    characterId = member.CharacterID,
                    Duty = (GuildDuty)member.Duty,
                    joinTime = (long)Time.GetTimestamp(member.JoinTime),
                    lastTime = (long)Time.GetTimestamp(member.LastTime),
                };

                var character = CharacterManager.Instance.GetCharacter(member.CharacterID);
                if (character != null)//角色在线
                {
                    //更新一下成员信息
                    memberInfo.charInfo = character.GetBasicInfo();
                    memberInfo.Status = true;
                    member.Level = character.TChar.Level;
                    member.Name = character.TChar.Name;
                    member.LastTime = DateTime.Now;
                    if (member.Id == this.Data.LeaderID)//如果当前成员是会长，则让会长赋值给当前会长信息
                        this.leader = character;
                }
                else//角色离线
                {
                    memberInfo.charInfo = this.GetMemberInfo(member);
                    memberInfo.Status = false;
                    if (member.Id == this.Data.LeaderID)//如果当前成员是会长，则赋值为null
                        this.leader = null;
                }
                members.Add(memberInfo);
            }
            return members;
        }

        private NCharacterInfo GetMemberInfo(TGuildMember member)
        {
            throw new NotImplementedException();
        }

        private List<NGuildApplyInfo> GetApplyInfos()
        {
            throw new NotImplementedException();
        }
    }
}
