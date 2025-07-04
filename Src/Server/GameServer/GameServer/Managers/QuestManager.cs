using Common;
using Common.Data;
using GameServer.Entities;
using GameServer.Services;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class QuestManager
    {
        Character Owner;
        public QuestManager(Character owner)
        {
            this.Owner = owner;
        }

        public void GetQuestInfos(List<NQuestInfo> list)
        {
            foreach (var quest in Owner.TChar.Quests)
            {
                list.Add(GetQuestInfo(quest));
            }
        }

        /// <summary>
        /// 将DB中的任务数据转换为NQuestInfo的网络数据
        /// </summary>
        /// <param name="quest"></param>
        /// <returns></returns>
        public NQuestInfo GetQuestInfo(TCharacterQuest quest)
        {
            return new NQuestInfo
            {
                QuestId = quest.QuestID,
                QuestGuid = quest.Id,
                Status = (QuestStatus)quest.Status,
                Targets = new int[3]
                {
                    quest.Target1,
                    quest.Target2,
                    quest.Target3
                }
            };
        }

        /// <summary>
        /// 接受任务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="questId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Result AcceptQuest(NetConnection<NetSession> sender, int questId)
        {
            //先拿到所有者是谁，即发送请求的玩家
            Character character = sender.Session.Character;
            QuestDefine quest;
            //再验证任务是否存在
            if (DataManager.Instance.Quests.TryGetValue(questId, out quest))
            {
                var dbquest = DBService.Instance.Entities.CharacterQuests.Create();
                dbquest.QuestID = quest.ID;
                if (quest.Target1 == QuestTraget.None)
                {
                    //如果任务没有目标，则直接设置为完成状态
                    dbquest.Status = (int)QuestStatus.Complated;
                }
                else
                {
                    //如果任务有目标，则设置为接受未完成状态
                    dbquest.Status = (int)QuestStatus.InProgress;
                }
                sender.Session.Response.questAccept.Quest = this.GetQuestInfo(dbquest);
                character.TChar.Quests.Add(dbquest);
                DBService.Instance.Save();
                return Result.Success;
            }
            else
            {
                //如果任务不存在，则返回错误信息
                sender.Session.Response.questAccept.Errormsg = "任务不存在";
                return Result.Failed;
            }
        }

        /// <summary>
        /// 提交任务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="questId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Result SubmitQuest(NetConnection<NetSession> sender, int questId)
        {
            //同样先拿到所有者是谁，即发送请求的玩家
            Character character = sender.Session.Character;
            //再验证任务是否存在
            QuestDefine quest;
            if (DataManager.Instance.Quests.TryGetValue(questId, out quest))
            {
                //如果存在
                //查询数据库中是否有这个任务
                var dbquest = character.TChar.Quests.Where(q => q.QuestID == questId).FirstOrDefault();
                if (dbquest != null)
                {
                    if (dbquest.Status != (int)QuestStatus.Complated)
                    {
                        //如果不是完成状态
                        //返回错误信息
                        sender.Session.Response.questSubmit.Errormsg = "任务未完成，无法提交";
                        return Result.Failed;
                    }
                    //任务已完成，设置为已提交状态
                    dbquest.Status = (int)QuestStatus.Finished;
                    sender.Session.Response.questSubmit.Quest = this.GetQuestInfo(dbquest);
                    DBService.Instance.Save();

                    //给玩家奖励
                    if (quest.RewardGold > 0)
                    {
                        character.Gold += quest.RewardGold;
                    }
                    if (quest.RewardExp > 0)
                    {
                        //经验值奖励
                    }

                    //奖励物品
                    if (quest.RewardItem1 > 0)
                    {
                        character.ItemManager.AddItem(quest.RewardItem1, quest.RewardItem1Count);
                    }
                    if (quest.RewardItem2 > 0)
                    {
                        character.ItemManager.AddItem(quest.RewardItem2, quest.RewardItem2Count);
                    }
                    if (quest.RewardItem3 > 0)
                    {
                        character.ItemManager.AddItem(quest.RewardItem3, quest.RewardItem3Count);
                    }
                    DBService.Instance.Save();
                    return Result.Success;
                }
                //数据库中的任务不存在
                sender.Session.Response.questSubmit.Errormsg = "任务不存在[1002]";
                return Result.Failed;
            }
            else
            {
                //数据表里的任务不存在
                sender.Session.Response.questSubmit.Errormsg = "任务不存在[1001]";
                return Result.Failed;
            }
        }
    }
}
