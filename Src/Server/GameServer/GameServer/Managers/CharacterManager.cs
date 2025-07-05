using Common;
using GameServer.Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class CharacterManager : Singleton<CharacterManager>
    {
        public Dictionary<int, Character> Characters = new Dictionary<int, Character>();

        public CharacterManager()
        {

        }
        public void Dispose()
        {

        }
        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {

        }
        /// <summary>
        /// 清空角色字典
        /// </summary>
        public void Clear()
        {
            Characters.Clear();
        }
        /// <summary>
        /// 添加角色到字典
        /// </summary>
        /// <param name="cha"></param>
        /// <returns></returns>
        public Character AddCharacter(TCharacter cha)
        {
            Character character = new Character(CharacterType.Player, cha);
            EntityManager.Instance.AddEntity(cha.MapID, character);
            character.Info.EntityId = character.entityId;
            this.Characters[character.entityId] = character;
            return character;
        }
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="characterId"></param>
        public void RemoveCharacter(int characterId)
        {
            var cha = this.Characters[characterId];
            EntityManager.Instance.RemoveEntity(cha.TChar.MapID, cha);
            this.Characters.Remove(characterId);
        }

        public Character GetCharacter(int characterId)
        {
            foreach (var cha in this.Characters)
            {
                if (cha.Value.Id == characterId)
                    return cha.Value;
            }
            return null;
        }

        //public Character GetCharacter(int entityId)
        //{
        //    Character character = null;
        //    this.Characters.TryGetValue(entityId, out character);
        //    return character;
        //}
    }
}
