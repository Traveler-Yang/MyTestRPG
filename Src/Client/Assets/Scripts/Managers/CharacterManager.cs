using SkillBridge.Message;
using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using Entities;

namespace Services
{
    class CharacterManager : Singleton<CharacterManager>, IDisposable
    {
        public Dictionary<int, Character> Characters = new Dictionary<int, Character>();

        public UnityAction<Character> OnCharacterEnter;

        public CharacterManager()
        {

        }

        public void Dispose()
        {
            
        }

        public void Init()
        {

        }

        public void Clear()
        {
            this.Characters.Clear();
        }
        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="cha"></param>
        public void AddCharacter(NCharacterInfo cha)
        {
            Debug.LogFormat("AddCharacter: {0} : {1} Map:{2} Entity:{3}", cha.Id, cha.Name, cha.mapId, cha.Entity.String());
            Character character = new Character(cha);
            this.Characters[cha.Id] = character;

            if (OnCharacterEnter != null)
            {
                OnCharacterEnter(character);
            }
        }
        /// <summary>
        /// 移除玩家
        /// </summary>
        /// <param name="characterId"></param>
        public void RemoveCharacter(int characterId)
        {
            Debug.LogFormat("RemoveCharacter:{0}",characterId);
            this.Characters.Remove(characterId);
        }
    }
}
