using Common.Data;
using GameServer.Core;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Managers;
using Network;
using GameServer.Models;
using Common;

namespace GameServer.Entities
{
    /// <summary>
    /// Character
    /// 玩家角色类
    /// </summary>
    class Character : CharacterBase,IPostResponser
    {
       
        public TCharacter TChar;

        public ItemManager ItemManager;
        public QuestManager QuestManager;
        public StatusManager StatusManager;
        public FriendManager FriendManager;

        public Temp temp;
        public double tempUpdateTS;//自己的队伍何时更新的时间戳

        public Guild guild;
        public double guildUpdateTS;//公会创建或加入更新时间戳

        public Chat chat;

        public Character(CharacterType type,TCharacter cha):
            base(new Core.Vector3Int(cha.MapPosX, cha.MapPosY, cha.MapPosZ),new Core.Vector3Int(100,0,0))
        {
            this.TChar = cha;
            this.Id = cha.ID;
            this.Info = new NCharacterInfo();
            this.Info.Type = type;
            this.Info.Id = cha.ID;
            this.Info.EntityId = this.entityId;
            this.Info.Name = cha.Name;
            this.Info.Level = 10;//cha.Level;
            this.Info.ConfigId = cha.TID;
            this.Info.Class = (CharacterClass)cha.Class;
            this.Info.mapId = cha.MapID;
            this.Info.Gold = cha.Gold;
            this.Info.Ride = 0;
            this.Info.Entity = this.EntityData;
            this.Define = DataManager.Instance.Characters[this.Info.ConfigId];

            this.ItemManager = new ItemManager(this);
            this.ItemManager.GetItemInfos(this.Info.Items);
            this.Info.Bag = new NBagInfo();
            this.Info.Bag.Unlocked = this.TChar.Bag.Unlocked;
            this.Info.Bag.Items = this.TChar.Bag.Items;
            this.Info.Equips = this.TChar.Equips;
            this.QuestManager = new QuestManager(this);
            this.QuestManager.GetQuestInfos(this.Info.Quests);
            this.StatusManager = new StatusManager(this);
            this.FriendManager = new FriendManager(this);
            this.FriendManager.GetFriendInfos(this.Info.Friends);

            this.guild = GuildManager.Instance.GetGuild(this.TChar.GuildId);

            this.chat = new Chat(this);
        }

        public long Gold
        {
            get { return this.TChar.Gold; }
            set
            {
                //要个金币赋值的话，先判断要赋值的值是否和当前金币是否相等
                //如果相等就返回
                if (this.TChar.Gold == value)
                    return;

                this.StatusManager.AddGoldChange((int)(value - this.TChar.Gold));
                this.TChar.Gold = value;
            }
        }

        public int Ride
        {
            get { return this.Info.Ride; }
            set
            {
                if (this.Info.Ride == value)
                    return;
                this.Info.Ride = value;
            }
        }

        public void PostProcess(NetMessageResponse message)
        {
            this.FriendManager.PostProcess(message);
            //当前身上有没有队伍
            if (this.temp != null)
            {
                Log.InfoFormat("Character > PostProcess Temp:Character{0}:{1} : {2}<{3}", this.Id, this.Info.Name, tempUpdateTS, temp.timestamp);
                //如果有，再判断自己的更新队伍时间是否小于此队伍的更新时间
                if (this.tempUpdateTS < temp.timestamp)
                {
                    this.tempUpdateTS = temp.timestamp;
                    this.temp.PostProcess(message);
                }
            }

            //判断有无公会
            if (this.guild != null)
            {
                Log.InfoFormat("Character > PostProcess Guild:Character{0}:{1} : {2}<{3}", this.Id, this.Info.Name, guildUpdateTS, guild.timestape);
                if (this.Info.Guild == null)//再次进行判断，网络消息中是否有公会，如果没有，则拉一份
                {
                    this.Info.Guild = this.guild.GuildInfo(this);
                    if (message.mapCharacterEnter != null)
                        guildUpdateTS = guild.timestape;
                }
                //判断自己公会的更新时间，是否小于公会的更新时间，如果小于，则进行更新
                if (guildUpdateTS < this.guild.timestape && message.mapCharacterEnter == null)
                {
                    guildUpdateTS = this.guild.timestape;
                    this.guild.Posprocess(this, message);
                }
            }

            if (this.StatusManager.HasStatus)
            {
                this.StatusManager.PostProcess(message);
            }

            this.chat.PostProcess(message);
        }
        /// <summary>
        /// 角色离开时调用
        /// </summary>
        public void Clear()
        {
            //离开时将自己的好友状态改变
            this.FriendManager.OfflineNotify();
        }

        public NCharacterInfo GetBasicInfo()
        {
            return new NCharacterInfo()
            {
                Id = this.Id,
                Name = this.Info.Name,
                Class = this.Info.Class,
                Level = this.Info.Level,
            };
        }
    }
}
