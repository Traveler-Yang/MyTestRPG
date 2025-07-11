using Common;
using Common.Utils;
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

        /// <summary>
        /// 公会名字
        /// </summary>
        public string Name { get { return this.Data.Name; } }

         public string Icon { get { return this.Data.Icon; } }
        
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

            this.timestape = TimeUtil.timestamp;
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
                DBService.Instance.Entities.TGuildApplies.Remove(oldApply);

            }
            //并保存
            DBService.Instance.Save();

            this.timestape = TimeUtil.timestamp;
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
        public void AddMember(int characterId, string characterName, int @class, int level, GuildDuty duty)
        {
            DateTime now = DateTime.Now;
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
            this.Data.Members.Add(dbMemeber);//添加到DB中的成员列表中
            Character member = CharacterManager.Instance.GetCharacter(characterId);
            if (member != null)//判断这个角色是否在线
                member.TChar.GuildId = this.Id;//在线，则直接让这个角色身上的公会id赋值给当前公会
            else
            {
                //不在线，则使用DB查询此角色，再给这个角色的公会id赋值未当前公会
                //DBService.Instance.Entities.Database.ExecuteSqlCommand("uPDATE characters SET GuildId = @p0 WHERE CharacterId = @p1", this.Id, characterId);
                TCharacter dbchar = DBService.Instance.Entities.Characters.SingleOrDefault(c => c.ID == characterId);
                dbchar.GuildId = this.Id;
            }
                this.timestape = TimeUtil.timestamp;
        }

        /// <summary>
        /// 成员离开
        /// </summary>
        /// <param name="character"></param>
        public bool Leave(Character character)
        {
            //如果是会长，则不允许直接离开
            if (character.TChar.ID == this.Data.LeaderID)
                return false;
            RemoveMember(character.TChar.ID);
            var cha = CharacterManager.Instance.GetCharacter(character.Id);
            if (cha != null)
            {
                //在线
                cha.TChar.GuildId = 0;
            }
            else
            {
                TCharacter dbchar = DBService.Instance.Entities.Characters.SingleOrDefault(c => c.ID == character.TChar.ID);
                dbchar.GuildId = 0;
            }
            DBService.Instance.Save();
            timestape = TimeUtil.timestamp;
            return true;
        }

        /// <summary>
        /// 移除角色
        /// </summary>
        /// <param name="characterId"></param>
        public void RemoveMember(int characterId)
        {
            //查找数据库中的要离开的成员
            TGuildMember member = this.Data.Members.FirstOrDefault(m => m.CharacterID == characterId);
            if (member != null)
                DBService.Instance.Entities.TGuildMembers.Remove(member);
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

        /// <summary>
        /// 获取NGuildInfo信息
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public NGuildInfo GuildInfo(Character from)
        {
            NGuildInfo info =  new NGuildInfo()
            {
                Id = this.Id,
                GuildName = this.Name,
                GuildIcon = this.Icon,
                Notice = this.Data.Notice,
                leaderId = this.Data.LeaderID,
                leaderName = this.Data.LeaderName,
                createTime = (long)TimeUtil.GetTimestamp(this.Data.CreateTime),
                memberCount = this.Data.Members.Count,
            };
            //在from有值时，他才会是公会成员
            //如果from是null，则只获得公会信息
            if (from != null)
            {
                //只有是成员才可以查看公会信息
                info.Members.AddRange(GetMemberInfos());
                //判断这个人是否是队长，只有是队长才可以查看申请信息
                if (from.Id == this.Data.LeaderID)
                    info.Applies.AddRange(GetApplyInfos());
            }
            return info;
        }

        /// <summary>
        /// 获取公会成员列表信息（DB改网络）
        /// </summary>
        /// <returns></returns>
        private List<NGuildMemberInfo> GetMemberInfos()
        {
            //构建返回值
            List<NGuildMemberInfo> members = new List<NGuildMemberInfo>();
            //循环遍历当前公会的数据公会成员列表
            foreach (var member in this.Data.Members)
            {
                var memberInfo = new NGuildMemberInfo
                {
                    Id = member.Id,
                    characterId = member.CharacterID,
                    Duty = (GuildDuty)member.Duty,
                    joinTime = (long)TimeUtil.GetTimestamp(member.JoinTime),
                    lastTime = (long)TimeUtil.GetTimestamp(member.LastTime),
                };
                //查看此角色是否在线
                var character = CharacterManager.Instance.GetCharacter(member.CharacterID);
                if (character != null)//角色在线
                {
                    //更新一下成员信息
                    memberInfo.charInfo = character.GetBasicInfo();
                    memberInfo.Status = true;
                    member.Level = character.TChar.Level;
                    member.Name = character.TChar.Name;
                    member.LastTime = DateTime.Now;
                }
                else//角色离线
                {
                    memberInfo.charInfo = this.GetMemberInfo(member);
                    memberInfo.Status = false;
                }
                //每拉取出来一个成员，就添加到返回值
                members.Add(memberInfo);
            }
            return members;
        }

        /// <summary>
        /// Get角色信息（DB改网络）
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        private NCharacterInfo GetMemberInfo(TGuildMember member)
        {
            return new NCharacterInfo
            {
                Id = member.Id,
                Name = member.Name,
                Class = (CharacterClass)member.Class,
                Level = member.Level,
            };
        }

        /// <summary>
        /// Get申请信息（DB改网络）
        /// </summary>
        /// <returns></returns>
        private List<NGuildApplyInfo> GetApplyInfos()
        {
            //构建返回值
            List<NGuildApplyInfo> applies = new List<NGuildApplyInfo>();
            //循环遍历数据的申请信息
            foreach (var apply in this.Data.Applies)
            {
                if (apply.Result != (int)ApplyResult.None) continue;
                //将每一条信息Add到网络信息中
                applies.Add(new NGuildApplyInfo()
                {
                    characterId = apply.CharacterID,
                    GuildId = apply.GuildId,
                    Class = apply.Class,
                    Level = apply.Level,
                    characterName = apply.Name,
                    Result = (ApplyResult)apply.Result,
                });
            }
            return applies;
        }

        private TGuildMember GetDBMember(int characterId)
        {
            foreach (var member in this.Data.Members)
            {
                if (member.CharacterID == characterId)
                    return member;
            }
            return null;
        }

        /// <summary>
        /// 执行行为
        /// </summary>
        /// <param name="command"></param>
        /// <param name="targetInfo"></param>
        /// <param name="characterId"></param>
        /// <returns></returns>
        public string ExecuteAdmin(GuildAdminCommand command, int targetInfo, int characterId)
        {
            var target = GetDBMember(targetInfo);
            var source = GetDBMember(characterId);
            if (characterId == this.Data.LeaderID)//判断是否是会长
            {
                switch (command)
                {
                    case GuildAdminCommand.Kickout://踢出公会
                        if (targetInfo == characterId)//判断目标是否是自己，如果是自己，则无法踢出自己
                            return "您无法将自己踢出公会";
                        Leave(CharacterManager.Instance.GetCharacter(target.CharacterID));
                        break;
                    case GuildAdminCommand.Promote://晋升
                        if (targetInfo == this.Data.LeaderID)//判断目标是否是会长，如果已经是会长，则无法再晋升我自己
                            return "您已是会长，已无法再晋升";
                        target.Duty = (int)GuildDuty.VicePresident;//晋升为副会长
                        break;
                    case GuildAdminCommand.Depost://罢免
                        if (targetInfo == this.Data.LeaderID)//判断目标是否是会长，如果已经是会长，则无法罢免我自己
                            return "您是会长，无法罢免自己，请先将会长一职转让给其他成员";
                        target.Duty = (int)GuildDuty.None;//降职为普通成员
                        break;
                    case GuildAdminCommand.Transfer://转让
                        if (targetInfo == this.Data.LeaderID)//判断目标是否是会长，如果已经是会长，则无法转让我自己
                            return "您已经是会长，无法转让给自己";
                        target.Duty = (int)GuildDuty.President;//目标为会长
                        source.Duty = (int)GuildDuty.None;//自己设置为普通成员
                        this.Data.LeaderID = targetInfo;
                        this.Data.LeaderName = target.Name;
                        break;
                    case GuildAdminCommand.ChangeInfo://更改公会信息
                        break;
                }
                DBService.Instance.Save();
                timestape = TimeUtil.timestamp;
                return string.Format("操作: {0} 成功", target);
            }
            else
                return string.Format("操作: {0} 失败", target);
        }

        /// <summary>
        /// 修改信息
        /// </summary>
        /// <param name="command"></param>
        /// <param name="guildNotice"></param>
        /// <param name="guildIcon"></param>
        /// <param name="id"></param>
        public bool ExecuteAdmin(GuildAdminCommand command, string guildNotice, string guildIcon, int id)
        {
            switch (command)
            {
                case GuildAdminCommand.ChangeInfo:
                    //判断公会公告是否为空或者Icon图标是否为空
                    if (guildNotice != null && guildNotice != "")
                    {
                        this.Data.Notice = guildNotice;
                        DBService.Instance.Save();
                    }
                    if (guildIcon != null && guildIcon != "")
                    {
                        this.Data.Icon = guildIcon;
                        DBService.Instance.Save();
                    }
                    timestape = TimeUtil.timestamp;
                    return true;
            }
            return false;
        }
    }
}
